using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PersonalUIItem : MonoBehaviour
{

    GameObject infoPannel;
    GetAllInfoRequest getAllInfoRequest;
    string theUsername;
    GameObject ViewPort;
    //TeacherUI teacherUI;
    InputField[] inputField;

    void Start()
    {
        //teacherUI = GameObject.Find("Main Camera").GetComponent<TeacherUI>();
        theUsername = transform.GetChild(0).GetComponent<Text>().text;
        ViewPort = GameObject.Find("Viewport");
        infoPannel = ViewPort.transform.GetChild(1).gameObject;
        getAllInfoRequest = GameObject.Find("Main Camera").GetComponent<GetAllInfoRequest>();
        inputField = infoPannel.GetComponentsInChildren<InputField>();
    }
    public void CheckInfo()
    {
        infoPannel.SetActive(true);
        string userName = GetComponentInChildren<Text>().text;
        //TeacherUI.PersonalInfo personalInfo = teacherUI.userInfo[userName];
        //infoPannel.transform.GetChild(0).GetComponent<Text>().text = userName;
        //infoPannel.transform.GetChild(1).GetComponent<Text>().text = personalInfo.password;
        //infoPannel.transform.GetChild(2).GetComponent<Text>().text = personalInfo.major;
        //infoPannel.transform.GetChild(3).GetComponent<Text>().text = personalInfo.theClass;
        //infoPannel.transform.GetChild(4).GetComponent<Text>().text = personalInfo.score;
        //infoPannel.transform.GetChild(5).GetComponent<Text>().text = personalInfo.testRecord;

        //inputField[0].text = personalInfo.password;
        //inputField[1].text = personalInfo.major;
        //inputField[2].text = personalInfo.theClass;
        //inputField[3].text = personalInfo.score;
        //inputField[4].text = personalInfo.testRecord;

        
}

    

    public void DownloadInfo()
    {
        BasicData.theUsername = theUsername;
        DownLoadScript.getFileScript();
        DownLoadScript.DownLoadFile(theUsername);
    }
}
