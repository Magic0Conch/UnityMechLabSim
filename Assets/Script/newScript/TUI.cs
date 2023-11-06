using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using UnityEngine.Events;

public class TUI : MonoBehaviour
{
    /// <summary>
    /// D：下载选中用户信息到Excel 下载选中人员分工 下载选中模型确定 下载选中加工工艺安排 下载选中凸/凹模加工 下载选中成品检验 下载选中实验总结
    /// U：上传实验指导书，上传实验报告，上传实验安全考试，上传职工素养测试
    /// S：返回主菜单 退出
    /// O：重新载入 插入一项 插入多项 删除选中项 全选 全不选
    /// 
    /// </summary>
    public ZipUtility zipUtility;
    public AliyunOSSWithProcess aoss;
    public Transform content;
    public GameObject Item,InsertSingleItem,personalInfoPannel;
    public Dropdown Major, Theclass, Column, CompeleteState;
    public InputField UsernameInput,videos;
    public Toggle toggle;
    Web web;
    List<User> users;
    Dictionary<string, HashSet<string>> majorToClass;
    XmlDocument xmlDocument = new XmlDocument();
    InputField[] insertIn;
    class User
    {
        public Dictionary<string, string> info = new Dictionary<string, string>();
        public User()
        {
            info.Add("username", "暂无");
            info.Add("password", "暂无");
            info.Add("name", "暂无");
            info.Add("major", "暂无");
            info.Add("class", "暂无");
            info.Add("score1", "暂无");
            info.Add("score2", "暂无");
            //info.Add("attemptTimes", "暂无");
            info.Add("file1date", "未提交");
            info.Add("file2date", "未提交");
            info.Add("file3date", "未提交");
            info.Add("file4date", "未提交");
            info.Add("file5date", "未提交");
            info.Add("file6date", "未提交");
            info.Add("file7date", "未提交");
            info.Add("file8date", "未提交");
            info.Add("file9date", "未提交");
            info.Add("file10date", "未提交");
            info.Add("completeTime", "未完成");
            info.Add("readTime1", "未完成");
            info.Add("readTime2", "未完成");
            info.Add("readTime3", "未完成");
        }
    }
    public GameObject UIPannel, remoteControl, hint, personalPannel, transimitProcess;
    string[] pns = new string[] { "username", "password","name","major","class","score1","score2", "readTime1", "readTime2", "readTime3", "file1date", "file2date", "file3date", "file4date", "file5date", "file6date", "completeTime", "file7date", "file8date", "file9date", "file10date" };
    private void Start()
    {
        majorToClass = new Dictionary<string, HashSet<string>>();
        web = GetComponent<Web>();
        users = new List<User>();
        insertIn = InsertSingleItem.transform.GetComponentsInChildren<InputField>();
        Reload();
    }
    public void Close(GameObject go)
    {
        go.SetActive(false);
    }
    public void Open(GameObject go)
    {
        go.SetActive(true);
    }
    public void DChange(Dropdown dp)
    {
        int index = dp.value;
        GameObject go = transimitProcess;
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        Slider slider = go.transform.GetChild(0).GetComponent<Slider>();
        if(index!=0&&index!=11)
        {
            transimitProcess.SetActive(true);
            formData.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.DOWNLOAD.ToString()));

        }
        string path = WebData.SaveProject();
        string rand = Path.GetRandomFileName();
        rand = rand.Substring(0, rand.Length - 4);
        if (index == 0)//下载用户信息到Excel
        {
            path += "\\" + rand + ".xls";
            int len = content.childCount;
            HSSFWorkbook excelBook = new HSSFWorkbook();
            ICellStyle style = excelBook.CreateCellStyle();
            ISheet sheet1 = excelBook.CreateSheet("学生信息表");
            IRow row1 = sheet1.CreateRow(0);
            style.Alignment = HorizontalAlignment.GENERAL;
            //给标题的每一个单元格赋值
            row1.CreateCell(0).SetCellValue("用户名");//0
            row1.CreateCell(1).SetCellValue("密码");//0
            row1.CreateCell(2).SetCellValue("姓名");//0
            row1.CreateCell(3).SetCellValue("专业");//0
            row1.CreateCell(4).SetCellValue("班级");//0
            row1.CreateCell(5).SetCellValue("安全测试分数");//0
            row1.CreateCell(6).SetCellValue("职业测试分数");//0
            row1.CreateCell(7).SetCellValue("实验目的内容");//0
            row1.CreateCell(8).SetCellValue("设备、仪器");//0
            row1.CreateCell(9).SetCellValue("材料");//0
            row1.CreateCell(10).SetCellValue("机床操作视频");//0
            row1.CreateCell(11).SetCellValue("人员分工");//0
            row1.CreateCell(12).SetCellValue("模型确定");//0
            row1.CreateCell(13).SetCellValue("加工工艺安排");//0
            row1.CreateCell(14).SetCellValue("凸/凹模虚拟加工");//0
            row1.CreateCell(15).SetCellValue("凸/凹模现实加工");//0
            row1.CreateCell(16).SetCellValue("电火花虚拟加工");//0
            row1.CreateCell(17).SetCellValue("电火花现实加工");//0
            row1.CreateCell(18).SetCellValue("尺寸");//0
            row1.CreateCell(19).SetCellValue("粗糙度");//0
            row1.CreateCell(20).SetCellValue("实验总结");//0
            int t = 1;
            for (int i = 0; i < len; i++)
            {
                Transform _tran = content.GetChild(i).transform;
                Text[] texts = _tran.GetComponentsInChildren<Text>();
                if (_tran.GetComponentInChildren<Toggle>().isOn)
                {
                    IRow trow = sheet1.CreateRow(t++);
                    for(int j = 0;j<texts.Length;j++)
                    {
                        trow.CreateCell(j).SetCellValue(texts[j].text);
                        trow.GetCell(j).CellStyle = style;
                    }
                }
            }
            for (int columnNum = 0; columnNum <= 20; columnNum++)
            {
                int columnWidth = sheet1.GetColumnWidth(columnNum) / 256;
                for (int rowNum = 1; rowNum <= sheet1.LastRowNum; rowNum++)
                {
                    IRow currentRow;
                    //当前行未被使用过
                    if (sheet1.GetRow(rowNum) == null)
                    {
                        currentRow = sheet1.CreateRow(rowNum);
                    }
                    else
                    {
                        currentRow = sheet1.GetRow(rowNum);
                    }

                    if (currentRow.GetCell(columnNum) != null)
                    {
                        ICell currentCell = currentRow.GetCell(columnNum);
                        int length = Encoding.Default.GetBytes(currentCell.ToString()).Length;
                        if (columnWidth < length)
                        {
                            columnWidth = length;
                        }
                    }
                }
                sheet1.SetColumnWidth(columnNum, columnWidth * 256);
            }
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                excelBook.Write(fs);
                Message.MessageBox("已完成");
            }
        }
        else if(index>10)
        {
            transimitProcess.SetActive(true);
            path += "\\" + rand + ".zip";
            int len = content.childCount;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                Transform _tran = content.GetChild(i).transform;
                if (_tran.GetComponentInChildren<Toggle>().isOn)
                    sb.AppendFormat("{0},", _tran.GetComponentInChildren<Text>().text);
            }
            if (sb.Length == 0)
            {
                Message.MessageBox("无有效文件");
                return;
            }
            Action<string> aciton = delegate (string text)
            {
                zipUtility.GetFilesByNames(text.Split(','), path);
            };
            StartCoroutine(web.Get(WebData.connectUri + "?" +Constant.propname.requesttype+"="+Constant.WebCommand.DOWNLOAD+ "&"+ Constant.propname.username + "=" + sb.ToString(),
                aciton));
            return;
        }
        else
        {
            path += "\\" + rand + ".zip";
            int len = content.childCount;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                Transform _tran = content.GetChild(i).transform;
                if (_tran.GetComponentInChildren<Toggle>().isOn)
                    sb.AppendFormat("{0},", _tran.GetComponentInChildren<Text>().text);
            }
            if(sb.Length==0)
            {
                Message.MessageBox("无有效文件");
                return;
            }
            Action<string> aciton = delegate (string text)
            {
                zipUtility.GetFilesByNames(text.Split(','), path);
            };
            StartCoroutine(web.Get(WebData.connectUri + "?" + Constant.propname.requesttype + "=" + Constant.WebCommand.DOWNLOAD + "&"+Constant.propname.username + "=" + sb.ToString() + "&" + Constant.propname.folderName + "=file" + index,
                aciton));
        }
       
    }
    public void UChange(Dropdown dp)
    {
        int index = dp.value;
        GameObject go = transimitProcess;
        //transimitProcess.SetActive(true);
        Slider slider = go.transform.GetChild(0).GetComponent<Slider>();
        if (index == 0)
        {
            transimitProcess.SetActive(true);
            UploadSubmit(slider, "实验指导书");
        }
        else if (index == 1)
        {
            transimitProcess.SetActive(true);
            UploadSubmit(slider, "实验报告模板");
        }
        else if(index == 2)
        {
            transimitProcess.SetActive(true);
            UploadSubmit(slider, "实验安全考试");
        }
        else if(index ==3)
        {
            transimitProcess.SetActive(true);
            UploadSubmit(slider, "职工素养测试");
        }
        else if (index == 4)
        {
            transimitProcess.SetActive(true);
            UploadSubmit(slider, "实验目的内容");
        }
        else if (index == 5)
        {
            transimitProcess.SetActive(true);
            UploadSubmit(slider, "m2");
        }
        else if (index == 6)
        {
            transimitProcess.SetActive(true);
            UploadSubmit(slider, "m3");
        }
        else if (index == 7)
        {
            //List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            //formData.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.DOWNCOMMON.ToString()));
            //formData.Add(new MultipartFormDataSection(Constant.propname.username, "videos"));
            string path = Application.streamingAssetsPath + "/" + "videos" + ".txt";
            UnityAction action = delegate ()
            {
                videos.text = File.ReadAllText(path);
            };

            Action<string> AddListenter_d = delegate (string res)
            {
                aoss.downpathInAli = res;
                aoss.AddListenter_d(path, action);

            };
            {
                StartCoroutine(
               web.Get(WebData.connectUri + "?" + Constant.propname.requesttype + "=" + Constant.WebCommand.DOWNCOMMON.ToString() + "&" + Constant.propname.username + "=" + "videos",
                AddListenter_d
                    ));

            };

            //StartCoroutine(web.getFileDown(WebData.connectUri, formData, slider, path,action));
            videos.transform.parent.gameObject.SetActive(true);
        }
    }

    public void SubmitVideo()
    {
        videos.transform.parent.gameObject.SetActive(false);
        transimitProcess.SetActive(true);
        GameObject go = transimitProcess;
        Slider slider = go.transform.GetChild(0).GetComponent<Slider>();
        //UploadSubmit(slider, "videos");
        string path = Application.streamingAssetsPath + "/" + "videos" + ".txt";
        FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine(videos.text);
        sw.Flush();
        sw.Close();
        fs.Close();
        //StartCoroutine(web.UpLoadFile(WebData.connectUri, path, slider, "videos", null, true));

        UploadSubmit(slider, "videos",path);
    }

    public void IChange(Dropdown dp)
    {
        int index = dp.value;
        if (index == 0)
        {
            hint.SetActive(true);
        }
        else if (index == 1)
        {
            personalPannel.SetActive(true);
        }

    }

    public void OChange(Dropdown dp)
    {
        int index = dp.value;
        if (index == 0)//重新载入
        {
            Reload();
        }
        else if (index == 1)
        {
            InsertSingleItem.SetActive(true);
        }
        else if(index == 2)
        {
            string path = WebData.OpenDialog();
            if (path == null)
            {
                Message.MessageBox("操作已取消");
                return;
            }
            Action<string> action = delegate (string s)
            {
                Message.MessageBox("插入成功" + s +"条");
                Reload();
            };
            StartCoroutine(web.AddUsersByExcel(WebData. connectUri, path,action));
        }
        else if(index==3)//删除选中
        {
            int len = content.childCount;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                Transform _tran = content.GetChild(i).transform;
                if (_tran.GetComponentInChildren<Toggle>().isOn)
                    sb.AppendFormat("{0},", _tran.GetComponentInChildren<Text>().text);

            }
            Action<string> action = delegate (string s)
            {
                Message.MessageBox("删除了" + s + "条记录");
                Reload();
            };
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection( Constant.propname.requesttype, Constant.WebCommand.DELETEUSERS.ToString()));
            formData.Add(new MultipartFormDataSection( Constant.propname.username, sb.ToString()));
            StartCoroutine(web.ExcuteSend(WebData.connectUri, formData, action));
        }
        else if(index==4)//全选
        {
            int len = content.childCount;
            for (int i = 0; i < len; i++)
                content.GetChild(i).GetComponentInChildren<Toggle>().isOn = true;
        }
        else if(index==5)//全不选
        {
            int len = content.childCount;
            for (int i = 0; i < len; i++)
                content.GetChild(i).GetComponentInChildren<Toggle>().isOn = false;
        }
    }

    public void Quanxuan()
    {
        int len = content.childCount;

        if (toggle.isOn)
        {
            for (int i = 0; i < len; i++)
                content.GetChild(i).GetComponentInChildren<Toggle>().isOn = true;
        }
        else
        {
            for (int i = 0; i < len; i++)
                content.GetChild(i).GetComponentInChildren<Toggle>().isOn = false;
        }
    }

    public void majorChanged()
    {
        string _key = Major.captionText.text;
        Theclass.ClearOptions();
        List<string> tl = new List<string>();
        tl.Add("所有");
        if(_key!="所有")
        {
            HashSet<string> hs = majorToClass[_key];
            foreach(string s in hs)
            {
                tl.Add(s);
            }
        }
        Theclass.AddOptions(tl);
    }
    public void check1()
    {
        string _key = UsernameInput.text;
        int len = content.childCount;
        for (int i = 0; i < len; i++)
            DestroyImmediate(content.GetChild(0).gameObject);
        len = users.Count;
        for (int i = 0; i < len; i++)
        {
            if(_key== users[i].info["username"])
            {
                Transform _item = Instantiate(Item, content).GetComponent<Transform>();
                for (int j = 1; j <= 21; j++)
                    _item.GetChild(j).GetComponentInChildren<Text>().text = users[i].info[pns[j - 1]];
                break;
            }
        }
    }
    public void check2()
    {
        string _major = Major.captionText.text;
        string _class = Theclass.captionText.text;
        int len = content.childCount;
        for (int i = 0; i < len; i++)
            DestroyImmediate(content.GetChild(0).gameObject);
        len = users.Count;
        if(_major=="所有")
        {
            for (int i = 0; i < len; i++)
            {
                Transform _item = Instantiate(Item, content).GetComponent<Transform>();
                for (int j = 1; j <= 21; j++)
                    _item.GetChild(j).GetComponentInChildren<Text>().text = users[i].info[pns[j - 1]];
            }
            return;
        }
        else if(_class=="所有")
        {
            for (int i = 0; i < len; i++)
            {
                if (users[i].info["major"] != _major)
                    continue;
                Transform _item = Instantiate(Item, content).GetComponent<Transform>();
                for (int j = 1; j <= 21; j++)
                    _item.GetChild(j).GetComponentInChildren<Text>().text = users[i].info[pns[j - 1]];
            }
            return;
        }
        else
        {
            for (int i = 0; i < len; i++)
            {
                if (users[i].info["major"] != _major|| users[i].info["class"] != _class)
                    continue;
                Transform _item = Instantiate(Item, content).GetComponent<Transform>();
                for (int j = 1; j <= 21; j++)
                    _item.GetChild(j).GetComponentInChildren<Text>().text = users[i].info[pns[j - 1]];
            }
        }
    }
    public void check3()
    {
        bool cs = CompeleteState.value==0?false:true;
        int ind = Column.value;
        int len = content.childCount;
        for (int i = 0; i < len; i++)
            DestroyImmediate(content.GetChild(0).gameObject);
        len = users.Count;
        if(ind==0)
        {
            int score1, score2;
            bool flag = false;
            string _key;
            for (int i = 0; i < len; i++)
            {
                flag = false;
                for(int j = 1;j<=10;j++)
                {
                    _key =  "file" + j + "date";
                    if (users[i].info[_key] == "未提交" ^ cs)
                        flag  = true;
                }
                score1 = score2 = 0;
                int.TryParse(users[i].info["score1"], out score1);
                int.TryParse(users[i].info["score2"], out score2);
                if ((score1 < 90 || score2 < 90) ^ cs) flag = true;
                if (flag) continue;
                Transform _item = Instantiate(Item, content).GetComponent<Transform>();
                for (int j = 1; j <= 21; j++)
                    _item.GetChild(j).GetComponentInChildren<Text>().text = users[i].info[pns[j - 1]];
            }
        }
        else if(ind==1)//考试通过
        {
            int score1,score2;
            for (int i = 0; i < len; i++)
            {
                score1 = score2 = 0;
                int.TryParse(users[i].info["score1"],out score1);
                int.TryParse(users[i].info["score2"],out score2);
                if ((score1 < 90 || score2 < 90)^cs) continue;
                Transform _item = Instantiate(Item, content).GetComponent<Transform>();
                for (int j = 1; j <= 21; j++)
                    _item.GetChild(j).GetComponentInChildren<Text>().text = users[i].info[pns[j - 1]];
            }
        }
        else if(ind==2)
        {
            int time;
            for(int i = 0;i<len;i++)
            {
                time = 0;
                int.TryParse(users[i].info["completeTime"], out time);
                if (time == 0) continue;
                Transform _item = Instantiate(Item, content).GetComponent<Transform>();
                for (int j = 1; j <= 21; j++)
                    _item.GetChild(j).GetComponentInChildren<Text>().text = users[i].info[pns[j - 1]];

            }
        }
        else if(ind<6)
        {
            int time;
            for(int i = 0;i<len;i++)
            {
                time = 0;
                int.TryParse(users[i].info["readTime"+(ind-2)], out time);
                if (time == 0) continue;
                Transform _item = Instantiate(Item, content).GetComponent<Transform>();
                for (int j = 1; j <= 21; j++)
                    _item.GetChild(j).GetComponentInChildren<Text>().text = users[i].info[pns[j - 1]];
            }
        }
        else
        {
            string _key = "file" + (ind-5) + "date";
            for(int i = 0;i<len;i++)
            {
                if (users[i].info[_key] == "未提交" ^ cs) continue;
                Transform _item = Instantiate(Item, content).GetComponent<Transform>();
                for (int j = 1; j <= 21; j++)
                    _item.GetChild(j).GetComponentInChildren<Text>().text = users[i].info[pns[j - 1]];
            }
        }

    }
    public void ConfirmInsert()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        string[] data = new string[5];
        data[0] = insertIn[0].text;
        if(data[0]=="")
        {
            Message.MessageBox("用户名不能为空");
            return;
        }
        data[1] = insertIn[1].text!=""?insertIn[1].text:data[0];
        for (int i = 2; i < 5; i++)
            data[i] = insertIn[i].text != "" ? insertIn[i].text : "暂无";
        formData.Add(new MultipartFormDataSection(Constant.propname.requesttype,Constant.WebCommand.INSERTSINGLEUSER.ToString()));
        formData.Add(new MultipartFormDataSection(Constant.propname.username,data[0]));
        formData.Add(new MultipartFormDataSection(Constant.propname.password, data[1]));
        formData.Add(new MultipartFormDataSection(Constant.propname.name, data[2]));
        formData.Add(new MultipartFormDataSection(Constant.propname.major, data[3]));
        formData.Add(new MultipartFormDataSection(Constant.propname.theClass, data[4]));
        Action<string> action = delegate (string s)
        {
            if(s.Length>0&& s[0]=='T')
            {
                Message.MessageBox("成功");
                Reload();
            }
            else
            {
                Message.MessageBox("失败");
            }
        };
        StartCoroutine(web.ExcuteSend(WebData.connectUri, formData,action));
    }
    public void personalClick()
    {
        Text []texts = personalInfoPannel.GetComponentsInChildren<Text>();
        InputField[] inputs = personalInfoPannel.GetComponentsInChildren<InputField>();
        string[] data = new string[5];
        data[0] = texts[0].text.Trim();
        for(int i = 0;i<4;i++)
        {
            data[i+1] = inputs[i].text;
            if(data[i+1].Trim()=="")
                data[i+1] = texts[i+1].text;
            print(data[i+1]);
        }
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.UPDATE.ToString()));
        formData.Add(new MultipartFormDataSection(Constant.propname.username, texts[0].text));
        formData.Add(new MultipartFormDataSection(Constant.propname.password, data[1]));
        formData.Add(new MultipartFormDataSection(Constant.propname.name, data[2]));
        formData.Add(new MultipartFormDataSection(Constant.propname.major, data[3]));
        formData.Add(new MultipartFormDataSection(Constant.propname.theClass, data[4]));
        Action<string> action = delegate (string s)
        {
                Message.MessageBox("成功");
        };
        StartCoroutine(web.ExcuteSend(WebData.connectUri, formData, action));
    }
    public void SChange(Dropdown dp)
    {
        int index = dp.value;
        if (index == 0)
        {
            Screen.SetResolution(900, 600,false);
            SceneManager.LoadScene("登录");
        }
        else if (index == 1)
        {
            Application.Quit();
        }
    }

    #region 辅助方法
    void Reload()
    {
        users.Clear();
        Action<string> action = delegate (string s)
        {
            print(s);
            xmlDocument.LoadXml(s);
            XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("Table");
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlNodeList xnl = xmlNodeList.Item(i).ChildNodes;
                if (xnl.Item(3).InnerText[0] != 's') continue;
                User user = new User();
                for (int j = 1; j < xnl.Count; j++)
                {
                    if (j == 3) continue;
                    string _key = xnl.Item(j).Name;
                    if (!user.info.ContainsKey(_key)) continue;
                    string val = xnl.Item(j).InnerText.Trim();
                    user.info[_key] = val;
                }
                users.Add(user);
            }
            for (int i = 0; i < users.Count; i++)
            {
                string _key = users[i].info["major"];
                if (majorToClass.ContainsKey(_key))
                    majorToClass[_key].Add(users[i].info["class"]);
                else
                {
                    majorToClass.Add(_key, new HashSet<string>());
                    majorToClass[_key].Add(users[i].info["class"]);
                }
                //majorToClass[users[i].info["major"]].Add(users[i].info["class"]);
            }
            Major.options.Clear();
            Theclass.options.Clear();
            List<string> tl = new List<string>();
            tl.Add("所有");
            Theclass.AddOptions(tl);
            tl.AddRange(majorToClass.Keys);
            Major.AddOptions(tl);
            tl.Clear();
            Show();
        };
        StartCoroutine(web.GetAllUserInfo(WebData.connectUri, action));
    }

    void Show()
    {
        int len = content.childCount;
        for (int i = 0; i < len; i++)
            DestroyImmediate(content.GetChild(0).gameObject);
        len = users.Count;
        for(int i = 0;i<len;i++)
        {
            Transform _item = Instantiate(Item,content).GetComponent<Transform>();
            for(int j = 1;j<=21;j++)
                _item.GetChild(j).GetComponentInChildren<Text>().text = users[i].info[pns[j-1]];
        }
    }
    void UploadSubmit(Slider slider, string username,string path = null)
    {
        if(path!=null)
        aoss.upInterface(null, WebData.connectUri, username,path);
        else
        aoss.upInterface(null, WebData.connectUri, username);
        //string path = WebData.OpenDialog();
        //if (path == null)
        //{
        //    Message.MessageBox("操作已取消");
        //    return;
        //}
        //StartCoroutine(web.UpLoadFile(WebData.connectUri, path,slider,username,null,true));
    }
    #endregion
}
