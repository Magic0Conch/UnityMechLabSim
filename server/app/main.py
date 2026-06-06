from typing import Any

from fastapi import FastAPI, HTTPException, Request
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import FileResponse, PlainTextResponse
from starlette.formparsers import MultiPartParser

from .database import init_db
from .handlers import handle_command, handle_down_common, resolve_common_file

# Unity 上传视频等可能超过 Starlette 默认 1MB 限制
MultiPartParser.max_file_size = 500 * 1024 * 1024
MultiPartParser.max_part_size = 500 * 1024 * 1024

app = FastAPI(title="MechLab API", version="1.0.0")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.on_event("startup")
def on_startup() -> None:
    init_db()


async def _parse_request(request: Request) -> tuple[dict[str, Any], Any]:
    """只读取一次 multipart，兼容 Unity（file 字段可能无 filename）。"""
    params: dict[str, Any] = dict(request.query_params)
    upload = None

    if request.method == "POST":
        form = await request.form()
        for key, value in form.items():
            if hasattr(value, "read"):
                if key == "file":
                    upload = value
                continue
            params[key] = value

    params["_method"] = request.method
    return params, upload


@app.api_route("/api/values", methods=["GET", "POST"])
async def api_values(request: Request):
    params, upload = await _parse_request(request)

    cmd = (params.get("requesttype") or "").strip().upper()
    if cmd == "DOWNCOMMON" and request.method == "GET":
        return handle_down_common(params, None)

    return await handle_command(params, upload)


@app.get("/files/{name}")
def serve_public_file(name: str) -> FileResponse:
    path = resolve_common_file(name)
    if not path:
        raise HTTPException(404, "file not found")
    return FileResponse(path)


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok"}


@app.get("/")
def root() -> PlainTextResponse:
    return PlainTextResponse(
        "MechLab 本地 API\n"
        "Unity 连接地址: http://localhost:8899/api/values\n"
        "健康检查: /health\n"
    )
