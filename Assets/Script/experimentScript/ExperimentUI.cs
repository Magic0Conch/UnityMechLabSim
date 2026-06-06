using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Windows.Forms;

public class ExperimentUI : MonoBehaviour
{
    Web web;
    public GameObject UIPannel, remoteControl,hint,personalPannel,transimitProcess;
    private void Awake()
    {
        for (int i = 0; i < 100; i++)
            BasicData.levelSwitch[i] = false;
    }
    private void Start()
    {
        BasicData.levelSwitch = new bool[100];
        BasicData.speedMutiple = 2;
        BasicData.toolBarStatu = false;
        BasicData.banYMove = false;
        BasicData.isfixed = false;
        BasicData.iscol = false;
        BasicData.colZ = false;
        BasicData.ExistClock = false;
        BasicData.X = 0;
        BasicData.Y = 0;
        BasicData.Z = 0;
        BasicData.offset = Vector3.zero;
        BasicData.CompeleteTime = 0;

        BasicData.Scores = 0;
        BasicData.costTime = 0;

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
        go.SetActive(true);
        Slider slider = go.transform.GetChild(0).GetComponent<Slider>();
        string path = WebData.SaveProject();
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.DOWNCOMMON.ToString()));
        if (index == 0)//指导书
        {
            path += "\\" + "实验指导书" + ".doc";
            formData.Add(new MultipartFormDataSection(Constant.propname.username, "实验指导书"));
            StartCoroutine(web.getFileDown(WebData.connectUri, formData, slider, path));
        }
        else if(index == 1)
        {
            path += "\\" + "实验报告模板" + ".doc";
            formData.Add(new MultipartFormDataSection(Constant.propname.username, "实验报告模板"));
            StartCoroutine(web.getFileDown(WebData.connectUri, formData, slider, path));
        }
    }
    public void UChange(Dropdown dp)
    {
        int index = dp.value;
        //int cmd = Message.MessageBox(IntPtr.Zero, "您是否执行次操作？", "注意", 1);
        //if (cmd != 1) return;
        //if (WebData.username == "")
        //{
        //    Message.MessageBox(IntPtr.Zero, "未登录，无法进行此操作。", "注意", 0);
        //    return;
        //}

        DialogResult dr = MessageBox.Show("您是否执行次操作", "注意", MessageBoxButtons.YesNo);
        if(dr==DialogResult.No)
        {
            return;
        }
        else
        {
            if (WebData.username == "")
            {
                MessageBox.Show("未登录，无法进行本次操作");
                return;
            }
        }


        GameObject go = transimitProcess;
        transimitProcess.SetActive(true);
        Slider slider = go.transform.GetChild(0).GetComponent<Slider>();
        if (index == 0)
        {
            transimitProcess.SetActive(true);
            slider.name = "5";
            UploadSubmit(slider, "实验总结");
        }
        else if(index == 1)
        {
            transimitProcess.SetActive(true);
            slider.name = "6";
            UploadSubmit(slider, "成品检验");
        }
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
    public void ActiveHint()
    {
        hint.SetActive(true);
    }

    public void backToMenu()
    {
        UnityEngine.Screen.SetResolution(1600, 900, false);
        SceneManager.LoadScene("MainScene");
    }

    public void ActiveRemoteControl()
    {
        remoteControl.SetActive(!remoteControl.activeInHierarchy);
    }
    public void ActiveUIPannel()
    {
        UIPannel.SetActive(!UIPannel.activeInHierarchy);
    }

    public void SChange(Dropdown dp)
    {
        int index = dp.value;
        if (index == 0)
        {
            UnityEngine.Screen.SetResolution(1600, 900, false);
            SceneManager.LoadScene("MainScene");
        }
        else if(index == 1)
        {
           UnityEngine.Application.Quit();
        }
    }
    public void OChange(Dropdown dp)
    {
        int index = dp.value;
        if (index == 0)
        {
            remoteControl.SetActive(!remoteControl.activeInHierarchy);
        }
        else if (index == 1)
        {
            UIPannel.SetActive(!UIPannel.activeInHierarchy);
        }
    }
    #region 辅助方法
    void UploadSubmit(Slider slider, string floderName)
    {
        string path = WebData.OpenDialog();
        if (path == null)
        {
            //Message.MessageBox(IntPtr.Zero, "操作已取消", "注意", 0);
            MessageBox.Show("操作已取消");
            return;
        }
        if (WebData.username == "")
        {
            //Message.MessageBox(IntPtr.Zero, "请登录", "注意", 0);
            MessageBox.Show("请登录");
            return;
        }
        //Action action = GetUploadTime;
        StartCoroutine(web.UpLoadFile(WebData.connectUri, path, floderName, slider));
    }
    #endregion
}
