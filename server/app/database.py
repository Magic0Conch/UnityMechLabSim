import sqlite3
from pathlib import Path

DATA_DIR = Path(__file__).resolve().parent.parent / "data"
DB_PATH = DATA_DIR / "mechlab.db"
STORAGE_DIR = DATA_DIR / "storage"
COMMON_DIR = STORAGE_DIR / "common"
UPLOADS_DIR = STORAGE_DIR / "uploads"

USER_COLUMNS = [
    "username", "password", "name", "major", "class", "role",
    "score1", "score2", "attempt_times",
    "file1", "file1date", "file2", "file2date", "file3", "file3date",
    "file4", "file4date", "file5", "file5date", "file6", "file6date",
    "file7", "file7date", "file8", "file8date", "file9", "file9date",
    "file10", "file10date",
    "complete_time", "read_time1", "read_time2", "read_time3",
]

DISPLAY_DEFAULTS = {
    "score1": "暂无", "score2": "暂无",
    "file1date": "未提交", "file2date": "未提交", "file3date": "未提交",
    "file4date": "未提交", "file5date": "未提交", "file6date": "未提交",
    "file7date": "未提交", "file8date": "未提交", "file9date": "未提交",
    "file10date": "未提交",
    "complete_time": "未完成", "read_time1": "未完成",
    "read_time2": "未完成", "read_time3": "未完成",
    "name": "暂无", "major": "暂无", "class": "暂无", "password": "暂无",
}


def ensure_dirs() -> None:
    DATA_DIR.mkdir(parents=True, exist_ok=True)
    COMMON_DIR.mkdir(parents=True, exist_ok=True)
    UPLOADS_DIR.mkdir(parents=True, exist_ok=True)


def get_connection() -> sqlite3.Connection:
    ensure_dirs()
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    return conn


def init_db() -> None:
    ensure_dirs()
    with get_connection() as conn:
        conn.execute(
            """
            CREATE TABLE IF NOT EXISTS users (
                username TEXT PRIMARY KEY,
                password TEXT NOT NULL,
                name TEXT DEFAULT '暂无',
                major TEXT DEFAULT '暂无',
                class TEXT DEFAULT '暂无',
                role TEXT NOT NULL DEFAULT 'student',
                score1 TEXT DEFAULT '暂无',
                score2 TEXT DEFAULT '暂无',
                attempt_times TEXT DEFAULT '未完成',
                file1 TEXT, file1date TEXT DEFAULT '未提交',
                file2 TEXT, file2date TEXT DEFAULT '未提交',
                file3 TEXT, file3date TEXT DEFAULT '未提交',
                file4 TEXT, file4date TEXT DEFAULT '未提交',
                file5 TEXT, file5date TEXT DEFAULT '未提交',
                file6 TEXT, file6date TEXT DEFAULT '未提交',
                file7 TEXT, file7date TEXT DEFAULT '未提交',
                file8 TEXT, file8date TEXT DEFAULT '未提交',
                file9 TEXT, file9date TEXT DEFAULT '未提交',
                file10 TEXT, file10date TEXT DEFAULT '未提交',
                complete_time TEXT DEFAULT '未完成',
                read_time1 TEXT DEFAULT '未完成',
                read_time2 TEXT DEFAULT '未完成',
                read_time3 TEXT DEFAULT '未完成'
            )
            """
        )
        conn.execute(
            """
            CREATE TABLE IF NOT EXISTS common_files (
                name TEXT PRIMARY KEY,
                storage_path TEXT NOT NULL,
                updated_at TEXT NOT NULL
            )
            """
        )
        count = conn.execute("SELECT COUNT(*) FROM users").fetchone()[0]
        if count == 0:
            seed_users(conn)
        conn.commit()


def seed_users(conn: sqlite3.Connection) -> None:
    samples = [
        ("teacher", "teacher123", "张老师", "机电", "教师组", "teacher"),
        ("yigengli@ysu.edu.cn", "ysu138352", "李一庚", "机电", "教师组", "teacher"),
        ("student1", "123456", "张三", "机械", "2021-1", "student"),
        ("student2", "123456", "李四", "机械", "2021-2", "student"),
    ]
    for u, p, name, major, cls, role in samples:
        conn.execute(
            """
            INSERT INTO users (username, password, name, major, class, role)
            VALUES (?, ?, ?, ?, ?, ?)
            """,
            (u, p, name, major, cls, role),
        )
    # 示例公共文件：实验目的内容（学生端 b0 下载）
    sample_zip = COMMON_DIR / "实验目的内容.zip"
    if not sample_zip.exists():
        import zipfile
        with zipfile.ZipFile(sample_zip, "w") as zf:
            zf.writestr("readme.txt", "本地测试用示例压缩包\n")
    conn.execute(
        """
        INSERT OR REPLACE INTO common_files (name, storage_path, updated_at)
        VALUES (?, ?, datetime('now'))
        """,
        ("实验目的内容", str(sample_zip)),
    )
    for name in ("videos", "Test1", "Test2", "职工素养测试", "实验安全考试", "m2", "m3"):
        txt = COMMON_DIR / f"{name}.txt"
        if not txt.exists():
            txt.write_text(f"本地占位文件：{name}\n", encoding="utf-8")
        conn.execute(
            """
            INSERT OR REPLACE INTO common_files (name, storage_path, updated_at)
            VALUES (?, ?, datetime('now'))
            """,
            (name, str(txt)),
        )
