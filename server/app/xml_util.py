import xml.etree.ElementTree as ET
from xml.dom import minidom


def _text(value) -> str:
    if value is None:
        return ""
    return str(value).strip()


# Unity 客户端按 XML 子节点名称解析（如 readTime2、completeTime）
_XML_FIELDS = (
    ("username", "username"),
    ("password", "password"),
    ("role", "role"),
    ("name", "name"),
    ("major", "major"),
    ("class", "class"),
    ("score1", "score1"),
    ("score2", "score2"),
    ("attempt_times", "attemptTimes"),
    ("file1", "file1"),
    ("file1date", "file1date"),
    ("file2", "file2"),
    ("file2date", "file2date"),
    ("file3", "file3"),
    ("file3date", "file3date"),
    ("file4", "file4"),
    ("file4date", "file4date"),
    ("file5", "file5"),
    ("file5date", "file5date"),
    ("file6", "file6"),
    ("file6date", "file6date"),
    ("file7", "file7"),
    ("file7date", "file7date"),
    ("file8", "file8"),
    ("file8date", "file8date"),
    ("file9", "file9"),
    ("file9date", "file9date"),
    ("file10", "file10"),
    ("file10date", "file10date"),
    ("complete_time", "completeTime"),
    ("read_time1", "readTime1"),
    ("read_time2", "readTime2"),
    ("read_time3", "readTime3"),
)


def user_to_table_element(user: dict) -> ET.Element:
    table = ET.Element("Table")
    ET.SubElement(table, "id").text = _text(user.get("username"))
    for db_key, tag in _XML_FIELDS:
        ET.SubElement(table, tag).text = _text(user.get(db_key))
    return table


def users_to_xml(users: list[dict]) -> str:
    root = ET.Element("root")
    for user in users:
        root.append(user_to_table_element(user))
    rough = ET.tostring(root, encoding="unicode")
    parsed = minidom.parseString(f'<?xml version="1.0" encoding="utf-8"?>{rough}')
    return parsed.toprettyxml(indent="  ", encoding="utf-8").decode("utf-8")


def parse_users_xml(xml_text: str) -> list[dict]:
    root = ET.fromstring(xml_text)
    users = []
    for table in root.findall("Table"):
        if table.find("user") is not None:
            for user_node in table.findall("user"):
                users.append(_node_to_user(user_node))
        else:
            users.append(_table_to_user(table))
    for user_node in root.findall("user"):
        users.append(_node_to_user(user_node))
    return users


def _node_to_user(node: ET.Element) -> dict:
    data = {}
    for child in node:
        key = "class" if child.tag == "class" else child.tag
        data[key] = (child.text or "").strip()
    return data


def _table_to_user(table: ET.Element) -> dict:
    data = {}
    for child in table:
        key = "class" if child.tag == "class" else child.tag
        data[key] = (child.text or "").strip()
    return data
