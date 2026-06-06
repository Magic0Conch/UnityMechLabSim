using UnityEngine;
using UnityEngine.UI;
using Common;

public class RegistrerPanel : MonoBehaviour
{

    public GameObject loginpanel;
    public InputField usernameIF;
    public InputField passwordIF;
    public Dropdown usertypeIF;
    public Text hintMassage;

    private RegisterRequest registerRequest;
    private void Start()
    {
        registerRequest = GetComponent<RegisterRequest>();
    }
    //点击事件确认登陆
    public void OnRegisterButton()
    {
        hintMassage.text = "";
        registerRequest.Username = usernameIF.text;
        registerRequest.Password = passwordIF.text;
        registerRequest.Usertype = usertypeIF.value == 0 ? "student" : "teacher";
        registerRequest.DefaultRequse();
    }
    //点击事件返回
    public void OnBackButton()
    {
        loginpanel.SetActive(true);
        gameObject.SetActive(false);

    }

    public void OnRegisterResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.StudentSuccess)
        {
            hintMassage.text = "学生用户注册成功，请返回登陆";
        }
        else if(returnCode==ReturnCode.TeacherSuccess)
        {
            hintMassage.text = "教师用户注册成功，请返回登陆";
        }
        else if (returnCode == ReturnCode.Failed)
        {
            hintMassage.text = "所用的用户名已被注册，请更改用户名";
        }
    }

}