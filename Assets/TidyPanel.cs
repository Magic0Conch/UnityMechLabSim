using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = System.Random;


public class TidyPanel : MonoBehaviour
{
    bool[] vis = new bool[9];
    public InputField input;
    public Web web;
    public string[] tidyText;
    public Text hint;
    int visNum = 0;
    Random rd = new Random();
    List<int> list = new List<int>();
    string rightAns = "";
    int[] rightSqu = new int[9];

    private void Start()
    {
        for (int i = 0; i < list.Count; i++)
            vis[i] = false;

        while(list.Count<9)
        {
            int nowNum = rd.Next(0, 9);
            if (vis[nowNum])
                continue;
            vis[nowNum] = true;
            list.Add(nowNum);
            hint.text += list.Count + "." + tidyText[nowNum]+"\r\n";
            rightSqu[nowNum] = list.Count;
        }
        for (int i = 0; i < 9; i++)
            rightAns += rightSqu[i];
        print(rightAns);

    }
    //1、电极Z正向抬起50mm
    //2、关闭油泵
    //3、抬起控制油泵操作杆
    //4、待油液完全下降后，打开机床门
    //5、打开磁力吸盘
    //6、卸下工件
    //7、关闭机床门
    //8、关闭操作面板
    //9、按下急停、关闭机床电源。

    public void Verify()
    {
        if(input.text.Trim()==rightAns)
        {
            BasicData.levelSwitch[29] = true;
            BasicData.CompeleteTime = (int)Time.timeSinceLevelLoad;
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.UPCOMPELETETIME.ToString()));
            formData.Add(new MultipartFormDataSection(Constant.propname.username, WebData.username));
            formData.Add(new MultipartFormDataSection(Constant.propname.propName, BasicData.CompeleteTime.ToString()));
            StartCoroutine(web.ExcuteSend(WebData.connectUri, formData));
            //StartCoroutine(Post());
            Assets.Message.MessageBox("您已完成模拟实验，请返回主界面。");
            //Assets.Message.MessageBox(IntPtr.Zero, "您已完成模拟实验，请返回主界面。", "提示", 0);
        }
        else
        {
            DialogInfo dialogInfo = new DialogInfo();
            dialogInfo.openType = OpenMessageType.SureandCancle;
            dialogInfo.warnInfo = "您已完成模拟实验，请返回主界面。";
            dialogInfo.cancleBtnInfo = "关闭";

            Assets.Message.MessageBox("顺序错误");
 //           Assets.Message.MessageBox(IntPtr.Zero, "顺序错误", "提示", 0);
        }
    }

    IEnumerator Post()
    {
        WWWForm form = new WWWForm();
        //键值对
        form.AddField("key", "value");
        form.AddField("username", "mafanwei");
        form.AddField("completeTime",BasicData.CompeleteTime);

        UnityWebRequest webRequest = UnityWebRequest.Post(WebData.baseHttp + "/experiment/uploadFinal", form);

        yield return webRequest.SendWebRequest();
        
        if (webRequest.isHttpError || webRequest.isNetworkError)
            Debug.Log(webRequest.error);
        else
        {
            Debug.Log(webRequest.downloadHandler.text);
        }
    }
}
