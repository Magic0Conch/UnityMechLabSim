# -*- coding: utf-8 -*-
"""生成教师示范题库（xlsx）。运行: python generate_demo.py"""
from pathlib import Path

from openpyxl import Workbook

ROOT = Path(__file__).resolve().parent

EXAMS = {
    "01-安全操作考试/实验安全考试": {
        "draw": 3,
        "rows": [
            ("序号", "题型", "题目", "答案", "选项A", "选项B", "选项C", "选项D"),
            (
                1,
                "选择",
                "电火花加工前必须佩戴的防护装备是？",
                "A",
                "安全眼镜",
                "拖鞋",
                "手套即可不护目",
                "无需防护",
            ),
            (
                2,
                "选择",
                "机床运行中发现异常振动应首先？",
                "B",
                "继续加工",
                "按下急停并报告",
                "自行拆机检查",
                "暂不处理",
            ),
            (
                3,
                "填空",
                "操作前检查工作液油位应在刻度线的什么位置？",
                "上下限之间",
                "",
                "",
                "",
                "",
            ),
            (
                4,
                "选择",
                "加工结束后下列哪项是正确关机顺序第一步？",
                "C",
                "直接关闭总电源",
                "卸下工件",
                "抬起电极Z轴",
                "打开机床门",
            ),
            (
                5,
                "选择",
                "磁力吸盘应在何时断电消磁？",
                "B",
                "加工过程中",
                "确认工件装夹可靠后按需操作",
                "开门瞬间",
                "任何时候",
            ),
        ],
    },
    "02-职工素养测试/职工素养测试": {
        "draw": 4,
        "rows": [
            ("序号", "题型", "题目", "答案", "选项A", "选项B", "选项C", "选项D"),
            (
                1,
                "选择",
                "团队协作中最重要的是？",
                "C",
                "个人表现",
                "推卸责任",
                "主动沟通配合",
                "独断专行",
            ),
            (
                2,
                "选择",
                "发现同事违规操作应？",
                "B",
                "视而不见",
                "及时提醒并报告",
                "网络传播",
                "效仿违规",
            ),
            (
                3,
                "填空",
                "实验报告一般应在实验结束后几天内提交？",
                "7",
                "",
                "",
                "",
                "",
            ),
            (
                4,
                "选择",
                "加工图纸尺寸标注应遵循？",
                "A",
                "国家标准GB",
                "随意标注",
                "口头约定",
                "无需标注",
            ),
            (
                5,
                "选择",
                "实验现场发现安全隐患首先要？",
                "B",
                "自行处理",
                "报告指导教师",
                "继续使用设备",
                "等待他人发现",
            ),
        ],
    },
}


def write_xlsx(rel_path: str, draw: int, rows: list) -> None:
    wb = Workbook()
    ws = wb.active
    ws.title = "Sheet1"
    ws.append(["题目数", draw])
    for row in rows:
        ws.append(list(row))
    out = ROOT / f"{rel_path}.xlsx"
    out.parent.mkdir(parents=True, exist_ok=True)
    wb.save(out)


def write_csv(rel_path: str, draw: int, rows: list) -> None:
    out = ROOT / f"{rel_path}.csv"
    out.parent.mkdir(parents=True, exist_ok=True)
    lines = [f", {draw}"] + [",".join(_csv_field(c) for c in row) for row in rows]
    out.write_text("\n".join(lines) + "\n", encoding="utf-8-sig")


def _csv_field(value) -> str:
    s = "" if value is None else str(value)
    if "," in s or '"' in s or "\n" in s:
        return '"' + s.replace('"', '""') + '"'
    return s


if __name__ == "__main__":
    for rel, spec in EXAMS.items():
        write_xlsx(rel, spec["draw"], spec["rows"])
        write_csv(rel, spec["draw"], spec["rows"])
    print("done:", ROOT)
