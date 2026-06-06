# -*- coding: utf-8 -*-
"""将示范题库同步到 server/data/storage/common 并更新数据库。"""
import shutil
import sqlite3
from pathlib import Path

ROOT = Path(__file__).resolve().parent
PROJECT = ROOT.parent
COMMON = PROJECT / "server" / "data" / "storage" / "common"
DB = PROJECT / "server" / "data" / "mechlab.db"

FILES = [
    ("01-安全操作考试/实验安全考试", "实验安全考试"),
    ("02-职工素养测试/职工素养测试", "职工素养测试"),
]

COMMON.mkdir(parents=True, exist_ok=True)
conn = sqlite3.connect(DB)
for rel, name in FILES:
    src = ROOT / f"{rel}.xlsx"
    dst = COMMON / f"{name}.xlsx"
    shutil.copy2(src, dst)
    conn.execute(
        "INSERT OR REPLACE INTO common_files (name, storage_path, updated_at) "
        "VALUES (?, ?, datetime('now'))",
        (name, str(dst)),
    )
conn.commit()
conn.close()
print("已同步:", ", ".join(n for _, n in FILES))
