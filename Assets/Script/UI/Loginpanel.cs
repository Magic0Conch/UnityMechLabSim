using UnityEngine;
using UnityEngine.UI;
using Common;
using UnityEngine.SceneManagement;

public class Loginpanel : MonoBehaviour
{

    public GameObject RegisterPanel;//注册面板
    public InputField username;//UI界面输入的用户名
    public InputField password;//UI界面输入的密码
    private LoginRequest loginRequest;//提示信息
    public GameObject Pengine;


    public Text hintMessage;
    private void Start()
    {
        Screen.SetResolution(900, 600, false);
        loginRequest = GetComponent<LoginRequest>();
    }
    //点击登陆按钮
    public void OnLoginButton()
    {
        hintMessage.text = "";
        loginRequest.Username = username.text;
        loginRequest.Password = password.text;
        loginRequest.DefaultRequse();
    }
    //点击注册按钮跳转到注册界面
    public void OnRegisterButton()
    {
        gameObject.SetActive(false);
        RegisterPanel.SetActive(true);
    }

    public void Offline()
    {
        Screen.SetResolution(1600, 900, false);
        Destroy(Pengine);
        BasicData.isConnected = false;
        SceneManager.LoadScene("SampleScene");
    }

    //登陆操作，如果验证成功登陆，失败提示用户或密码错误
    public void OnLoginResponse(ReturnCode returnCode)
    {
        BasicData.username = username.text;
        if (returnCode == ReturnCode.TeacherSuccess)
        {
            //验证成功，跳转到下一个场景
            hintMessage.text = "教师登录成功";
            Screen.SetResolution(1600, 900, false);
            SceneManager.LoadScene("teacherScene");
        }
        else if(returnCode==ReturnCode.StudentSuccess)
        {
            hintMessage.text = "学生登录成功";
            Screen.SetResolution(1600, 900, false);
            SceneManager.LoadScene("previewScene");
        }
        else if (returnCode == ReturnCode.Failed)
        {
            hintMessage.text = "用户名或密码错误";
        }
    }
}
