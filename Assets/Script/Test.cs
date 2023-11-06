using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Test : MonoBehaviour
{
    public Web web;

    public void Send()
    {
        //List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        ////formData.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.UPCOMPELETETIME.ToString()));
        //formData.Add(new MultipartFormDataSection("username", "12312"));
        //formData.Add(new MultipartFormDataSection("completeTime", "123"));
        //StartCoroutine(web.ExcuteSend("http://localhost:8888/experiment/uploadFinal", formData));

        //Assets.Message.MessageBox("您已完成模拟实验，请返回主界面。");
        StartCoroutine(Post());
    }

    IEnumerator Post()
    {
        WWWForm form = new WWWForm();
        //键值对
        form.AddField("key", "value");
        form.AddField("username", "mafanwei");
        form.AddField("completeTime", "qwe25878");

        UnityWebRequest webRequest = UnityWebRequest.Post("http://localhost:8888/experiment/uploadFinal", form);

        yield return webRequest.SendWebRequest();
        ////异常处理，很多博文用了error!=null这是错误的，请看下文其他属性部分
        if (webRequest.isHttpError || webRequest.isNetworkError)
            Debug.Log(webRequest.error);
        else
        {
            Debug.Log(webRequest.downloadHandler.text);
        }
    }
}
