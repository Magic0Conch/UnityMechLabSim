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
    public string InitConnectUri = "http://49.233.160.65:8899/api/values";
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
        if (string.IsNullOrWhiteSpace(connectUri))
        {
            SetHint("请先填写服务器地址。");
            return;
        }
        string username = usernameText.text.Trim();
        if (username.Length < 6)
        {
            SetHint("用户名不少于6位。");
            return;
        }
        PlayerPrefs.SetString(PrefsUsername, username);
        PlayerPrefs.Save();
        SetHint("正在登录…");
        string password = passwordText != null ? (passwordText.text ?? "") : "";
        StartCoroutine(Login(connectUri, username, password));
    }

    void SetHint(string message)
    {
        Debug.Log("[LoginUI] " + message);
        if (hintMessage != null)
            hintMessage.text = message;
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
        InfoDic[Constant.propname.username] = username ?? "";
        InfoDic[Constant.propname.password] = password ?? "";
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        foreach (var item in InfoDic)
            formData.Add(new MultipartFormDataSection(item.Key, item.Value ?? ""));
        UnityWebRequest request = UnityWebRequest.Post(url, formData);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        string body = request.downloadHandler != null ? request.downloadHandler.text.Trim() : "";
        Debug.Log($"[LoginUI] POST {url} result={request.result} code={request.responseCode} body={body}");

#if UNITY_2020_2_OR_NEWER
        bool requestFailed = request.result != UnityWebRequest.Result.Success;
#else
        bool requestFailed = request.isHttpError || request.isNetworkError;
#endif
        if (requestFailed)
        {
            string err = string.IsNullOrEmpty(request.error) ? $"HTTP {request.responseCode}" : request.error;
            SetHint("网络错误: " + err);
            Assets.Message.MessageBox("无法连接服务器：\n" + err + "\n\n请确认地址为 http://IP:8899/api/values");
            yield break;
        }

        if (!Enum.TryParse(body, true, out Constant.loginState loginState))
        {
            SetHint("服务器返回异常: " + body);
            Assets.Message.MessageBox("登录失败，服务器返回：\n" + (string.IsNullOrEmpty(body) ? "(空)" : body));
            yield break;
        }

        switch (loginState)
        {
            case Constant.loginState.TEACHERSUCCESS:
                SetHint("登陆成功");
                Screen.SetResolution(1600, 900, false);
                SceneManager.LoadScene("Tscene");
                break;
            case Constant.loginState.STUDENTSUCCESS:
                SetHint("登陆成功");
                WebData.username = username;
                Screen.SetResolution(1600, 900, false);
                SceneManager.LoadScene("MainScene");
                break;
            case Constant.loginState.FAILED:
                SetHint("登录失败，请检查用户名或教师密码。");
                break;
        }
    }
}
