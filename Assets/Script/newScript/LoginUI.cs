using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets;

public class LoginUI : MonoBehaviour
{
    public Text hintMessage;
    public InputField usernameText, passwordText, uriText;
    public string InitConnectUri = "http://localhost:40/api/values";
    public string connectUri;
    void Start()
    {
        connectUri = PlayerPrefs.GetString("connectUri");
        if(connectUri==null||connectUri=="")
        {
            connectUri = InitConnectUri;
        }

        if (WebData.connectUri=="")
        {
            WebData.connectUri = connectUri;
            uriText.text = connectUri;
        }
        else
        {
            uriText.text = WebData.connectUri;

        }
    }
    private void Awake()
    {
        Screen.SetResolution(900, 600, false);
    }
    public void Ok()
    {
        WebData.connectUri = connectUri = uriText.text;
        PlayerPrefs.SetString("connectUri", uriText.text);

    }

    public void SwitchOpen(GameObject go)
    {
        go.SetActive(!go.activeInHierarchy);
    }

    public void Login()
    {
        StartCoroutine(Login(WebData.connectUri, usernameText.text, passwordText.text));
    }
    
    public void offLine()
    {
        OnSure action = delegate ()
        {
            Screen.SetResolution(1600, 900, false);
            SceneManager.LoadScene("SampleScene");
        };
        Assets.Message.MessageBox("离线模式会导致大部分功能不可用。\n确认以离线模式开始吗？", 1, action);    
    }
    IEnumerator Login(string url, string username, string password)
    {
        Dictionary<string, string> InfoDic = new Dictionary<string, string>();
        InfoDic[Constant.propname.requesttype] = Constant.WebCommand.LOGIN.ToString();
        InfoDic[Constant.propname.username] = username;
        InfoDic[Constant.propname.password] = password;
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        foreach (var item in InfoDic)
            formData.Add(new MultipartFormDataSection(item.Key, item.Value));
        UnityWebRequest request = UnityWebRequest.Post(url, formData);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
            hintMessage.text = request.error;
        else
        {
            Constant.loginState loginState;
            bool connectedSuccess = Enum.TryParse(request.downloadHandler.text, out loginState);
            if (connectedSuccess)
            {
                switch (loginState)
                {
                    case Constant.loginState.TEACHERSUCCESS:
                        //loginStateText.text = "已登录";
                        hintMessage.text = "登陆成功";
                        //WebData.username = usernameText.text;
                        Screen.SetResolution(1600, 900, false);
                        SceneManager.LoadScene("Tscene");
                        break;
                    case Constant.loginState.STUDENTSUCCESS:
                        //loginStateText.text = "已登录";
                        hintMessage.text = "登陆成功";
                        WebData.username = usernameText.text;
                        Screen.SetResolution(1600, 900, false);
                        SceneManager.LoadScene("MainScene");
                        //loginStateText.text = usernameText.text;
                        //GetUploadTime();
                        //getScore();
                        break;

                    case Constant.loginState.FAILED:
                        hintMessage.text = "用户名或密码错误。";
                        //loginStateText.text = "未登录";
                        break;
                }
            }
        }
    }
}
