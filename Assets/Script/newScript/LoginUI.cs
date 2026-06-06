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
    const string PrefsConnectUri = "connectUri";
    const string PrefsUsername = "loginUsername";

    public Text hintMessage;
    public InputField usernameText, passwordText, uriText;
    public string InitConnectUri = "http://localhost:8899/api/values";
    public string connectUri;

    void Start()
    {
        string savedUri = PlayerPrefs.GetString(PrefsConnectUri, "");
        if (string.IsNullOrEmpty(savedUri))
            savedUri = InitConnectUri;

        connectUri = WebData.connectUri = savedUri;
        uriText.text = savedUri;

        string savedUser = PlayerPrefs.GetString(PrefsUsername, "");
        if (!string.IsNullOrEmpty(savedUser))
            usernameText.text = savedUser;

        uriText.onEndEdit.AddListener(delegate { OnUriChanged(); });
    }

    private void Awake()
    {
        Screen.SetResolution(900, 600, false);
    }

    void SaveConnectUri(string uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
            return;
        connectUri = WebData.connectUri = uri.Trim();
        PlayerPrefs.SetString(PrefsConnectUri, connectUri);
        PlayerPrefs.Save();
    }

    public void Ok()
    {
        SaveConnectUri(uriText.text);
    }

    public void OnUriChanged()
    {
        SaveConnectUri(uriText.text);
    }

    public void SwitchOpen(GameObject go)
    {
        go.SetActive(!go.activeInHierarchy);
    }

    public void Login()
    {
        SaveConnectUri(uriText.text);
        string username = usernameText.text.Trim();
        if (username.Length < 6)
        {
            hintMessage.text = "用户名不少于6位。";
            return;
        }
        PlayerPrefs.SetString(PrefsUsername, username);
        PlayerPrefs.Save();
        StartCoroutine(Login(connectUri, username, passwordText.text));
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
                        hintMessage.text = "登陆成功";
                        Screen.SetResolution(1600, 900, false);
                        SceneManager.LoadScene("Tscene");
                        break;
                    case Constant.loginState.STUDENTSUCCESS:
                        hintMessage.text = "登陆成功";
                        WebData.username = username;
                        Screen.SetResolution(1600, 900, false);
                        SceneManager.LoadScene("MainScene");
                        break;

                    case Constant.loginState.FAILED:
                        hintMessage.text = "登录失败，请检查用户名或教师密码。";
                        break;
                }
            }
        }
    }
}
