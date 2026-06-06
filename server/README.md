# MechLab 轻量后端

FastAPI + SQLite，兼容 Unity 客户端 `requesttype` 协议（原 `http://host/api/values`）。

> 公共服务器部署见项目根目录 **《部署手册.md》**（支持 Linux 与 **Windows Server**）；教师与学生操作见 **《用户使用手册.md》**。

## 环境要求

- Python 3.10+

## 第一次启动

```powershell
cd e:\work\Unity\UnityMechLabSim-main\server
python -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r requirements.txt
uvicorn app.main:app --host 0.0.0.0 --port 8899 --reload
```

浏览器打开 http://localhost:8899/health 应返回 `{"status":"ok"}`。

> Windows 上部分端口（如 5040）可能被系统保留无法绑定，默认使用 **8899**。

## 预置账号

| 用户名 | 密码 | 角色 |
|--------|------|------|
| teacher | teacher123 | 教师 → Tscene |
| student1 | 123456 | 学生 → MainScene |
| student2 | 123456 | 学生 |

数据库与上传文件目录：`server/data/`（已 gitignore）。

## Unity 连接

1. 打开场景 `登录.unity`，Play。
2. 服务器地址填：`http://localhost:8899/api/values`（LoginUI 默认已是该地址）。
3. 若曾保存过旧地址，清空 PlayerPrefs 或手动改 URI 输入框后点确认。

## 本地可测功能

- 登录、学生进度 XML（GEIINFO）、教师用户列表（GETALLUSERINFO）
- 成绩提交（SUBMITSCORE）、阅读时长（SUBMITTIME）、实验完成时间（UPCOMPELETETIME）
- 公共文件 **POST 下载**（DOWNCOMMON + getFileDownAdType）
- 用户增删改、Excel 批量导入（ADDUSERS）
- 经 **HTTP multipart** 的学生文件上传（UPLOAD）

## 传输方式

客户端已改为 **纯 HTTP**（`Web.cs` multipart），文件落在 `server/data/storage/`。PDF 等公共资源也可通过 `http://localhost:8899/files/{名称}` 访问。

教师批量下载：POST `DOWNLOAD`，服务端直接返回 zip。

> 场景里若仍挂着 `AliyunOSSWithProcess` 组件可手动移除，脚本已不再引用。

## 常用 API 自测

```powershell
# 登录
curl -X POST "http://localhost:8899/api/values" -F "requesttype=LOGIN" -F "username=student1" -F "password=123456"

# 学生进度
curl -X POST "http://localhost:8899/api/values" -F "requesttype=GEIINFO" -F "username=student1"
```
