using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System.Xml;
using NPOI.SS.UserModel;
using System;

public class TIMU
{
    public int NUM;/// 题数
    public string[] TrueA;///正确答案
    public string[] Timu;///题目
    public string[] Tixing;///题型
    public string[] Axx;
    public string[] Bxx;
    public string[] Cxx;
    public string[] Dxx;
    public string[] YourAns;
    public bool[] TrueF;///答案正确与否
    public bool[] HaveAns;///是否回答过

    public TIMU(int n)
    {
        TrueA = new string[n + 1];
        Timu = new string[n + 1];
        Tixing = new string[n + 1];
        Axx = new string[n + 1];
        Bxx = new string[n + 1];
        Cxx = new string[n + 1];
        Dxx = new string[n + 1];
        YourAns = new string[n + 1];
        TrueF = new bool[n + 1];
        HaveAns= new bool[n + 1];

        for (int i=0;i<=n;i++)
        {
            TrueF[i] = false;
            HaveAns[i] = false;
        }
    }
}

public class StartExam : MonoBehaviour
{
    public GameObject panel;
    public FlowPath1 flow;

    public GameObject exam;//试卷组建
    public Text Num;//题号（题型）
    public Text test;//题目，选项
    //public GameObject StartButton;//考试按钮
    public GameObject LastB;
    public GameObject NextB;
    public GameObject EndB;
    public GameObject BackB;//返回主界面
    public GameObject chooseAns;//选择的选项
    public Transform Tog;
    public Transform inA;//填空题答案
    public GameObject inputAns;//选择的选项
    public GameObject buttons;//生成按钮的父级
    public GameObject buttonT;//按钮模板
    public GameObject EndResult;
    public GameObject QAQ;//确定返回吗？
    public Text TimerText;//计时器
    //public Web web;
    

    public string excelName = @"测试.xls";
    public string propName = "score1";
    public string excelFilePath;
    ISheet sheet;
    public int n = 1;//当前题号
    //public int[] L;//一个数组，记录各选择题选择了什么、
    //string[] str;//记录填空题答案
    //bool[] RightF;//记录各题正确与否
    public int len;//表格有效数据行数
    public bool endF = false;//是否显示结束按钮
    public TIMU examTimu;
    int TimuN;
    int score;
    int timer = 0;
    public Web web;
    public void ExamStart()
    {
        
    }
    void Start()
    {
        if (buttons.transform.childCount == 0)
            FreshProblem();
        
    }

    public void ChangeColor()
    {
        Button[] btns = buttons.GetComponentsInChildren<Button>();
        for(int i = 0; i < btns.Length; i++)
        {
            if (examTimu.TrueF[i+1])
            {
                btns[i].GetComponent<Image>().color = Color.green;
            }
            else
            {
                btns[i].GetComponent<Image>().color = Color.red;
            }
        }
    }

    public void FreshProblem()
    {
        EndResult.SetActive(false);
        //StartButton.SetActive(true);
        int leng = buttons.transform.childCount;
        for (int i = 0; i < leng; i++)
            DestroyImmediate(buttons.transform.GetChild(0).gameObject);


        //OpenTimer(0);
        timer = 0;//开启时间
        //excelFilePath =Application.streamingAssetsPath + "\\"+excelName;
        //excelFilePath = @"F:\000乱七八糟集中营\测试.xls";
        IWorkbook workbook = WorkbookFactory.Create(excelFilePath);//读取表格
        sheet = workbook.GetSheetAt(0);//获取第一个工作簿
        len = sheet.PhysicalNumberOfRows - 2;///排除前两行
        IRow nRow = sheet.GetRow(0);///获取首行，读取题目数
        ICell _cell = nRow.GetCell(1);
        int tempN = 5;
        if (_cell.CellType == CellType.NUMERIC)
        {
            try
            {
                tempN = (int)(_cell.NumericCellValue);

            }
            catch (Exception e)
            {
                Debug.Log(_cell.NumericCellValue);
                Debug.Log(e.Message);
            }
        }
        else if (_cell.CellType == CellType.STRING)
        {
            tempN = int.Parse(_cell.StringCellValue.Trim());
        }
        else
        {
            Debug.LogErrorFormat("神奇的类:{0}型，无法转化。", _cell.CellType.ToString());

        }
        examTimu = new TIMU(tempN);
        examTimu.NUM = tempN;
        TimuN = tempN;
        n = 1;

        ChuTi();
        //读取excel，隐藏考试按钮，加载试卷/
        //StartButton.SetActive(false);
        exam.SetActive(true);
        buttons.SetActive(true);

        CreatButton();
        NextB.SetActive(true);
        TiMu(n);
    }

    public void NextButton()
    {
        NextB.SetActive(false);
        NextB.SetActive(true);
        if (n != TimuN)
        {
            n++;
            if (n != 1) LastB.SetActive(true);
            else LastB.SetActive(false);
            if (n == TimuN)
            {
                //print("n:" + n + "len:" + TimuN);
                NextB.SetActive(false);
                //EndB.SetActive(true);
            }
            //更新试卷题目
            TiMu(n);
        }
    }
    public void LastButton()
    {
        LastB.SetActive(false);
        LastB.SetActive(true);
        if (n != 1)
        {
            n--;
            if (n != TimuN)
            {
                NextB.SetActive(true);
                //EndB.SetActive(false);
            }
            else NextB.SetActive(false);
            if (n == 1) LastB.SetActive(false);
            //更新试卷题目
            TiMu(n);
        }
    }

    public void TiMu(int t)
    {
        string TiXing = examTimu.Tixing[t];
        test.text = examTimu.Timu[t] + "\n";
        if (TiXing == "选择")
        {
            Num.text = "第" + t + "题（选择）";//int转string n.tostring() 一般int加进字符串里直接默认转换成字符串了
            inputAns.SetActive(false);
            chooseAns.SetActive(true);//表格中从4到7是选项
            for (int i = 0; i <= 4; i++)//显示曾经做过的选项
            {
                Toggle toggle = Tog.GetChild(i + 1).GetComponent<Toggle>();
                if (int.Parse(examTimu.YourAns[t]) == i) toggle.isOn = true;
                Text textT = Tog.GetChild(i + 1).GetChild(1).GetComponent<Text>();//获得toggle下的lable
                if (i < 4)
                {
                    if (i == 0)
                        textT.text = "A：" + examTimu.Axx[t];
                    else if (i == 1)
                        textT.text = "B：" + examTimu.Bxx[t];
                    else if (i == 2)
                        textT.text = "C：" + examTimu.Cxx[t];
                    else if (i == 3)
                        textT.text = "D：" + examTimu.Dxx[t];
                }
            }
        }
        else if (TiXing == "填空")
        {
            chooseAns.SetActive(false);
            Num.text = "第" + t + "题（填空）";//int转string n.tostring() 一般int加进字符串里直接默认转换成字符串了
            chooseAns.SetActive(false);
            inputAns.SetActive(true);
            InputField InF = inA.GetComponent<InputField>();
            InF.text = examTimu.YourAns[t];
        }
    }
    public void Update()
    {
        //if(!endF)
            timer++;
        TimerText.text = string.Format("{0:D2}:{1:D2}", timer / 60 / 60, timer / 60 % 60);
        checkAns();
    }

    void checkAns()
    {
        IWorkbook workbook = WorkbookFactory.Create(excelFilePath);//读取表格
        sheet = workbook.GetSheetAt(0);//获取第一个工作簿
        len = sheet.PhysicalNumberOfRows - 2;///排除前两行
        IRow nRow = sheet.GetRow(n);//获取第n行
        string TiXing = examTimu.Tixing[n];
        string Answer = examTimu.TrueA[n];
        if (TiXing == "选择")
        {
            for (int i = 0; i < 4; i++)//记录做过的选项
            {
                Toggle toggle = Tog.GetChild(i + 1).GetComponent<Toggle>();
                if (toggle.isOn == true)
                {
                    examTimu.YourAns[n] = i.ToString();
                    //与正确答案比较，记录下正确的个数
                    if (Answer == examTimu.YourAns[n]) examTimu.TrueF[n] = true;
                    break;
                }
            }
            //if (endF == true)//显示正确答案
            //{
            //    string trueans="";

            //    if (examTimu.TrueA[n] == "0")
            //        trueans = "A";
            //    else if (examTimu.TrueA[n] == "1")
            //        trueans = "B";
            //    else if (examTimu.TrueA[n] == "2")
            //        trueans = "C";
            //    else if (examTimu.TrueA[n] == "3")
            //        trueans = "D";
            //    Text[] res = EndResult.GetComponentsInChildren<Text>();
            //    res[1].text = "正确答案为：" + trueans;
            //}
        }
        else if (TiXing == "填空")
        {
            InputField InF = inA.GetComponent<InputField>();
            examTimu.YourAns[n] = InF.text;
            string Yans = examTimu.YourAns[n].ToLower();//转为小写
            string Tans = Answer.ToLower();
            Yans = Yans.Replace(" ", "");//去除空格
            Tans = Tans.Replace(" ", "");
            if (Tans == Yans) examTimu.TrueF[n] = true;
            //if (endF == true)
            //{
            //    Text[] res = EndResult.GetComponentsInChildren<Text>();
            //    res[1].text = "正确答案为：" + examTimu.TrueA[n];
            //}
        }

    }

    public void CreatButton()
    {
        for (int i = 1; i <= examTimu.NUM; i++)
        {
            //buttonT.GetComponent<Button>().colors
            GameObject newButton = Instantiate(buttonT);
            newButton.name = i.ToString();
            newButton.transform.SetParent(buttons.transform); //设置Button的父物体
            Text showText = newButton.transform.Find("Text").GetComponent<Text>();
            showText.text = i.ToString();
        }
    }

    public void EndExam()
    {
        checkAns();
        ChangeColor();
        score = 0;
        endF = true;

        //EndB.SetActive(false);
        EndResult.SetActive(true);
        int trueNum = 0;//正确个数,最终分数
        for(int i=1;i<=TimuN;i++)
        {
            if (examTimu.TrueF[i] == true) trueNum++;
        }
        if (trueNum != 0)
            score = trueNum * 100 / TimuN;
        else
            score = 0;
        Text[] res = EndResult.GetComponentsInChildren<Text>();
        res[0].text ="最终得分为："+ score;

        //BackB.SetActive(true);


        Action<string> action = delegate (string ss)
        {
            Assets.Message.MessageBox("分数已更新");
            Action<string> action1 = delegate (string s1)
            {
                int.TryParse(s1.Trim(), out WebData.score1);
                flow.RefreshSchedule();
            };
            Action<string> action2 = delegate (string s1)
            {
                int.TryParse(s1.Trim(), out WebData.score2);
                flow.RefreshSchedule();
            };

            StartCoroutine(web.GetInfo(WebData.connectUri, WebData.username, Constant.propname.score1, action1));
            StartCoroutine(web.GetInfo(WebData.connectUri, WebData.username, Constant.propname.score2, action2));
        };
        List<IMultipartFormSection> formdata = new List<IMultipartFormSection>();
        formdata.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.SUBMITSCORE.ToString()));
        formdata.Add(new MultipartFormDataSection(Constant.propname.username, WebData.username));
        formdata.Add(new MultipartFormDataSection(Constant.propname.fileInDb, propName));
        formdata.Add(new MultipartFormDataSection(Constant.propname.filename, score.ToString()));
        StartCoroutine(web.ExcuteSend(WebData.connectUri, formdata, action));
    }
    public void Back()
    {
        //if(score!=100)
        //{
        //    BackB.SetActive(false);
        //    QAQ.SetActive(true);
        //}
        //else
        //{
        //直接返回，不用确认
        OnSure action = delegate ()
        {
            ExamStart();
            panel.SetActive(false);

        };
        Assets.Message.MessageBox("您确定退出考试吗？", 1,action);
        //}
    }

    public void WuQingBack()
    {
        exam.SetActive(false);
        //BackB.SetActive(false);
        EndResult.SetActive(false);
        //StartButton.SetActive(true);
        int leng = buttons.transform.childCount;
        for (int i = 0; i < leng; i++)
            DestroyImmediate(buttons.transform.GetChild(0).gameObject);
        //Start();
        ExamStart();
        timer = 0;
        //跳转回主界面
        panel.SetActive(false);
    }
    public void CheckAns()
    {
        QAQ.SetActive(false);
        //BackB.SetActive(true);
    }

    public void ChuTi()
    {//从题库随机抽取n个题目，把题目、答案、选项等分别存入数组
        endF = false;
        int[] sequence = new int[len];
        int[] Rand = new int[examTimu.NUM+1];///无重复数组
        for (int i = 0; i < len; i++)
            sequence[i] = i;
        int end = len - 1;
        for (int i = 1; i <=examTimu.NUM; i++)
        {
            int num = UnityEngine.Random.Range(2, end + 1);
            Rand[i] = sequence[num];
            sequence[num] = sequence[end];
            end--;

            IRow tempRow = sheet.GetRow(Rand[i]);
            examTimu.Tixing[i]= tempRow.GetCell(1).StringCellValue;
            examTimu.Timu[i] = tempRow.GetCell(2).StringCellValue;
            examTimu.TrueA[i]= tempRow.GetCell(3).StringCellValue;
            if (examTimu.Tixing[i] == "选择")
            {
                int[] shuzu = new int[8];
                int[] xx = new int[4];///4个选项的无重复数组
                for (int j = 0; j < 8; j++)
                    shuzu[j] = j;
                int endn = 7;
                for (int j = 0; j < 4; j++)
                {
                    int xxn = UnityEngine.Random.Range(4, endn + 1);
                    
                    xx[j] = shuzu[xxn];
                    shuzu[xxn] = shuzu[endn];
                    endn--;
                    if (examTimu.TrueA[i] == "A" && xx[j] == 4 || examTimu.TrueA[i] == "B" && xx[j] == 5 ||
                        examTimu.TrueA[i] == "C" && xx[j] == 6 || examTimu.TrueA[i] == "D" && xx[j] == 7)
                        examTimu.TrueA[i] = j.ToString();

                }
                ICell Acell = tempRow.GetCell(xx[0]);
                if (Acell.CellType == CellType.NUMERIC)
                {
                    try
                    {
                        examTimu.Axx[i] = Convert.ToString(Acell.NumericCellValue);

                    }
                    catch (Exception e)
                    {
                        Debug.Log(Acell.NumericCellValue);
                        Debug.Log(e.Message);
                    }
                }
                else if (Acell.CellType == CellType.STRING)
                {
                    examTimu.Axx[i] = Acell.StringCellValue.Trim();
                }
                ICell Bcell = tempRow.GetCell(xx[1]);
                if (Bcell.CellType == CellType.NUMERIC)
                {
                    try
                    {
                        examTimu.Bxx[i] = Convert.ToString(Bcell.NumericCellValue);

                    }
                    catch (Exception e)
                    {
                        Debug.Log(Bcell.NumericCellValue);
                        Debug.Log(e.Message);
                    }
                }
                else if (Bcell.CellType == CellType.STRING)
                {
                    examTimu.Bxx[i] = Bcell.StringCellValue.Trim();
                }
                ICell Ccell = tempRow.GetCell(xx[2]);
                if (Ccell.CellType == CellType.NUMERIC)
                {
                    try
                    {
                        examTimu.Cxx[i] = Convert.ToString(Ccell.NumericCellValue);

                    }
                    catch (Exception e)
                    {
                        Debug.Log(Ccell.NumericCellValue);
                        Debug.Log(e.Message);
                    }
                }
                else if (Ccell.CellType == CellType.STRING)
                {
                    examTimu.Cxx[i] = Ccell.StringCellValue.Trim();
                }
                ICell Dcell = tempRow.GetCell(xx[3]);
                if (Dcell.CellType == CellType.NUMERIC)
                {
                    try
                    {
                        examTimu.Dxx[i] = Convert.ToString(Dcell.NumericCellValue);

                    }
                    catch (Exception e)
                    {
                        Debug.Log(Dcell.NumericCellValue);
                        Debug.Log(e.Message);
                    }
                }
                else if (Dcell.CellType == CellType.STRING)
                {
                    examTimu.Dxx[i] = Dcell.StringCellValue.Trim();
                }

                //examTimu.Axx[i] = tempRow.GetCell(xx[0]).StringCellValue;
                //examTimu.Bxx[i] = tempRow.GetCell(xx[1]).StringCellValue;
                //examTimu.Cxx[i] = tempRow.GetCell(xx[2]).StringCellValue;
                //examTimu.Dxx[i] = tempRow.GetCell(xx[3]).StringCellValue;
                examTimu.YourAns[i] = "4";
            }
            else examTimu.YourAns[i] = "";
        }
    }
}
