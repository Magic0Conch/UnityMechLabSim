import io
import re
import shutil
import sqlite3
import zipfile
from datetime import datetime
from pathlib import Path
from typing import Any
from xml.etree import ElementTree as ET

from fastapi import HTTPException
from fastapi.responses import PlainTextResponse, Response

from .database import (
    COMMON_DIR,
    DISPLAY_DEFAULTS,
    UPLOADS_DIR,
    get_connection,
)
from .xml_util import parse_users_xml, users_to_xml

FILE_INDEX_MAP = {str(i): f"file{i}" for i in range(1, 11)}
READ_TIME_MAP = {"1": "read_time1", "2": "read_time2", "3": "read_time3"}


def row_to_user(row) -> dict:
    keys = row.keys()
    data = {k: row[k] for k in keys}
    if data.get("class") is None:
        data["class"] = "暂无"
    for k, default in DISPLAY_DEFAULTS.items():
        if k in data and (data[k] is None or data[k] == ""):
            data[k] = default
    return data


def get_user(username: str) -> dict | None:
    with get_connection() as conn:
        row = conn.execute(
            "SELECT * FROM users WHERE username = ?", (username,)
        ).fetchone()
    return row_to_user(row) if row else None


def _now_label(ori_name: str = "") -> str:
    """教师端/学生端展示：原文件名（上传时间）"""
    stamp = datetime.now().strftime("%Y-%m-%d %H:%M")
    if ori_name:
        return f"{ori_name}（{stamp}）"
    return stamp


def _ori_name_from_label(label: str, fallback: str) -> str:
    """从 fileNdate 解析学生上传时的原始文件名（兼容旧数据 date+name）。"""
    if not label or label in ("未提交", "暂无", "未完成"):
        return fallback
    if "（" in label and label.endswith("）"):
        return label[: label.index("（")].strip()
    if "+" in label:
        return label.split("+", 1)[1].strip()
    return fallback


def _safe_zip_name(name: str) -> str:
    return name.replace("\\", "_").replace("/", "_").strip() or "upload"


def _safe_storage_name(ori_name: str, field: str, ext: str) -> str:
    """磁盘文件名：优先保留学生上传的原始文件名。"""
    base = Path((ori_name or "").strip()).name
    base = base.replace("\\", "_").replace("/", "_").strip()
    if not base or base in (".", ".."):
        return f"{field}{ext or '.bin'}"
    return base[:200]


async def handle_command(params: dict[str, Any], upload) -> Response:
    cmd = (params.get("requesttype") or "").strip().upper()
    if not cmd:
        raise HTTPException(400, "missing requesttype")

    handlers = {
        "LOGIN": handle_login,
        "UPLOAD": handle_upload,
        "UPLOADCOMMONFILE": handle_upload_common,
        "DOWNLOAD": handle_download_list,
        "DOWNCOMMON": handle_down_common,
        "GETSINGLEINFO": handle_get_single_info,
        "GETALLUSERINFO": handle_get_all_user_info,
        "GEIINFO": handle_gei_info,
        "ADDUSERS": handle_add_users,
        "DELETEUSERS": handle_delete_users,
        "INSERTSINGLEUSER": handle_insert_single_user,
        "UPDATE": handle_update_user,
        "SUBMITSCORE": handle_submit_score,
        "SUBMITTIME": handle_submit_time,
        "UPCOMPELETETIME": handle_up_complete_time,
    }
    fn = handlers.get(cmd)
    if not fn:
        return PlainTextResponse(f"unsupported requesttype: {cmd}", status_code=400)
    if cmd in ("UPLOAD", "UPLOADCOMMONFILE"):
        return await fn(params, upload)
    return fn(params, upload)


def _register_student(username: str) -> dict | None:
    """首次登录时自动创建学生账号。"""
    with get_connection() as conn:
        try:
            conn.execute(
                """
                INSERT INTO users (username, password, name, major, class, role)
                VALUES (?, ?, ?, ?, ?, 'student')
                """,
                (username, "", username, "暂无", "暂无"),
            )
            conn.commit()
        except sqlite3.IntegrityError:
            pass
    return get_user(username)


def handle_login(params: dict, upload) -> Response:
    username = (params.get("username") or "").strip()
    password = (params.get("password") or "").strip()
    if len(username) < 6:
        return PlainTextResponse("FAILED")

    user = get_user(username)
    if not user:
        user = _register_student(username)
    if not user:
        return PlainTextResponse("FAILED")

    if user["role"] == "teacher":
        if user["password"] != password:
            return PlainTextResponse("FAILED")
        return PlainTextResponse("TEACHERSUCCESS")
    # 学生账号不校验密码；不存在时已自动入库
    return PlainTextResponse("STUDENTSUCCESS")


async def _save_upload_file(upload, dest: Path) -> None:
    dest.parent.mkdir(parents=True, exist_ok=True)
    data = await upload.read()
    dest.write_bytes(data)


async def handle_upload(params: dict, upload) -> Response:
    """学生作业上传（multipart 带 file）或 OSS 回调（GET 查询参数，无文件）。"""
    username = (params.get("username") or params.get("filename") or "").strip()
    file_in_db = (params.get("fileInDb") or "").strip()
    ori_name = (params.get("oriName") or "upload").strip()
    folder = (params.get("folderName") or "misc").strip()

    if not username:
        return PlainTextResponse("FAILED")

    field = FILE_INDEX_MAP.get(file_in_db)
    if not field:
        return PlainTextResponse("OK")

    storage_path = None
    if upload is not None:
        ext = Path(ori_name).suffix
        if not ext and getattr(upload, "filename", None):
            ext = Path(str(upload.filename)).suffix
        file_name = _safe_storage_name(ori_name, field, ext)
        rel = Path(username) / folder / file_name
        dest = UPLOADS_DIR / rel
        with get_connection() as conn:
            row = conn.execute(
                f"SELECT {field} FROM users WHERE username = ?", (username,)
            ).fetchone()
            if row and row[field]:
                old = UPLOADS_DIR / str(row[field]).replace("\\", "/")
                if old.is_file() and old.resolve() != dest.resolve():
                    old.unlink()
        await _save_upload_file(upload, dest)
        storage_path = str(rel).replace("\\", "/")

    label = _now_label(ori_name)
    with get_connection() as conn:
        if storage_path:
            conn.execute(
                f"UPDATE users SET {field} = ?, {field}date = ? WHERE username = ?",
                (storage_path, label, username),
            )
        else:
            conn.execute(
                f"UPDATE users SET {field}date = ? WHERE username = ?",
                (label, username),
            )
        conn.commit()
    return PlainTextResponse("OK")


async def handle_upload_common(params: dict, upload) -> Response:
    name = (params.get("username") or params.get("folderName") or "").strip()
    if not name:
        return PlainTextResponse("FAILED")

    ext = (params.get("filetype") or "").strip()
    if ext and not ext.startswith("."):
        ext = "." + ext

    storage_path = COMMON_DIR / f"{name}{ext or ''}"
    if upload is not None:
        await _save_upload_file(upload, storage_path)
    elif not storage_path.exists():
        return PlainTextResponse("FAILED")

    with get_connection() as conn:
        conn.execute(
            """
            INSERT OR REPLACE INTO common_files (name, storage_path, updated_at)
            VALUES (?, ?, datetime('now'))
            """,
            (name, str(storage_path)),
        )
        conn.commit()
    return PlainTextResponse("OK")


def handle_down_common(params: dict, upload) -> Response:
    name = (params.get("username") or "").strip()
    if not name:
        raise HTTPException(400, "missing username")

    with get_connection() as conn:
        row = conn.execute(
            "SELECT storage_path FROM common_files WHERE name = ?", (name,)
        ).fetchone()

    if not row:
        return PlainTextResponse("", status_code=404)

    path = Path(row["storage_path"])
    if not path.is_file():
        return PlainTextResponse("", status_code=404)

    # GET：返回 OSS 对象键或本地路径（供 AliyunOSSWithProcess 使用）
    if upload is None and params.get("_method") == "GET":
        return PlainTextResponse(path.name)

    data = path.read_bytes()
    filename = path.name
    return Response(
        content=data,
        media_type="application/octet-stream",
        headers={"Content-Disposition": f'attachment; filename="{filename}"'},
    )


def _collect_student_file_paths(names: list[str], folder: str) -> list[tuple[str, Path]]:
    file_field = None
    m = re.fullmatch(r"file(\d+)", folder, re.I)
    if m:
        file_field = f"file{m.group(1)}"

    collected: list[tuple[str, Path]] = []
    with get_connection() as conn:
        for name in names:
            row = conn.execute(
                "SELECT * FROM users WHERE username = ?", (name,)
            ).fetchone()
            if not row:
                continue
            if file_field:
                rel = row[file_field]
                if rel:
                    full = UPLOADS_DIR / str(rel).replace("\\", "/")
                    if full.is_file():
                        label = row[f"{file_field}date"]
                        ori = _ori_name_from_label(str(label or ""), full.name)
                        arc = f"{name}/{_safe_zip_name(ori)}"
                        collected.append((arc, full))
            else:
                for i in range(1, 11):
                    rel = row[f"file{i}"]
                    if not rel:
                        continue
                    full = UPLOADS_DIR / str(rel).replace("\\", "/")
                    if full.is_file():
                        label = row[f"file{i}date"]
                        ori = _ori_name_from_label(str(label or ""), full.name)
                        arc = f"{name}/{_safe_zip_name(ori)}"
                        collected.append((arc, full))
    return collected


def handle_download_list(params: dict, upload) -> Response:
    """POST：返回 zip 包；GET：返回逗号分隔的相对路径（兼容旧客户端）。"""
    names_raw = (params.get("username") or "").strip()
    folder = (params.get("folderName") or "").strip()
    names = [n.strip() for n in names_raw.split(",") if n.strip()]

    if params.get("_method") == "POST":
        files = _collect_student_file_paths(names, folder)
        if not files:
            return PlainTextResponse("无文件", status_code=404)
        buf = io.BytesIO()
        with zipfile.ZipFile(buf, "w", zipfile.ZIP_DEFLATED) as zf:
            for arc, full in files:
                zf.write(full, arcname=arc)
        return Response(
            content=buf.getvalue(),
            media_type="application/zip",
        )

    keys = [arc for arc, _ in _collect_student_file_paths(names, folder)]
    return PlainTextResponse(",".join(keys))


def resolve_common_file(name: str) -> Path | None:
    with get_connection() as conn:
        row = conn.execute(
            "SELECT storage_path FROM common_files WHERE name = ?", (name,)
        ).fetchone()
    if row:
        p = Path(row["storage_path"])
        if p.is_file():
            return p
    for candidate in (
        COMMON_DIR / name,
        COMMON_DIR / f"{name}.pdf",
        COMMON_DIR / f"{name}.txt",
        COMMON_DIR / f"{name}.xls",
        COMMON_DIR / f"{name}.zip",
    ):
        if candidate.is_file():
            return candidate
    return None


def handle_get_single_info(params: dict, upload) -> Response:
    username = (params.get("username") or "").strip()
    prop = (params.get("propName") or "").strip()
    user = get_user(username)
    if not user:
        return PlainTextResponse("")

    key = prop
    if prop.startswith("readTime") and prop[8:].isdigit():
        key = f"read_time{prop[8:]}"
    elif prop == "completeTime":
        key = "complete_time"
    elif prop in ("class",):
        key = "class"

    val = user.get(key, user.get(prop, ""))
    return PlainTextResponse(_text(val))


def handle_get_all_user_info(params: dict, upload) -> Response:
    with get_connection() as conn:
        rows = conn.execute("SELECT * FROM users ORDER BY username").fetchall()
    users = [row_to_user(r) for r in rows]
    xml = users_to_xml(users)
    return Response(content=xml, media_type="application/xml; charset=utf-8")


def handle_gei_info(params: dict, upload) -> Response:
    username = (params.get("username") or "").strip()
    user = get_user(username)
    if not user:
        return PlainTextResponse("")
    xml = users_to_xml([user])
    return Response(content=xml, media_type="application/xml; charset=utf-8")


def handle_add_users(params: dict, upload) -> Response:
    xml_body = (params.get("file") or "").strip()
    if not xml_body:
        return PlainTextResponse("0")
    try:
        users = parse_users_xml(xml_body)
    except ET.ParseError:
        return PlainTextResponse("0")

    inserted = 0
    with get_connection() as conn:
        for u in users:
            username = (u.get("username") or "").strip()
            if not username:
                continue
            password = u.get("password") or username
            name = u.get("name") or "暂无"
            major = u.get("major") or "暂无"
            cls = u.get("class") or "暂无"
            role = u.get("role") or "student"
            try:
                conn.execute(
                    """
                    INSERT INTO users (username, password, name, major, class, role)
                    VALUES (?, ?, ?, ?, ?, ?)
                    """,
                    (username, password, name, major, cls, role),
                )
                inserted += 1
            except sqlite3.IntegrityError:
                pass
        conn.commit()
    return PlainTextResponse(str(inserted))


def handle_delete_users(params: dict, upload) -> Response:
    names_raw = (params.get("username") or "").strip()
    names = [n.strip() for n in names_raw.split(",") if n.strip()]
    if not names:
        return PlainTextResponse("0")
    deleted = 0
    with get_connection() as conn:
        for n in names:
            cur = conn.execute("DELETE FROM users WHERE username = ?", (n,))
            deleted += cur.rowcount
        conn.commit()
    return PlainTextResponse(str(deleted))


def handle_insert_single_user(params: dict, upload) -> Response:
    username = (params.get("username") or "").strip()
    if not username:
        return PlainTextResponse("False")
    password = params.get("password") or username
    name = params.get("name") or "暂无"
    major = params.get("major") or "暂无"
    cls = params.get("theClass") or params.get("class") or "暂无"
    with get_connection() as conn:
        try:
            conn.execute(
                """
                INSERT INTO users (username, password, name, major, class, role)
                VALUES (?, ?, ?, ?, ?, 'student')
                """,
                (username, password, name, major, cls),
            )
            conn.commit()
            return PlainTextResponse("True")
        except Exception:
            return PlainTextResponse("False")


def handle_update_user(params: dict, upload) -> Response:
    username = (params.get("username") or "").strip()
    if not username:
        return PlainTextResponse("False")
    mapping: dict[str, str] = {}
    if params.get("password"):
        mapping["password"] = str(params["password"]).strip()
    if params.get("name"):
        mapping["name"] = str(params["name"]).strip()
    if params.get("major"):
        mapping["major"] = str(params["major"]).strip()
    cls = params.get("theClass") or params.get("class")
    if cls:
        mapping["class"] = str(cls).strip()
    if not mapping:
        return PlainTextResponse("True")
    sets = [f"{col} = ?" for col in mapping]
    vals = list(mapping.values()) + [username]
    with get_connection() as conn:
        conn.execute(
            f"UPDATE users SET {', '.join(sets)} WHERE username = ?",
            vals,
        )
        conn.commit()
    return PlainTextResponse("True")


def handle_submit_score(params: dict, upload) -> Response:
    username = (params.get("username") or "").strip()
    field = (params.get("fileInDb") or "").strip()
    score = (params.get("filename") or "").strip()
    if field not in ("score1", "score2") or not username:
        return PlainTextResponse("FAILED")
    with get_connection() as conn:
        conn.execute(
            f"UPDATE users SET {field} = ? WHERE username = ?",
            (score, username),
        )
        conn.commit()
    return PlainTextResponse("OK")


def handle_submit_time(params: dict, upload) -> Response:
    username = (params.get("username") or "").strip()
    code = (params.get("fileInDb") or "").strip()
    seconds = (params.get("propName") or "0").strip()
    col = READ_TIME_MAP.get(code)
    if not username or not col:
        return PlainTextResponse("FAILED")
    with get_connection() as conn:
        row = conn.execute(
            f"SELECT {col} FROM users WHERE username = ?", (username,)
        ).fetchone()
        total = 0
        if row and row[0] and str(row[0]).replace("未完成", "").isdigit():
            total = int(row[0])
        try:
            total += int(float(seconds))
        except ValueError:
            pass
        conn.execute(
            f"UPDATE users SET {col} = ? WHERE username = ?",
            (str(total), username),
        )
        conn.commit()
    return PlainTextResponse("OK")


def handle_up_complete_time(params: dict, upload) -> Response:
    username = (params.get("username") or "").strip()
    # Unity 误用 propName 字段传 completeTime 数值
    value = (params.get("propName") or params.get("completeTime") or "").strip()
    if not username:
        return PlainTextResponse("FAILED")
    with get_connection() as conn:
        conn.execute(
            "UPDATE users SET complete_time = ? WHERE username = ?",
            (value, username),
        )
        conn.commit()
    return PlainTextResponse("OK")


def _text(value) -> str:
    if value is None:
        return ""
    return str(value).strip()
