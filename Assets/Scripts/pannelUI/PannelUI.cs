using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Common;
using UnityEngine.Networking;

public class PannelUI : MonoBehaviour
{
    #region 全局变量
    public GameObject[] middleButton;
    public GameObject[] images;
    public GameObject item0;
    public GameObject item1;
    public GameObject container0;
    public GameObject container1;
    public GameObject toggleGroup0;
    public GameObject toggleGroup1;
    public GameObject toggleGroup2;
    public Scrollbar scrollbar0;
    public Scrollbar scrollbar1;
    public MoveMachine moveMachine;
    public Text moveDisText;
    public Web web;
    public GameObject remainTimeGo;
    public GameObject tidyPanel;
    private GameObject nowMiddleButton;
    private GameObject nowImage;
    private InputField nowText;
    private int theAxisCode;
    private int fieldCode = 1;
    private int operatePanelCode = 1;

    EditState editState = EditState.EDIT;

    List<Content1> content1s = new List<Content1>();
    List<Content2> content2s = new List<Content2>();
    enum EditState
    {
        INSERT,
        EDIT
    }
    enum ChangeCode
    {
        BUTTON,
        IMAGE
    }
    enum ConstructorState
    {
        SUCCESS,
        FAILED
    }

    class Content1
    {
        public int fieldIndex;
        public string program;
        public float X;
        public float Y;
        public float Z;
        public float translationAmount;
        public int gaugeGroup;
        public int beginField;
        public int endField;
        public ConstructorState constructorState = ConstructorState.FAILED;
        public Content1(string a,string b,string c,string d,string e,string f,string g,string h,string i)
        {
            constructorState = ConstructorState.SUCCESS;
            try
            {
                fieldIndex = int.Parse(a);
                program = b;
                X = float.Parse(c);
                Y = float.Parse(d);
                Z = float.Parse(e);
                translationAmount = float.Parse(f);
                gaugeGroup = int.Parse(g);
                beginField = int.Parse(h);
                endField = int.Parse(i);
            }
            catch(Exception)
            {
                constructorState = ConstructorState.FAILED;
            }
        }
    }

    class Content2
    {
        public int[] data = new int[13];
        public ConstructorState constructorState = ConstructorState.FAILED;
        public Content2(string[] a)
        {
            constructorState = ConstructorState.SUCCESS;
            try
            {
                for (int i = 0; i < 13; i++)
                    data[i] = int.Parse(a[i]);
            }
            catch(Exception)
            {
                constructorState = ConstructorState.FAILED;
            }
        }
    }

    #endregion
    #region 面板1方法集
    private bool Save(Content1 con)
    {
        //重新组织链表
        if (con.constructorState == ConstructorState.FAILED)
            return false;
        if(editState==EditState.INSERT)
        {
            bool flag = false;
            content1s.Insert(con.fieldIndex - 1, con);
            foreach(Content1 tmp in content1s)
            {
                if (flag)
                    tmp.fieldIndex++;
                if(tmp.fieldIndex==con.fieldIndex)
                    flag = true;
            }
        }
        else if(editState == EditState.EDIT)
        {
            content1s[con.fieldIndex - 1] = con;
        }
        return true;
    }
    private void CopyToScreen()
    {
        if(operatePanelCode==1)
        {
            int t = 0;
            foreach (Transform item in container0.transform)
            {
                Text[] texts = item.GetComponentsInChildren<Text>();
                texts[0].text = content1s[t].fieldIndex.ToString();
                texts[1].text = content1s[t].program;
                texts[2].text = content1s[t].X.ToString("0.000");
                texts[3].text = content1s[t].Y.ToString("0.000");
                texts[4].text = content1s[t].Z.ToString("0.000");
                texts[5].text = content1s[t].translationAmount.ToString("0.000");
                texts[6].text = content1s[t].gaugeGroup.ToString();
                texts[7].text = content1s[t].beginField.ToString();
                texts[8].text = content1s[t++].endField.ToString();
            }
        }
        else if(operatePanelCode==2)
        {
            int t = 0;
            foreach(Transform item in container1.transform)
            {
                Text[] texts = item.GetComponentsInChildren<Text>();
                for(int i = 0;i<13;i++)
                {
                    texts[i].text = content2s[t].data[i].ToString();
                }
                ++t;
            }
        }
    }
    private bool Save(Content2 con)
    {

        if (con.constructorState == ConstructorState.FAILED)
            return false;
        if (editState == EditState.INSERT)
        {
            bool flag = false;
            content2s.Insert(con.data[0] - 1, con);
            foreach (Content2 tmp in content2s)
            {
                if (flag)
                    tmp.data[0]++;
                if (tmp.data[0] == con.data[0])
                    flag = true;
            }
        }
        else if (editState == EditState.EDIT)
        {
            content2s[con.data[0] - 1] = con;
        }
        return true;
    }
    void LoadFromScreen()
    {
        content1s.Clear();
        foreach (Transform item in container0.transform)
        {
            Text[] texts = item.GetComponentsInChildren<Text>();
            Content1 tmp = new Content1(texts[0].text,texts[1].text, texts[2].text, texts[3].text, texts[4].text, texts[5].text, texts[6].text, texts[7].text, texts[8].text);
            content1s.Add(tmp);
        }
        content2s.Clear();
        foreach(Transform item in container1.transform)
        {
            Text[] texts = item.GetComponentsInChildren<Text>();
            string[] ss = new string[13];
            for (int i = 0; i < 13; i++)
                ss[i] = texts[i].text;
            Content2 tmp = new Content2(ss);
            content2s.Add(tmp);
        }
    }
    private void OnEnable()
    {
        nowMiddleButton = null;
        nowImage = null;
        nowText = null;
        foreach (GameObject go in middleButton)
        {
            if (go.activeInHierarchy)
                nowMiddleButton = go;
        }
        foreach(GameObject go in images)
        {
            if (go.activeInHierarchy)
                nowImage = go;
        }
        LoadFromScreen();
    }
    public void OnfieldClick()
    {
        GameObject go = EventSystem.current.currentSelectedGameObject;
        fieldCode = int.Parse(go.transform.GetChild(0).GetComponent<Text>().text);
    }
    public void OnAxisProgramming()
    {
        Change(middleButton[1], ChangeCode.BUTTON);
    }
    public void OnFieldChooseClick()
    {

    }
    public void OnFileManageClick()
    {
        Change(middleButton[2], ChangeCode.BUTTON);
        Change(images[0], ChangeCode.IMAGE);
    }
    public void OnCoordinateSettingClick()
    {
        Change(middleButton[3], ChangeCode.BUTTON);
    }
    public void OnGaugedPictureClick()
    {
        Change(middleButton[4], ChangeCode.BUTTON);
        Change(images[1], ChangeCode.IMAGE);
        operatePanelCode = 2;
    }
    public void On3AxisPictureClick()
    {
        Change(middleButton[0], ChangeCode.BUTTON);
        images[1].SetActive(false);
        nowImage = null;
        operatePanelCode = 1;
    }
    public void OnBackClick()
    {
        if(operatePanelCode==1)
        {
            Change(middleButton[0], ChangeCode.BUTTON);
            if (nowImage != null) 
            {
                nowImage.SetActive(false);
                nowImage = null;
            }
        }
        else if(operatePanelCode==2)
        {
            if (images[0].activeInHierarchy)
                images[0].SetActive(false);
            else
                images[7].SetActive(false);
            Change(middleButton[4], ChangeCode.BUTTON);
            Change(images[1], ChangeCode.IMAGE);
        }
    }
    public void OnFieldInsertClick()
    {
        if(operatePanelCode==1)
        {
            Change(images[2], ChangeCode.IMAGE);
            images[2].GetComponentInChildren<Text>().text = fieldCode.ToString();
            InputField[] inpts = images[2].GetComponentsInChildren<InputField>();
            foreach(InputField input in inpts)
            {
                input.text = "";
            }
            editState = EditState.INSERT;
            nowText = null;
        }
        else if(operatePanelCode==2)
        {
            Change(images[3], ChangeCode.IMAGE);
            images[3].GetComponentInChildren<Text>().text = fieldCode.ToString();
            InputField[] inpts = images[3].GetComponentsInChildren<InputField>();
            foreach (InputField input in inpts)
            {
                input.text = "";
            }

            editState = EditState.INSERT;
            nowText = null;
        }
    }
    public void OnFieldInsertOKClick()
    {
        string[] s = new string[9];
        for (int i = 0; i < 9; i++)
        {
            Transform go = images[2].transform.GetChild(i);
            Dropdown dropdown = go.GetComponent<Dropdown>();
            InputField inputField = go.GetComponent<InputField>();
            if (dropdown != null)
            {
                s[i] = dropdown.captionText.text;
            }
            else if(inputField != null)
            {
                s[i] = inputField.text;
            }
            else
            {
                s[i] = go.GetComponent<Text>().text;
            }
        }
        images[2].SetActive(false);
        nowText = null;
        Content1 cont = new Content1(s[0],s[1],s[2],s[3],s[4],s[5],s[6],s[7],s[8]);
        if(editState == EditState.INSERT)
        {
            if(cont.constructorState==ConstructorState.SUCCESS)
            {
                if(Save(cont))
                {
                    GameObject go = Instantiate(item0);
                    go.transform.SetParent(container0.transform);
                    CopyToScreen();
                    go.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                    container0.GetComponent<RectTransform>().sizeDelta += Vector2.up * 24;
                    go.GetComponent<Button>().onClick.AddListener(OnfieldClick);
                }
            }
        }
        else if(editState == EditState.EDIT)
        {
            if (cont.constructorState == ConstructorState.SUCCESS)
            {
                if(Save(cont))
                {
                    CopyToScreen();
                }
            }
        }
    }
    public void OnI3OKClick()
    {
        string[] s = new string[13];
        for (int i = 0; i < 13; i++)
        {
            Transform go = images[3].transform.GetChild(i);
            InputField inputField = go.GetComponent<InputField>();
            if (inputField != null)
                s[i] = inputField.text;
            else
                s[i] = go.GetComponent<Text>().text;
        }
        images[3].SetActive(false);
        nowText = null;
        Content2 cont = new Content2(s);
        if (editState == EditState.INSERT)
        {
            if (cont.constructorState == ConstructorState.SUCCESS)
            {
                if (Save(cont))
                {
                    GameObject go = Instantiate(item1);
                    go.transform.SetParent(container1.transform);
                    CopyToScreen();
                    go.GetComponent<RectTransform>().localScale = new Vector3(0.98f, 0.98f, 0.98f);
                    container1.GetComponent<RectTransform>().sizeDelta += Vector2.up * 24;
                    go.GetComponent<Button>().onClick.AddListener(OnfieldClick);
                }
            }
        }
        else if (editState == EditState.EDIT)
        {
            if (cont.constructorState == ConstructorState.SUCCESS)
            {
                if (Save(cont))
                {
                    CopyToScreen();
                }
            }
        }
    }
    public void OnUpPageClick()
    {
        if(operatePanelCode==1)
            scrollbar0.value = Mathf.Clamp01(scrollbar0.value + 168 / container0.GetComponent<RectTransform>().sizeDelta.y);
        else if(operatePanelCode==2)
            scrollbar1.value = Mathf.Clamp01(scrollbar0.value + 168 / container0.GetComponent<RectTransform>().sizeDelta.y);

    }
    public void OnDownPageClick()
    {
        if(operatePanelCode==1)
            scrollbar0.value = Mathf.Clamp01(scrollbar0.value - 168 / container0.GetComponent<RectTransform>().sizeDelta.y);
        else if(operatePanelCode==2)
            scrollbar1.value = Mathf.Clamp01(scrollbar0.value - 168 / container0.GetComponent<RectTransform>().sizeDelta.y);

    }
    public void OnDeleteFieldClick()
    {
        if(operatePanelCode==1)
        {
            if (fieldCode <= 0 || fieldCode > content1s.Count) return;
            content1s.RemoveAt(fieldCode - 1);
            for (int i = fieldCode-1; i < content1s.Count; i++)
                content1s[i].fieldIndex--;
            DestroyImmediate(container0.transform.GetChild(0).gameObject);
            CopyToScreen();
            container0.GetComponent<RectTransform>().sizeDelta -= Vector2.up * 24;
        }
        else if(operatePanelCode==2)
        {
            if (fieldCode <= 0 || fieldCode > content2s.Count) return;
            content2s.RemoveAt(fieldCode - 1);
            for (int i = fieldCode - 1; i < content2s.Count; i++)
                content2s[i].data[0]--;
            DestroyImmediate(container1.transform.GetChild(0).gameObject);
            CopyToScreen();
            container1.GetComponent<RectTransform>().sizeDelta -= Vector2.up * 24;
        }
    }
    public void OnEditFieldClick()
    {
        if(operatePanelCode==1)
        {
            Change(images[2], ChangeCode.IMAGE);
            images[2].GetComponentInChildren<Text>().text = fieldCode.ToString();
            InputField[] inpts = images[2].GetComponentsInChildren<InputField>();
            Text[] datas = container0.transform.GetChild(fieldCode-1).GetComponentsInChildren<Text>();
            for(int i = 0;i<7;i++)
            {
                inpts[i].text = datas[i+2].text;
            }
            editState = EditState.EDIT;
            nowText = null;
        }
        else if(operatePanelCode==2)
        {
            Change(images[3], ChangeCode.IMAGE);
            images[3].GetComponentInChildren<Text>().text = fieldCode.ToString();
            InputField[] inpts = images[3].GetComponentsInChildren<InputField>();
            Text[] datas = container1.transform.GetChild(fieldCode - 1).GetComponentsInChildren<Text>();
            for (int i = 0; i < 12; i++)
            {
                inpts[i].text = datas[i + 1].text;
            }


            editState = EditState.EDIT;
            nowText = null;
        }
    }
    public void OnCancelClick()
    {
        if(operatePanelCode==1)
        {
            if (nowImage != null)
            {
                nowImage.SetActive(false);
                nowImage = null;
            }
        }
        else if(operatePanelCode==2)
        {
            nowImage.SetActive(false);
            nowImage = null;
        }
    }
    #region 数字键盘
    public void OnInputClick()
    {
        GameObject go = EventSystem.current.currentSelectedGameObject;
        nowText = go.GetComponent<InputField>();
    }
    public void Number()
    {
        if (nowText == null) return;
        string text = EventSystem.current.currentSelectedGameObject.name;
        if (text=="-")
        {
            if (nowText.text.Length == 0 || nowText.text[0] != '-')
                nowText.text = '-' + nowText.text;
            else if (nowText.text.Length != 0 && nowText.text[0] == '-')
                nowText.text = nowText.text.Substring(1);
        }
        else if(text==".")
        {
            if (!nowText.text.Contains("."))
                nowText.text += ".";
            else if(nowText.text.EndsWith("."))
                nowText.text = nowText.text.Substring(0, nowText.text.Length - 1);
        }
        else if(text=="d")
        {
            if(nowText.text.Length != 0)
                nowText.text = nowText.text.Substring(0, nowText.text.Length - 1);
        }
        else if(text=="n")
        {
            nowText.text = "";
        }
        else
            nowText.text += text;
    }

    #endregion
    #endregion
    public void ProcessingGauged()
    {
        Change(middleButton[1], ChangeCode.BUTTON);
        operatePanelCode = 2;
    }
    public void ExpertGauged()
    {
        Change(images[4], ChangeCode.IMAGE);
        images[1].SetActive(true);
    }
    public void ExpertFile()
    {
        Change(images[7], ChangeCode.IMAGE);
        Change(middleButton[5], ChangeCode.BUTTON);
    }
    public void I4CancelClick()
    {
        images[4].SetActive(false);
    }
    public void ExitClick()
    {
        gameObject.SetActive(false);
    }
    public void OnXAxisClick()
    {
        theAxisCode = 1;
    }
    public void OnYAxisClick()
    {
        theAxisCode = 2;
    }
    public void OnZAxisClick()
    {
        theAxisCode = 3;
    }
    public void CoordinatesDivide()
    {
        float temp = 0;
        switch(theAxisCode)
        {
            case 1:
                temp = (BasicData.X * 100 + BasicData.offset.x) / 2;
                BasicData.offset.x = temp - BasicData.X * 100;
                break;
            case 2:
                temp = (BasicData.Y * 80 + BasicData.offset.y) / 2;
                BasicData.offset.y = temp - BasicData.Y * 80;
                break;
            case 3:
                temp = (BasicData.Z * 100 + BasicData.offset.z) / 2;
                BasicData.offset.z = temp - BasicData.Z * 100;
                break;
        }
    }
    public void ResetCoordinates()
    {
        switch (theAxisCode)
        {
            case 1:
                BasicData.offset.x = - BasicData.X * 100;
                break;
            case 2:
                BasicData.offset.y = - BasicData.Y * 80;
                break;
            case 3:
                BasicData.offset.z = - BasicData.Z * 100;
                break;
        }
    }
    public void OnProgramButtonClick()
    {
        editState = EditState.INSERT;
        string[] s = new string[13];
        s[0] = (content2s.Count).ToString();
        s[1] = s[2] = "1";
        s[3] = "3";
        s[4] = "300";
        s[5] = "100";
        s[6] = "30";
        s[7] = "9";
        s[8] = "5";
        s[9] = "1";
        s[10] = "1";
        s[11] = "1";
        s[12] = "0";
        images[4].SetActive(false);
        nowText = null;
        Content2 cont = new Content2(s);
        if (cont.constructorState == ConstructorState.SUCCESS)
        {
            if (Save(cont))
            {
                GameObject go = Instantiate(item1);
                go.transform.SetParent(container1.transform);
                CopyToScreen();
                go.GetComponent<RectTransform>().localScale = new Vector3(0.98f, 0.98f, 0.98f);
                container1.GetComponent<RectTransform>().sizeDelta += Vector2.up * 24;
                go.GetComponent<Button>().onClick.AddListener(OnfieldClick);
            }
        }
    }
    public void AxisMove()
    {
        Change(images[5], ChangeCode.IMAGE);
    }
    public void I5Cancel()
    {
        if(images[5].activeInHierarchy)
            images[5].SetActive(false);
    }
    public void OnI5OkClick()
    {
        //moveDisText
        BasicData.levelSwitch[33] = true;
        moveMachine.maxMoveDistance = 0;
        float.TryParse(moveDisText.text,out moveMachine.maxMoveDistance);
        moveMachine.movedDistance = 0;
        int t = 0;
        moveMachine.isDistanceMove = true;
        foreach (Transform tran in toggleGroup0.transform)
        {
            Toggle toggle = tran.GetComponent<Toggle>();
            if (toggle.isOn)
                break;
            t++;
        }
        if (t == 2) //Z
        {
            print("2");
            //print(BasicData.X);
            //print(BasicData.offset.x / 100);
            moveMachine.maxMoveDistance /= 100;
            moveMachine.maxMoveDistance += BasicData.Z + BasicData.offset.z / 100;
            if (moveMachine.maxMoveDistance>0)
                StartCoroutine(moveMachine.Xp());
            else
            {
                moveMachine.maxMoveDistance = -moveMachine.maxMoveDistance;
                StartCoroutine(moveMachine.Xs());
            }
        }
        else if (t == 1)
        {
            print("1");
            //print(BasicData.Y);
            //print(BasicData.offset.y / 80);
            moveMachine.maxMoveDistance /= 80;
            moveMachine.maxMoveDistance -= BasicData.Y + BasicData.offset.y/80;
            if (moveMachine.maxMoveDistance > 0)
                StartCoroutine(moveMachine.Yp());
            else
            {
                moveMachine.maxMoveDistance = -moveMachine.maxMoveDistance;
                StartCoroutine(moveMachine.Ys());
            }

        }
        else if (t == 0)//x
        {
            print("0");
            //print(BasicData.Z);
            //print(BasicData.offset.z / 100);
            moveMachine.maxMoveDistance /= 100;
            moveMachine.maxMoveDistance -= BasicData.X + BasicData.offset.x / 100;
            if (moveMachine.maxMoveDistance > 0)
                StartCoroutine(moveMachine.Zs());
            else
            {
                moveMachine.maxMoveDistance = -moveMachine.maxMoveDistance;
                StartCoroutine(moveMachine.Zp());
            }
        }

    }
    public void AutoToolSetting()
    {
        if(BasicData.levelSwitch[17])
        {
            GameObject go = EventSystem.current.currentSelectedGameObject;
            go.transform.GetChild(2).gameObject.SetActive(!go.transform.GetChild(2).gameObject.activeInHierarchy);
            Change(images[6], ChangeCode.IMAGE);
        }
    }
    public void OnI6OKClick()
    {
        int t0 = 0;
        int t1 = 0;
        moveMachine.isAutoToolSetting = false;
        moveMachine.isAutoToolSettingSuccess = false;
        foreach (Transform tran in toggleGroup1.transform)
        {
            Toggle toggle = tran.GetComponent<Toggle>();
            if (toggle.isOn)
                break;
            t0++;
        }
        foreach (Transform tran in toggleGroup2.transform)
        {
            Toggle toggle = tran.GetComponent<Toggle>();
            if (toggle.isOn)
                break;
            t1++;
        }
        print(t0);
        switch(t0)
        {
            case 0:
                moveMachine.isAutoToolSetting = true;
                theAxisCode = 3;
                moveMachine.onClickXp();
                break;
            case 1:
                theAxisCode = 2;
                moveMachine.isAutoToolSetting = true;
                moveMachine.onClickYp();
                break;
            case 2:
                theAxisCode = 1;
                moveMachine.isAutoToolSetting = true;
                moveMachine.onClickZp();
                break;
            case 3:
                theAxisCode = 3;

                moveMachine.isAutoToolSetting = true;
                moveMachine.onClickXs();
                break;
            case 4:
                theAxisCode = 2;
                moveMachine.isAutoToolSetting = true;
                moveMachine.onClickYs();
                break;
            case 5:
                theAxisCode = 1;
                moveMachine.isAutoToolSetting = true;
                moveMachine.onClickZs();
                break;
        }
    }
    public void OnI6CancelClick()
    {
        if(images[6].activeInHierarchy)
        {
            images[6].SetActive(false);
            GameObject.Find("autotoolsetting").transform.GetChild(2).gameObject.SetActive(!GameObject.Find("autotoolsetting").transform.GetChild(2).gameObject.activeInHierarchy);
        }
    }
    public void SwitchOnOff()
    {
        GameObject go = EventSystem.current.currentSelectedGameObject;
        go.transform.GetChild(2).gameObject.SetActive(!go.transform.GetChild(2).gameObject.activeInHierarchy);
    }
    public void OilSwitch()
    {
        if (!BasicData.levelSwitch[24])
        {
            Instantiate(GameObject.Find("Canvas").transform.GetChild(11), GameObject.Find("Canvas").transform).gameObject.SetActive(true);
        }
        if (BasicData.levelSwitch[24])//当关门结束
        {
            GameObject go = EventSystem.current.currentSelectedGameObject;
            go.transform.GetChild(2).gameObject.SetActive(!go.transform.GetChild(2).gameObject.activeInHierarchy);
            if(go.transform.GetChild(2).gameObject.activeInHierarchy)
            {
                BasicData.levelSwitch[25] = true;
            }
            //GameObject.Find("MovablePole").GetComponent<Animator>().speed = 1;
            //GameObject.Find("Oil").GetComponent<Animator>().speed = 1;
        }
    }
    public void OnProcessButtonClick()
    {
        if (!BasicData.levelSwitch[26])
        {
            Instantiate(GameObject.Find("Canvas").transform.GetChild(11), GameObject.Find("Canvas").transform).gameObject.SetActive(true);
        }
        if (BasicData.levelSwitch[26])//放油结束时
        {
            GameObject go0 = EventSystem.current.currentSelectedGameObject;
            go0.transform.GetChild(2).gameObject.SetActive(!go0.transform.GetChild(2).gameObject.activeInHierarchy);
            GameObject go = GameObject.Find("plat");
            go.transform.GetChild(1).gameObject.SetActive(true);
            BasicData.levelSwitch[27] = true;
            OnSure os = delegate ()
            {

                remainTimeGo.SetActive(true);
            };
            Assets.Message.MessageBox("本次加工预计时间10分钟",1,os);
            //Assets.Message.MessageBox(IntPtr.Zero, "本次加工预计时间10分钟", "提示", 0);
            go.SetActive(true);


            
        }
    }
    
    void Change(GameObject go,ChangeCode changeCode)
    {
        switch(changeCode)
        {
            case ChangeCode.BUTTON:
                go.SetActive(true);
                if(nowMiddleButton!=null)
                    nowMiddleButton.SetActive(false);
                nowMiddleButton = go;
                break;
            case ChangeCode.IMAGE:
                if(nowImage!=null&&operatePanelCode==1)
                    nowImage.SetActive(false);
                go.SetActive(true); 
                nowImage = go;
                break;
        }
    }
}
