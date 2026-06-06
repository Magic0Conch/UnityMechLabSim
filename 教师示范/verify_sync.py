# -*- coding: utf-8 -*-
import sqlite3
from pathlib import Path

DB = Path(__file__).resolve().parent.parent / "server" / "data" / "mechlab.db"
conn = sqlite3.connect(DB)
for name in ("实验安全考试", "职工素养测试"):
    row = conn.execute(
        "SELECT storage_path FROM common_files WHERE name = ?", (name,)
    ).fetchone()
    path = Path(row[0]) if row else None
    ok = path and path.exists() and path.suffix == ".xlsx"
    print(name, "OK" if ok else "MISSING", path)
