using Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FlowPath : MonoBehaviour
{
    public Transform btParent;
    public Button[] buttons;
    public Button startBtn;
    public Text description,usernameText,comText,theOtherText;
    public Slider slider;
    public Image comRate;
    //public Text[] Dates,texts;//0-9 0-10
    public GameObject examPanel, examPanel2;
    public Paroxe.PdfRenderer.PDFViewer pdf;
    StartExam startExam1, startExam2;
    // 0-17个按钮是功能节点。全部为0-23个按钮

    public bool[] recordTask;
    public bool[] needSlider;
    Color32 gray = new Color32(190,190,190,255);
    Color32 green = new Color32(197,255,119,255);
    Color32 blue = new Color32(112,235,255,255);
    Color32 yellow = new Color32(255,249,114,255);

    Dictionary<string, string> infoDic;
    public Web web;
    List<string> keys;
    float startTime = 0.0f;
    int pdfCode = 0;
    void Start()
    {
        infoDic = new Dictionary<string, string>();
        infoDic.Add("file1date", "未提交"); //0
        infoDic.Add("file2date", "未提交");//1
        infoDic.Add("file3date", "未提交");//2
        infoDic.Add("file4date", "未提交");//03
        infoDic.Add("file5date", "未提交");//04
        infoDic.Add("file6date", "未提交");//05
        infoDic.Add("file7date", "未提交");//06
        infoDic.Add("file8date", "未提交");//07
        infoDic.Add("file9date", "未提交");//08
        infoDic.Add("file10date", "未提交");//09
        infoDic.Add("password", "暂无");//10
        infoDic.Add("name", "暂无");//11
        infoDic.Add("major", "暂无");//12
        infoDic.Add("class", "暂无");//13
        infoDic.Add("score1", "暂无");//14
        infoDic.Add("score2", "暂无");//15
        infoDic.Add("attemptTimes", "未完成");//16
        infoDic.Add("completeTime", "未完成");//17
        infoDic.Add("readTime1", "未完成");//18
        infoDic.Add("readTime2", "未完成");//19
        infoDic.Add("readTime3", "未完成");//20
        //WebData.username = "1";//记得删
        //WebData.connectUri = "http://localhost:33401/api/values";//记得删
        
        startExam1 = examPanel.GetComponent<StartExam>();
        startExam2 = examPanel2.GetComponent<StartExam>();
        startExam1.excelFilePath = Application.streamingAssetsPath + "/Test1.xls";
        startExam2.excelFilePath = Application.streamingAssetsPath + "/Test2.xls";
        usernameText.text = WebData.username;
        buttons = btParent.GetComponentsInChildren<Button>();
        recordTask = new bool[24];
        for (int i = 0; i < recordTask.Length; i++)
            recordTask[i] = false;
        
        buttons[0].interactable = buttons[1].interactable = buttons[2].interactable = buttons[3].interactable = true;
        RefreshSchedule();
    }
    private void OnEnable()
    {
        RefreshSchedule();
    }
    public void ClosePDF()
    {
        float t = Time.time - startTime;
        t = Mathf.Round(t);
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.SUBMITTIME.ToString()));
        formData.Add(new MultipartFormDataSection(Constant.propname.username, WebData.username));
        formData.Add(new MultipartFormDataSection(Constant.propname.propName, t.ToString()));
        formData.Add(new MultipartFormDataSection(Constant.propname.fileInDb, pdfCode.ToString()));
        StartCoroutine(web.ExcuteSend(WebData.connectUri,formData,RefreshSchedule));
    }
    public void b0()
    {
        description.text = "任务名称：实验目的、内容\n任务内容：阅读PDF\n\n任务完成要求：阅读完成相应PDF\n当前状态：阅读总时间："+infoDic["readTime1"] +"s\n";
        slider.gameObject.SetActive(false);
        //Button.ButtonClickedEvent buttonClickedEvent = b1;
        UnityAction action = delegate ()
        {
            string url = WebData.connectUri;
            int ind = url.LastIndexOf(':');
            while (url[ind] != '/') ind++;
            url = url.Substring(0, ind);
            url = url + "/CommonFile/m1.pdf";
            pdf.FileURL = url;
            pdf.gameObject.SetActive(true);
            startTime = Time.time;
            pdfCode = 1;
            RefreshSchedule();
        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    public void b1()
    {
        description.text = "任务名称：设备、仪器\n任务内容：阅读PDF\n\n任务完成要求：阅读完成相应PDF\n当前状态：阅读总时间：" + infoDic["readTime2"] + "s\n";
        slider.gameObject.SetActive(false);
        UnityAction action = delegate ()
        {
            string url = WebData.connectUri;
            int ind = url.LastIndexOf(':');
            while (url[ind] != '/') ind++;
            url = url.Substring(0, ind);
            url = url + "/CommonFile/m2.pdf";
            print(url);
            pdf.FileURL = url;
            pdf.gameObject.SetActive(true);
            startTime = Time.time;
            pdfCode = 2;
            RefreshSchedule();
        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    public void b2()
    {
        description.text = "任务名称：材料\n任务内容：阅读PDF\n\n任务完成要求：阅读完成相应PDF\n当前状态：阅读总时间：" + infoDic["readTime3"] + "s\n";
        slider.gameObject.SetActive(false);
        UnityAction action = delegate ()
        {
            string url = WebData.connectUri;
            int ind = url.LastIndexOf(':');
            while (url[ind] != '/') ind++;
            url = url.Substring(0, ind);
            url = url + "/CommonFile/m3.pdf";
            pdf.FileURL = url;
            pdf.gameObject.SetActive(true);
            startTime = Time.time;
            pdfCode = 3;
            RefreshSchedule();
        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    public void b3()//子物体要有Text 注意slider要改名
    {
        string ans = infoDic["file1date"].Contains("+") ? infoDic["file1date"].Substring(0, infoDic["file1date"].LastIndexOf('+')) : infoDic["file1date"];
        description.text = "任务名称：机床操作视频\n任务内容：上传机床操作视频\n\n任务完成要求：上传完成机床操作视频\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "1";
        UnityAction action = delegate ()
        {
            UploadSubmit(slider, "机床操作视频");
            RefreshSchedule();
        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    public void b4()
    {
        description.text = "任务名称：安全操作考试\n任务内容：进行安全操作考试\n\n任务完成要求：安全操作考试达到90分以上\n当前状态：" + infoDic["score1"] + "\n";
        slider.gameObject.SetActive(false);
        UnityAction action = delegate ()
        {
            if (WebData.username == "")
            {
                //Message.MessageBox(IntPtr.Zero, "请登录", "注意", 0);
                System.Windows.Forms.MessageBox.Show("请登录");
                return;
            }
            string path = Application.streamingAssetsPath + "/" + "Test1" + ".xls";
            if (!File.Exists(path))
            {
                OnSure act = delegate ()
                {
                    ReloadTest();
                };
                Message.MessageBox("您还未获取题库，是否重新获取？", 1, act);
                return;
            }
            examPanel.SetActive(true);

            RefreshSchedule();
        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    public void b5()
    {
        description.text = "任务名称：职业工程素养测试\n任务内容：进行职业工程素养测试\n\n任务完成要求：职业工程素养测试达到90分以上\n当前状态：" + infoDic["score2"] + "\n";
        slider.gameObject.SetActive(false);
        UnityAction action = delegate ()
        {
            if (WebData.username == "")
            {
                Message.MessageBox("请登录？");
                //Message.MessageBox(IntPtr.Zero, "请登录", "注意", 0);
                return;
            }
            string path1 = Application.streamingAssetsPath + "/" + "Test2" + ".xls";
            if (!File.Exists(path1))
            {
                OnSure os =delegate() { ReloadTest(); };
                Message.MessageBox("您还未获取题库，是否重新获取？", 1,os);
                
                return;
            }
            examPanel2.SetActive(true);
            RefreshSchedule();

        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    public void b6()
    {
        string ans = infoDic["file2date"].Contains("+") ? infoDic["file2date"].Substring(0, infoDic["file2date"].LastIndexOf('+')):infoDic["file2date"];
        description.text = "任务名称：人员分工\n任务内容：上传人员分工\n\n任务完成要求：上传完成人员分工\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "2";
        UnityAction action = delegate ()
        {
            UploadSubmit(slider, "人员分工");
            RefreshSchedule();
        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    public void b7()
    {
        string ans = infoDic["file3date"].Contains("+") ? infoDic["file3date"].Substring(0, infoDic["file3date"].LastIndexOf('+')) : infoDic["file3date"];
        description.text = "任务名称：模型分配\n任务内容：上传模型分配\n\n任务完成要求：上传完成模型分配\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "3";
        UnityAction action = delegate ()
        {
            slider.gameObject.SetActive(true);
            UploadSubmit(slider, "模型分配");
            RefreshSchedule();
        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    public void b8()
    {
        string ans = infoDic["file4date"].Contains("+") ? infoDic["file4date"].Substring(0, infoDic["file4date"].LastIndexOf('+')) : infoDic["file4date"];
        description.text = description.text = "任务名称：加工工艺安排\n任务内容：上传加工工艺安排\n\n任务完成要求：上传完成加工工艺安排\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "4";

        UnityAction action = delegate ()
        {
            UploadSubmit(slider, "加工工艺安排");
            RefreshSchedule();

        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    public void b9()
    {
        string ans = infoDic["file5date"].Contains("+") ? infoDic["file5date"].Substring(0, infoDic["file5date"].LastIndexOf('+')) : infoDic["file5date"]; 
        description.text = description.text = "任务名称：凸凹模虚拟加工\n任务内容：上传凸凹模虚拟加工\n\n任务完成要求：上传完成凸凹模虚拟加工\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "5";

        UnityAction action = delegate ()
        {
            UploadSubmit(slider, "凸凹模虚拟加工");
            RefreshSchedule();
        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    public void b10()
    {
        string ans = infoDic["file6date"].Contains("+") ? infoDic["file6date"].Substring(0, infoDic["file6date"].LastIndexOf('+')) : infoDic["file6date"];
        description.text = description.text = "任务名称：凸凹模现实加工\n任务内容：上传凸凹模现实加工\n\n任务完成要求：上传完成凸凹模现实加工\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "6";
        UnityAction action = delegate ()
        {
            UploadSubmit(slider, "凸凹模现实加工");
            RefreshSchedule();

        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    public void b11()
    {
        description.text = description.text = "任务名称：电火花放电虚拟加工\n任务内容：在本软件中进行电火花放电虚拟加工\n任务完成要求：完成电火花放电虚拟加工实验总结\n当前状态：最短完成时间" + infoDic["completeTime"] + "\n";
        slider.gameObject.SetActive(false);

        UnityAction action = delegate ()
        {
            SceneManager.LoadScene("SampleScene");
            
        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    public void b12()
    {
        string ans = infoDic["file7date"].Contains("+") ? infoDic["file7date"].Substring(0, infoDic["file7date"].LastIndexOf('+')) : infoDic["file7date"];
        description.text = "任务名称：电火花放电现实加工\n任务内容：上传电火花放电现实加工\n任务完成要求：上传完成电火花放电现实加工\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "7";

        UnityAction action = delegate ()
        {
            UploadSubmit(slider, "电火花放电现实加工");
            RefreshSchedule();

        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    public void b13()
    {
        string ans = infoDic["file8date"].Contains("+") ? infoDic["file8date"].Substring(0, infoDic["file8date"].LastIndexOf('+')) : infoDic["file8date"];
        description.text = description.text = "任务名称：尺寸\n任务内容：上传尺寸\n\n任务完成要求：上传完成尺寸\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "8";
        UnityAction action = delegate ()
        {
            UploadSubmit(slider, "尺寸");
            RefreshSchedule();

        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    public void b14()
    {
        string ans = infoDic["file9date"].Contains("+") ? infoDic["file9date"].Substring(0, infoDic["file9date"].LastIndexOf('+')) : infoDic["file9date"];
        description.text = description.text = "任务名称：粗糙度\n任务内容：上传粗糙度\n\n任务完成要求：上传完成粗糙度\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "9";

        UnityAction action = delegate ()
        {
            UploadSubmit(slider, "粗糙度");
            RefreshSchedule();

        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    public void b15()
    {
        string ans = infoDic["file10date"].Contains("+") ? infoDic["file10date"].Substring(0, infoDic["file10date"].LastIndexOf('+')) : infoDic["file10date"];
        description.text = description.text = "任务名称：实验总结\n任务内容：上传实验总结\n\n任务完成要求：上传完成实验总结\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "10";

        UnityAction action = delegate ()
        {

            UploadSubmit(slider, "实验总结");
            RefreshSchedule();
        };
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
    }
    #region 帮助函数
    void UploadSubmit(Slider slider, string floderName)
    {
        string path = WebData.OpenDialog();
        if (path == null)
        {
            Assets.Message.MessageBox("操作已取消");
            return;
        }
        if (WebData.username == "")
        {
            Assets.Message.MessageBox("请登录");
            return;
        }
        StartCoroutine(web.UpLoadFile(WebData.connectUri, path, floderName, slider, RefreshSchedule));
    }

    void GetUploadTime()
    {
        Action<string>[] actions = new Action<string>[10];
        for(int i = 0;i<10;i++)
        {
            //actions[i] = delegate (string str)
            //{
            //    //Dates[i].text = str;
            //};
            StartCoroutine(web.GetInfo(WebData.connectUri, WebData.username, Constant.propname.file1date, actions[i]));
        }
    }
    public void ReloadTest()
    {
        Action action = delegate ()
        {
            //Message.MessageBox(IntPtr.Zero, "成功获取题库", "提示", 0);
            System.Windows.Forms.MessageBox.Show("成功获取题库");
        };
        {
            string path1 = Application.streamingAssetsPath + "\\" + "Test2" + ".xls";
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.DOWNCOMMON.ToString()));
            formData.Add(new MultipartFormDataSection(Constant.propname.username, "职工素养测试"));
            StartCoroutine(web.getFileDown(WebData.connectUri, formData, null, path1));
        }
        {
            string path = Application.streamingAssetsPath + "\\" + "Test1" + ".xls";
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.DOWNCOMMON.ToString()));
            formData.Add(new MultipartFormDataSection(Constant.propname.username, "实验安全考试"));
            StartCoroutine(web.getFileDown(WebData.connectUri, formData, null, path, action));
        }
    }
    public void SwitchOpen(GameObject go)
    {
        go.SetActive(!go.activeInHierarchy);
    }
    public void closePdf()
    {
        float interval = Time.time - startTime;
        Action<String> action = delegate (string s)
        {
            RefreshSchedule();
        };
        StartCoroutine(web.GetInfo(WebData.connectUri, WebData.username, "readTime"+pdfCode, action));
        pdf.gameObject.SetActive(false);
    }
    public void RefreshSchedule()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.GEIINFO.ToString()));
        formData.Add(new MultipartFormDataSection(Constant.propname.username, WebData.username));
        Action<string> action = delegate (string str)
        {
            print(str);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(str);
            XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("Table");
            XmlNodeList xnl = xmlNodeList.Item(0).ChildNodes;
            for (int j = 1; j < xnl.Count; j++)
            {
                string _key = xnl.Item(j).Name;
                if (!infoDic.ContainsKey(_key)) continue;
                string val = xnl.Item(j).InnerText.Trim();
                infoDic[_key] = val;
            }
            keys = new List<string>(infoDic.Keys);
            foreach(Button b in buttons)
            {
                b.transform.GetComponent<Image>().color = gray;
                b.interactable = false;
            }
            buttons[0].transform.GetComponent<Image>().color = yellow;
            buttons[0].interactable = true;
            buttons[1].transform.GetComponent<Image>().color = yellow;
            buttons[1].interactable = true;
            buttons[2].transform.GetComponent<Image>().color = yellow;
            buttons[2].interactable = true;
            buttons[3].transform.GetComponent<Image>().color = green;
            buttons[3].interactable = true;
            buttons[16].transform.GetComponent<Image>().color = yellow;
            buttons[16].interactable = true;
            recordTask[0] = true;
            if (!infoDic["readTime2"].Contains("未完成")) recordTask[1] = true;
            else recordTask[1] = false;
            if (!infoDic["readTime3"].Contains("未完成")) recordTask[2] = true;
            else recordTask[2] = false;
             recordTask[3] = true;
            if (!infoDic["score1"].Contains("暂无")&&int.Parse(infoDic["score1"])>=90) recordTask[4] = true;
            if (!infoDic["score2"].Contains("暂无")&&int.Parse(infoDic["score2"]) >= 90) recordTask[5] = true;
            //2 3 4 5 6
            for(int i = 1;i<6;i++)
            {
                if (!infoDic[keys[i]].Contains("未提交") && !infoDic[keys[i]].Contains("暂无"))
                {
                    recordTask[i + 5] = true;
                }
            }
            if (!infoDic["completeTime"].Contains("未完成")) recordTask[11] = true;
            for (int i = 0; i < 4; i++)
            {
                if (!infoDic[keys[i+6]].Contains("未提交") && !infoDic[keys[i+6]].Contains("暂无"))
                    recordTask[i + 12] = true;
            }
            if (recordTask[0] && recordTask[1] && recordTask[2] && recordTask[3])
                recordTask[16] = true;
            else if(recordTask[0] || recordTask[1] || recordTask[2] || recordTask[3])
            {
                buttons[16].transform.GetComponent<Image>().color = blue;
                recordTask[16] = false;
            }
            if(recordTask[16])
            {
                buttons[4].transform.GetComponent<Image>().color = yellow;
                buttons[4].interactable = true;
                buttons[5].transform.GetComponent<Image>().color = yellow;
                buttons[5].interactable = true;
                buttons[17].transform.GetComponent<Image>().color = yellow;
                buttons[17].interactable = true;
                if(!infoDic["score1"].Contains("无")&& int.Parse(infoDic["score1"].Trim())>0) buttons[4].transform.GetComponent<Image>().color = blue;
                if(!infoDic["score2"].Contains("无") && int.Parse(infoDic["score2"].Trim()) > 0) buttons[5].transform.GetComponent<Image>().color = blue;
                if (recordTask[4] && recordTask[5])
                    recordTask[17] = true;
                else if (!infoDic["score1"].Contains("无") && int.Parse(infoDic["score1"].Trim()) > 0 || !infoDic["score2"].Contains("无") && int.Parse(infoDic["score2"].Trim()) > 0)
                    buttons[17].transform.GetComponent<Image>().color = blue;
            }
            if(recordTask[17])
            {
                buttons[6].transform.GetComponent<Image>().color = yellow;
                buttons[6].interactable = true;
                buttons[7].transform.GetComponent<Image>().color = yellow;
                buttons[7].interactable = true;
                buttons[8].transform.GetComponent<Image>().color = yellow;
                buttons[8].interactable = true;
                buttons[9].transform.GetComponent<Image>().color = yellow;
                buttons[9].interactable = true;
                buttons[11].transform.GetComponent<Image>().color = yellow;
                buttons[11].interactable = true;
                if(recordTask[9])
                {
                    buttons[10].transform.GetComponent<Image>().color = yellow;
                    buttons[10].interactable = true;
                }
                if(recordTask[11])
                {
                    buttons[12].transform.GetComponent<Image>().color = yellow;
                    buttons[12].interactable = true;
                }
                buttons[13].transform.GetComponent<Image>().color = yellow;
                buttons[13].interactable = true;
                buttons[14].transform.GetComponent<Image>().color = yellow;
                buttons[14].interactable = true;
                if(recordTask[13]&&recordTask[14])
                {
                    buttons[15].transform.GetComponent<Image>().color = yellow;
                    buttons[15].interactable = true;
                }

                for (int i = 18;i<24;i++)
                {
                    buttons[i].transform.GetComponent<Image>().color = yellow;
                    buttons[i].interactable = true;
                }
            }

            if (recordTask[6] && recordTask[7] && recordTask[8])
                recordTask[19] = true;
            else if(recordTask[6]|| recordTask[7]|| recordTask[8])
                buttons[19].transform.GetComponent<Image>().color = blue;
            if(recordTask[10])
                recordTask[22] = true;
            else if(recordTask[9])
                buttons[22].transform.GetComponent<Image>().color = blue;
            if(recordTask[12])
                recordTask[23] = true;
            else if(recordTask[11])
                buttons[23].transform.GetComponent<Image>().color = blue;
            if (recordTask[23] && recordTask[22])
                recordTask[20] = true;
            else if (recordTask[23] || recordTask[22])
                buttons[20].transform.GetComponent<Image>().color = blue;
            if (recordTask[15])
                recordTask[21] = true;
            else if (recordTask[13] || recordTask[14])
                buttons[21].transform.GetComponent<Image>().color = blue;
            if (recordTask[19] && recordTask[20] && recordTask[21])
                recordTask[18] = true;
            else if (recordTask[19] || recordTask[20] || recordTask[21] || buttons[21].transform.GetComponent<Image>().color == blue || buttons[20].transform.GetComponent<Image>().color == blue || buttons[19].transform.GetComponent<Image>().color == blue)
                buttons[18].transform.GetComponent<Image>().color = blue;
            int ans = 0;
            for (int i = 0; i < 24; i++)
            {
                if (recordTask[i])
                {
                    buttons[i].transform.GetComponent<Image>().color = green;
                    buttons[i].interactable = true;
                    ++ans;
                }
            }
            comText.text = ans.ToString();
            theOtherText.text = (24 - ans).ToString();
            comRate.fillAmount = ans * 1.0f / 24;
            comRate.GetComponentInChildren<Text>(true).text = ans * 1.0f / 24*100 + "%";
        };
        StartCoroutine(web.ExcuteSend(WebData.connectUri, formData,action));
    }

    
    #endregion
}
