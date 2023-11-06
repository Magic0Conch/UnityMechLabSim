using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Application = UnityEngine.Application;
using Button = UnityEngine.UI.Button;


public class FlowPath1 : MonoBehaviour
{
    //public Transform btParent;
    public AliyunOSSWithProcess aoss;
    public Button[] buttons;
    public GameObject liucheng;
    public Text description,usernameText;
    public Slider slider;
    public Color finish;
    public Button startBtn;
    public Button freshProblemBtn;
    //public Image comRate;
    //public Text[] Dates,texts;//0-9 0-10
    public GameObject examPanel, examPanel2;
    public Paroxe.PdfRenderer.PDFViewer pdf;
    public InputField videosInput;
    StartExam startExam1, startExam2;
    // 0-17个按钮是功能节点。全部为0-23个按钮

    public bool[] recordTask;
    public bool[] needSlider;

    Dictionary<string, string> infoDic;
    Web web;
    List<string> keys;
    float startTime = 0.0f;
    int pdfCode = 0;
    int nb = -1;
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
        web = GetComponent<Web>();
        startExam1 = examPanel.GetComponent<StartExam>();
        startExam2 = examPanel2.GetComponent<StartExam>();
        startExam1.excelFilePath = UnityEngine.Application.streamingAssetsPath + "/Test1.xls";
        startExam2.excelFilePath = UnityEngine.Application.streamingAssetsPath + "/Test2.xls";
        usernameText.text = WebData.username;
        //if(btParent!=null)
        //buttons = btParent.GetComponentsInChildren<Button>();
        recordTask = new bool[24];
        for (int i = 0; i < recordTask.Length; i++)
            recordTask[i] = false;
        
        buttons[0].interactable = buttons[1].interactable = buttons[2].interactable = buttons[3].interactable = true;
        Model1();
        RefreshSchedule();
    }
    public void Sheji()
    {
        videosInput.gameObject.SetActive(false);
        Model3();
        buttons[6].gameObject.SetActive(true);
        buttons[7].gameObject.SetActive(true);
        buttons[8].gameObject.SetActive(true);

        liucheng.SetActive(false);
    }
    public void JiaGong()
    {
        videosInput.gameObject.SetActive(false);
        Model3();
        buttons[9].gameObject.SetActive(true);
        buttons[10].gameObject.SetActive(true);
        buttons[11].gameObject.SetActive(true);
        buttons[12].gameObject.SetActive(true);

        liucheng.SetActive(false);
    }
    public void JianYan()
    {
        videosInput.gameObject.SetActive(false);
        Model3();
        buttons[13].gameObject.SetActive(true);
        buttons[14].gameObject.SetActive(true);
        liucheng.SetActive(false);
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
        videosInput.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(true);
        nb = 0;
        description.text = "任务名称：实验目的、内容\n任务内容：下载压缩包\n\n";
        slider.gameObject.SetActive(true);
        freshProblemBtn.gameObject.SetActive(false);
        startBtn.GetComponentInChildren<Text>().text = "下载";
        //Button.ButtonClickedEvent buttonClickedEvent = b1;


        UnityAction action = delegate ()
        {
            string path = WebData.SaveProject();
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.DOWNCOMMON.ToString()));
            path += "\\" + "实验目的内容" + ".zip";
            formData.Add(new MultipartFormDataSection(Constant.propname.username, "实验目的内容"));
            StartCoroutine(web.getFileDownAdType(WebData.connectUri, formData, slider, path));


            RefreshSchedule();
        };
        startBtn.onClick.RemoveAllListeners();
        //startBtn.onClick.AddListener(action);
        Action<string> AddListenter_d = delegate (string res)
        {
            aoss.downpathInAli = res;
            // 绑定按钮事件

            aoss.AddListenter_d();

            RefreshSchedule();
        };

        UnityAction action2 = delegate ()
        {
            StartCoroutine(
           web.Get(WebData.connectUri+"?"+ Constant.propname.requesttype+"="+ Constant.WebCommand.DOWNCOMMON.ToString()+"&"+ Constant.propname.username+"="+ "实验目的内容",
            AddListenter_d
                ));                        
           
        };
        startBtn.onClick.AddListener(action2);
    }
    public void b1()
    {
        videosInput.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(true);
        startBtn.GetComponentInChildren<Text>().text = "开始任务";
        nb = 1;
        description.text = "任务名称：设备、仪器\n任务内容：阅读PDF\n\n任务完成要求：阅读完成相应PDF\n当前状态：阅读总时间：" + infoDic["readTime2"] + "s\n";
        freshProblemBtn.gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
        UnityAction action = delegate ()
        {
            pdf.FileURL = "https://cemm20201009.oss-cn-qingdao.aliyuncs.com/m2/m2.pdf";
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
        videosInput.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(true);
        startBtn.GetComponentInChildren<Text>().text = "开始任务";
        nb = 2;
        description.text = "任务名称：阅读实验指导书\n任务内容：阅读PDF\n\n任务完成要求：阅读完成相应PDF\n当前状态：阅读总时间：" + infoDic["readTime3"] + "s\n";
        freshProblemBtn.gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
        UnityAction action = delegate ()
        {
            pdf.FileURL = "https://cemm20201009.oss-cn-qingdao.aliyuncs.com/m3/m3.pdf";
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
        videosInput.gameObject.SetActive(true);
        startBtn.gameObject.SetActive(false);
        nb = 3;
        description.text = "";
        freshProblemBtn.gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
        string path = Application.streamingAssetsPath + "/" + "videos" + ".txt";
        videosInput.text = "未获取";
        UnityAction action = delegate ()
        {
            videosInput.text = File.ReadAllText(path);
        };
        //List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.DOWNCOMMON.ToString()));
        //formData.Add(new MultipartFormDataSection(Constant.propname.username, "videos"));
        //StartCoroutine(web.getFileDownMain(WebData.connectUri, formData, slider, path, action));
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
        startBtn.onClick.RemoveAllListeners();
    }
    public void b4()
    {
        videosInput.gameObject.SetActive(false);
        startBtn.GetComponentInChildren<Text>().text = "开始任务";
        startBtn.gameObject.SetActive(true);
        nb = 4;
        description.text = "任务名称：安全操作考试\n任务内容：进行安全操作考试\n\n任务完成要求：安全操作考试达到90分以上\n当前状态：" + infoDic["score1"] + "\n";
        slider.gameObject.SetActive(false);
        freshProblemBtn.gameObject.SetActive(true);
        string path = Application.streamingAssetsPath + "/" + "Test1" + ".xls";
        UnityAction action = delegate ()
        {
            if (WebData.username == "")
            {
                //Message.MessageBox(IntPtr.Zero, "请登录", "注意", 0);
                Assets.Message.MessageBox("请登录");

                return;
            }
            if (!File.Exists(path))
            {
                OnSure os = delegate ()
                {
                    ReloadTest1();

                };
                Assets.Message.MessageBox("您还未获取题库，是否重新获取？",1,os);
                //int index = Message.MessageBox(IntPtr.Zero, "您还未获取题库，是否重新获取？", "注意", 1);
                //if (dr == DialogResult.No) return;
                return;
            }
            examPanel.SetActive(true);
            RefreshSchedule();
        };
        UnityAction refreshAction = delegate ()
        {
            OnSure os = delegate ()
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path);
                }
                ReloadTest1();
            };
            Assets.Message.MessageBox("是否更新当前题库？", 1, os);
        };    
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
        freshProblemBtn.onClick.RemoveAllListeners();
        freshProblemBtn.onClick.AddListener(refreshAction);
    }
    public void b5()
    {
        videosInput.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(true);
        startBtn.GetComponentInChildren<Text>().text = "开始任务";
        nb = 5;
        description.text = "任务名称：职业工程素养测试\n任务内容：进行职业工程素养测试\n\n任务完成要求：职业工程素养测试达到90分以上\n当前状态：" + infoDic["score2"] + "\n";
        slider.gameObject.SetActive(false);
        freshProblemBtn.gameObject.SetActive(true);
        string path1 = Application.streamingAssetsPath + "/" + "Test2" + ".xls";
        UnityAction action = delegate ()
        {
            if (WebData.username == "")
            {
                Assets.Message.MessageBox("请登录");
                //Message.MessageBox(IntPtr.Zero, "请登录", "注意", 0);
                return;
            }
            if (!File.Exists(path1))
            {
                OnSure os = delegate ()
                {
                    ReloadTest2();

                };
                Assets.Message.MessageBox("您还未获取题库，是否重新获取？", 1, os);
                
                return;
            }
            examPanel2.SetActive(true);
            RefreshSchedule();
        };
        UnityAction refreshAction = delegate ()
        {
            OnSure os = delegate ()
            {
                if (Directory.Exists(path1))
                {
                    Directory.Delete(path1);
                }
                ReloadTest2();
            };
            Assets.Message.MessageBox("是否更新当前题库？", 1, os);
        };

        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(action);
        freshProblemBtn.onClick.RemoveAllListeners();
        freshProblemBtn.onClick.AddListener(refreshAction);

    }
    public void b6()
    {
        videosInput.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(true);
        startBtn.GetComponentInChildren<Text>().text = "开始任务";
        freshProblemBtn.gameObject.SetActive(false);
        nb = 6;
        string ans = infoDic["file2date"].Contains("+") ? infoDic["file2date"].Substring(0, infoDic["file2date"].LastIndexOf('+')):infoDic["file2date"];
        description.text = "任务名称：人员分工\n任务内容：上传人员分工\n\n任务完成要求：上传完成人员分工\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "2";
        //UnityAction action = delegate ()
        //{
        //    //UploadSubmit(slider, "人员分工");
        //    RefreshSchedule();
        //};
        startBtn.onClick.RemoveAllListeners();
        UploadSubmit(slider, "人员分工");
        //startBtn.onClick.AddListener(action);
        //aoss.AddListenter_(startBtn,action,WebData.connectUri);
    }
    public void b7()
    {
        videosInput.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(true);
        startBtn.GetComponentInChildren<Text>().text = "开始任务";
        freshProblemBtn.gameObject.SetActive(false);
        nb = 7;
        string ans = infoDic["file3date"].Contains("+") ? infoDic["file3date"].Substring(0, infoDic["file3date"].LastIndexOf('+')) : infoDic["file3date"];
        description.text = "任务名称：模型分配\n任务内容：上传模型分配\n\n任务完成要求：上传完成模型分配\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "3";
        //UnityAction action = delegate ()
        //{
        //    slider.gameObject.SetActive(true);
        //    RefreshSchedule();
        //};

        startBtn.onClick.RemoveAllListeners();
            UploadSubmit(slider, "模型分配");

        //startBtn.onClick.AddListener(action);
    }
    public void b8()
    {
        videosInput.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(true);
        freshProblemBtn.gameObject.SetActive(false);
        startBtn.GetComponentInChildren<Text>().text = "开始任务";
        freshProblemBtn.gameObject.SetActive(false);
        nb = 8;
        string ans = infoDic["file4date"].Contains("+") ? infoDic["file4date"].Substring(0, infoDic["file4date"].LastIndexOf('+')) : infoDic["file4date"];
        description.text = description.text = "任务名称：加工工艺安排\n任务内容：上传加工工艺安排\n\n任务完成要求：上传完成加工工艺安排\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "4";
        //UnityAction action = delegate ()
        //{
        //    RefreshSchedule();
        //};

        startBtn.onClick.RemoveAllListeners();
            UploadSubmit(slider, "加工工艺安排");

        //startBtn.onClick.AddListener(action);
    }
    public void b9()
    {
        videosInput.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(true);
        startBtn.GetComponentInChildren<Text>().text = "开始任务";
        freshProblemBtn.gameObject.SetActive(false);
        nb = 9;
        string ans = infoDic["file5date"].Contains("+") ? infoDic["file5date"].Substring(0, infoDic["file5date"].LastIndexOf('+')) : infoDic["file5date"]; 
        description.text = description.text = "任务名称：凸凹模虚拟加工\n任务内容：上传凸凹模虚拟加工\n\n任务完成要求：上传完成凸凹模虚拟加工\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "5";

        //UnityAction action = delegate ()
        //{
        //    RefreshSchedule();
        //};

        startBtn.onClick.RemoveAllListeners();
            UploadSubmit(slider, "凸凹模虚拟加工");

        //startBtn.onClick.AddListener(action);
    }
    public void b10()
    {
        videosInput.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(true);
        startBtn.GetComponentInChildren<Text>().text = "开始任务";
        nb = 10;
        freshProblemBtn.gameObject.SetActive(false);
        string ans = infoDic["file6date"].Contains("+") ? infoDic["file6date"].Substring(0, infoDic["file6date"].LastIndexOf('+')) : infoDic["file6date"];
        description.text = description.text = "任务名称：凸凹模现实加工\n任务内容：上传凸凹模现实加工\n\n任务完成要求：上传完成凸凹模现实加工\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "6";
        //UnityAction action = delegate ()
        //{
        //    RefreshSchedule();
        //};

        startBtn.onClick.RemoveAllListeners();
            UploadSubmit(slider, "凸凹模现实加工");

        //startBtn.onClick.AddListener(action);
    }
    public void b11()
    {
        videosInput.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(true);
        startBtn.GetComponentInChildren<Text>().text = "开始任务";
        nb = 11;
        freshProblemBtn.gameObject.SetActive(false);
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
        videosInput.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(true);
        startBtn.GetComponentInChildren<Text>().text = "开始任务";
        nb = 12;
        freshProblemBtn.gameObject.SetActive(false);
        string ans = infoDic["file7date"].Contains("+") ? infoDic["file7date"].Substring(0, infoDic["file7date"].LastIndexOf('+')) : infoDic["file7date"];
        description.text = "任务名称：电火花放电现实加工\n任务内容：上传电火花放电现实加工\n任务完成要求：上传完成电火花放电现实加工\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "7";
        //UnityAction action = delegate ()
        //{
        //    RefreshSchedule();
        //};

        startBtn.onClick.RemoveAllListeners();
            UploadSubmit(slider, "电火花放电现实加工");

        //startBtn.onClick.AddListener(action);
    }
    public void b13()
    {
        videosInput.gameObject.SetActive(false);
        startBtn.GetComponentInChildren<Text>().text = "开始任务";
        startBtn.gameObject.SetActive(true);
        nb = 13;
        freshProblemBtn.gameObject.SetActive(false);
        string ans = infoDic["file8date"].Contains("+") ? infoDic["file8date"].Substring(0, infoDic["file8date"].LastIndexOf('+')) : infoDic["file8date"];
        description.text = description.text = "任务名称：尺寸\n任务内容：上传尺寸\n\n任务完成要求：上传完成尺寸\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "8";

        //UnityAction action = delegate ()
        //{
        //    RefreshSchedule();
        //};
        startBtn.onClick.RemoveAllListeners();
            UploadSubmit(slider, "尺寸");
        //startBtn.onClick.AddListener(action);
    }
    public void b14()
    {
        videosInput.gameObject.SetActive(false);
        startBtn.GetComponentInChildren<Text>().text = "开始任务";
        startBtn.gameObject.SetActive(true);
        nb = 14;
        freshProblemBtn.gameObject.SetActive(false);
        string ans = infoDic["file9date"].Contains("+") ? infoDic["file9date"].Substring(0, infoDic["file9date"].LastIndexOf('+')) : infoDic["file9date"];
        description.text = description.text = "任务名称：粗糙度\n任务内容：上传粗糙度\n\n任务完成要求：上传完成粗糙度\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "9";

        //UnityAction action = delegate ()
        //{
        //    RefreshSchedule();
        //};
        startBtn.onClick.RemoveAllListeners();
            UploadSubmit(slider, "粗糙度");
        //startBtn.onClick.AddListener(action);
    }
    public void b15()
    {
        videosInput.gameObject.SetActive(false);
        startBtn.GetComponentInChildren<Text>().text = "开始任务";
        startBtn.gameObject.SetActive(true);
        nb = 15;
        freshProblemBtn.gameObject.SetActive(false);
        string ans = infoDic["file10date"].Contains("+") ? infoDic["file10date"].Substring(0, infoDic["file10date"].LastIndexOf('+')) : infoDic["file10date"];
        description.text = description.text = "任务名称：实验总结\n任务内容：上传实验总结\n\n任务完成要求：上传完成实验总结\n当前状态：" + ans + "\n";
        slider.gameObject.SetActive(true);
        slider.name = "10";

        //UnityAction action = delegate ()
        //{
        //    RefreshSchedule();
        //};
        startBtn.onClick.RemoveAllListeners();
            UploadSubmit(slider, "实验总结");
        //startBtn.onClick.AddListener(action);
    }
    #region 帮助函数

    string openDia()
    {
        string path = WebData.OpenDialog();
        if (path == null)
        {
            Assets.Message.MessageBox("操作已取消");
            return null;
        }
        if (WebData.username == "")
        {
            Assets.Message.MessageBox("请登录");
            return null;
        }
        return path;
    }

        

    void UploadSubmit(Slider slider,string folderName)
    {
        aoss.AddListenter_(startBtn, RefreshSchedule, WebData.connectUri,folderName);
    }




    void GetUploadTime()
    {
        Action<string>[] actions = new Action<string>[10];
        for(int i = 0;i<10;i++)
        {
            StartCoroutine(web.GetInfo(WebData.connectUri, WebData.username, Constant.propname.file1date, actions[i]));
        }
    }

    public void ReloadTest2()
    {
        UnityAction action = delegate ()
        {
            startExam2.FreshProblem();
            print("重...");
            Assets.Message.MessageBox("获取成功");
        };
        string path = Application.streamingAssetsPath + "\\" + "Test2" + ".xls";
        Action<string> AddListenter_d = delegate (string res)
        {
            aoss.downpathInAli = res;
            aoss.AddListenter_d(path, action);
        };
        StartCoroutine(
       web.Get(WebData.connectUri + "?" + Constant.propname.requesttype + "=" + Constant.WebCommand.DOWNCOMMON.ToString() + "&" + Constant.propname.username + "=" + "职工素养测试",
        AddListenter_d));

    }
    public void ReloadTest1()
    {
        UnityAction action = delegate ()
        {
            startExam1.FreshProblem();
            print("重...");
            Assets.Message.MessageBox("获取成功");
        };
        string path = Application.streamingAssetsPath + "\\" + "Test1" + ".xls";
        Action<string> AddListenter_d = delegate (string res)
        {
            aoss.downpathInAli = res;
            aoss.AddListenter_d(path,action);
        };
        StartCoroutine(
        web.Get(WebData.connectUri + "?" + Constant.propname.requesttype + "=" + Constant.WebCommand.DOWNCOMMON.ToString() + "&" + Constant.propname.username + "=" + "实验安全考试",
        AddListenter_d));
    }
    public void Model1()
    {
        liucheng.SetActive(false);
        videosInput.gameObject.SetActive(false);
        freshProblemBtn.gameObject.SetActive(false);
        CancelButtons();
        for(int i = 0;i<4;i++)
            buttons[i].gameObject.SetActive(true);
        //buttons[15].gameObject.SetActive(true);
        description.text = "当前为认知模式";
        slider.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(false);

    }
    public void Model2()
    {
        liucheng.SetActive(false);
        videosInput.gameObject.SetActive(false);
        freshProblemBtn.gameObject.SetActive(false);
        CancelButtons();
        buttons[4].gameObject.SetActive(true);
        buttons[5].gameObject.SetActive(true);
        description.text = "当前为考试模式";
        slider.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(false);
    }
    public void Model3()
    {
        liucheng.SetActive(false);
        videosInput.gameObject.SetActive(false);
        freshProblemBtn.gameObject.SetActive(false);
        CancelButtons();
        buttons[15].gameObject.SetActive(true);
        buttons[16].gameObject.SetActive(true);
        buttons[17].gameObject.SetActive(true);
        buttons[18].gameObject.SetActive(true);
        description.text = "当前为实验模式";
        slider.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(false);
    }
    public void Model4()
    {
        
    }
    public void CancelButtons()
    {
        foreach(Button b in buttons)
        {
            b.gameObject.SetActive(false);
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
            if (str == null || str == "") return;
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
            foreach (Button b in buttons)
            {
                b.interactable = false;
            }
            buttons[0].interactable = true;
            buttons[1].interactable = true;
            buttons[2].interactable = true;
            buttons[3].interactable = true;
           // buttons[16].interactable = true;
            recordTask[0] = true;
            recordTask[3] = true;
            if (!infoDic["readTime2"].Contains("未完成")) recordTask[1] = true;
            else recordTask[1] = false;
            if (!infoDic["readTime3"].Contains("未完成")) recordTask[2] = true;
            else recordTask[2] = false;
            if (!infoDic["file1date"].Contains("未提交")) recordTask[3] = true;
            if (!infoDic["score1"].Contains("暂无") && int.Parse(infoDic["score1"]) >= 90) recordTask[4] = true;
            if (!infoDic["score2"].Contains("暂无") && int.Parse(infoDic["score2"]) >= 90) recordTask[5] = true;
            //2 3 4 5 6
            for (int i = 1; i < 6; i++)
            {
                if (!infoDic[keys[i]].Contains("未提交") && !infoDic[keys[i]].Contains("暂无"))
                {
                    recordTask[i + 5] = true;
                }
            }
            if (!infoDic["completeTime"].Contains("未完成")) recordTask[11] = true;
            for (int i = 0; i < 4; i++)
            {
                if (!infoDic[keys[i + 6]].Contains("未提交") && !infoDic[keys[i + 6]].Contains("暂无"))
                    recordTask[i + 12] = true;
            }
            if ( recordTask[1] && recordTask[2] && recordTask[3])
                recordTask[16] = true;
            else if (recordTask[1] || recordTask[2] || recordTask[3])
            {
                recordTask[16] = false;
            }
            if (recordTask[16])
            {
                buttons[4].interactable = true;
                buttons[5].interactable = true;
                //buttons[17].interactable = true;
                if (recordTask[4] && recordTask[5])
                    recordTask[17] = true;
            }
            if (recordTask[17])
            {
                buttons[6].interactable = true;
                buttons[7].interactable = true;
                buttons[8].interactable = true;
                buttons[9].interactable = true;
                buttons[11].interactable = true;
                if (recordTask[9])
                {
                    buttons[10].interactable = true;
                }
                if (recordTask[11])
                {
                    buttons[12].interactable = true;
                }
                buttons[13].interactable = true;
                buttons[14].interactable = true;
                buttons[15].interactable = true;
                

                for (int i = 16; i < 19; i++)
                {
                    buttons[i].interactable = true;
                }
            }

            if (recordTask[6] && recordTask[7] && recordTask[8])
                recordTask[19] = true;
            if (recordTask[10])
                recordTask[22] = true;

            if (recordTask[12])
                recordTask[23] = true;

            if (recordTask[23] && recordTask[22])
                recordTask[20] = true;

            if (recordTask[15])
                recordTask[21] = true;

            if (recordTask[19] && recordTask[20] && recordTask[15])
                recordTask[18] = true;
            for (int i = 0; i < 16; i++)
            {
                if (recordTask[i])
                {
                    buttons[i].interactable = true;
                }
            }
            for(int i = 16;i<19;i++)
            {
                if(recordTask[i+3])
                    buttons[i].interactable = true;
            }
            string ans;
            switch (nb)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    ans = infoDic["file1date"].Contains("+") ? infoDic["file1date"].Substring(0, infoDic["file1date"].LastIndexOf('+')) : infoDic["file1date"];
                    description.text = "任务名称：机床操作视频\n任务内容：上传机床操作视频\n\n任务完成要求：上传完成机床操作视频\n当前状态：" + ans + "\n";
                    break;
                case 4:
                    description.text = "任务名称：安全操作考试\n任务内容：进行安全操作考试\n\n任务完成要求：安全操作考试达到90分以上\n当前状态：" + infoDic["score1"] + "\n";
                    break;
                case 5:
                    description.text = "任务名称：职业工程素养测试\n任务内容：进行职业工程素养测试\n\n任务完成要求：职业工程素养测试达到90分以上\n当前状态：" + infoDic["score2"] + "\n";
                    break;
                case 6:
                    ans = infoDic["file2date"].Contains("+") ? infoDic["file2date"].Substring(0, infoDic["file2date"].LastIndexOf('+')) : infoDic["file2date"];
                    description.text = "任务名称：人员分工\n任务内容：上传人员分工\n\n任务完成要求：上传完成人员分工\n当前状态：" + ans + "\n";
                    b6();
                    break;
                case 7:
                    ans = infoDic["file3date"].Contains("+") ? infoDic["file3date"].Substring(0, infoDic["file3date"].LastIndexOf('+')) : infoDic["file3date"];
                    description.text = "任务名称：模型分配\n任务内容：上传模型分配\n\n任务完成要求：上传完成模型分配\n当前状态：" + ans + "\n";
                    b7();
                    break;
                case 8:
                    ans = infoDic["file4date"].Contains("+") ? infoDic["file4date"].Substring(0, infoDic["file4date"].LastIndexOf('+')) : infoDic["file4date"];
                    description.text = description.text = "任务名称：加工工艺安排\n任务内容：上传加工工艺安排\n\n任务完成要求：上传完成加工工艺安排\n当前状态：" + ans + "\n";
                    b8();
                    break;
                case 9:
                    ans = infoDic["file5date"].Contains("+") ? infoDic["file5date"].Substring(0, infoDic["file5date"].LastIndexOf('+')) : infoDic["file5date"];
                    description.text = description.text = "任务名称：凸凹模虚拟加工\n任务内容：上传凸凹模虚拟加工\n\n任务完成要求：上传完成凸凹模虚拟加工\n当前状态：" + ans + "\n";
                    b9();
                    break;
                case 10:
                    ans = infoDic["file6date"].Contains("+") ? infoDic["file6date"].Substring(0, infoDic["file6date"].LastIndexOf('+')) : infoDic["file6date"];
                    description.text = description.text = "任务名称：凸凹模现实加工\n任务内容：上传凸凹模现实加工\n\n任务完成要求：上传完成凸凹模现实加工\n当前状态：" + ans + "\n";
                    b10();
                    break;
                case 11:
                    break;
                case 12:
                    ans = infoDic["file7date"].Contains("+") ? infoDic["file7date"].Substring(0, infoDic["file7date"].LastIndexOf('+')) : infoDic["file7date"];
                    description.text = "任务名称：电火花放电现实加工\n任务内容：上传电火花放电现实加工\n任务完成要求：上传完成电火花放电现实加工\n当前状态：" + ans + "\n";
                    b12();
                    break;
                case 13:
                    ans = infoDic["file8date"].Contains("+") ? infoDic["file8date"].Substring(0, infoDic["file8date"].LastIndexOf('+')) : infoDic["file8date"];
                    description.text = description.text = "任务名称：尺寸\n任务内容：上传尺寸\n\n任务完成要求：上传完成尺寸\n当前状态：" + ans + "\n";
                    b13();
                    break;
                case 14:
                    ans = infoDic["file9date"].Contains("+") ? infoDic["file9date"].Substring(0, infoDic["file9date"].LastIndexOf('+')) : infoDic["file9date"];
                    description.text = description.text = "任务名称：粗糙度\n任务内容：上传粗糙度\n\n任务完成要求：上传完成粗糙度\n当前状态：" + ans + "\n";
                    b14();
                    break;
                case 15:
                    ans = infoDic["file10date"].Contains("+") ? infoDic["file10date"].Substring(0, infoDic["file10date"].LastIndexOf('+')) : infoDic["file10date"];
                    description.text = description.text = "任务名称：实验总结\n任务内容：上传实验总结\n\n任务完成要求：上传完成实验总结\n当前状态：" + ans + "\n";
                    b15();
                    break;
            }
            for(int i = 0;i<16;i++)
            {
                if (recordTask[i])
                    buttons[i].transform.GetComponent<Image>().color = finish;
            }
            //16 17 18
            for (int i = 16; i < 19; i++)
            {
                if (recordTask[i + 3])
                    buttons[i].transform.GetComponent<Image>().color = finish;
            }
        };
        StartCoroutine(web.ExcuteSend(WebData.connectUri, formData, action));
    }

    #endregion
}
