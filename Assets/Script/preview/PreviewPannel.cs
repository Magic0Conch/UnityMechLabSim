using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Script;

public class PreviewPannel : MonoBehaviour
{
    public Text UsernameText;
    public GameObject UIPannel;
    public GameObject VideoPlayer;
    public GameObject exitButton;
    public GameObject Sheet;
    public GameObject[] marks;
    public GameObject button4;

    private SubmitRequest submitRequest;
    private GetfileRequest getfileRequest;

    void Start()
    {
        //-387 225
        GameObject peng = GameObject.Find("PhotonEngine");
        RectTransform rt1 = peng.transform.GetChild(0).GetComponent<RectTransform>();//connected status
        rt1.localPosition = new Vector2(-387, 225);
        UsernameText.text = BasicData.username;
        submitRequest = GetComponent<SubmitRequest>();
        getfileRequest = GetComponent<GetfileRequest>();
    }

    public void Button1()
    {
       
        BasicData.levelSwitch[31] = true;
        marks[0].SetActive(true);

        if (BasicData.levelSwitch[31] && BasicData.levelSwitch[32] && BasicData.levelSwitch[33])
            button4.GetComponent<UnityEngine.UI.Button>().interactable = true;
    }
    public void Button2()
    {
        Sheet.SetActive(true);
    }
    public void Button3()
    {
        UIPannel.SetActive(false);
        VideoPlayer.SetActive(true);
        exitButton.SetActive(true);
    }
    public void Button4()
    {
        SceneManager.LoadScene("SampleScene");

    }
    public void endPlayVideo()
    {
        VideoPlayer.SetActive(false);
        UIPannel.SetActive(true);
        exitButton.SetActive(false);
        marks[2].SetActive(true);
        BasicData.levelSwitch[33] = true;
        if (BasicData.levelSwitch[31] && BasicData.levelSwitch[32] && BasicData.levelSwitch[33])
            button4.GetComponent<UnityEngine.UI.Button>().interactable = true;
    }

    public void onSubmitSheet()
    {
        int score = 0;
        string record = "";
        ToggleGroup[] toggleGroups = Sheet.GetComponentsInChildren<ToggleGroup>();
        for (int i = 0; i < toggleGroups.Length; i++)
        {
            Toggle[] toggles = toggleGroups[i].GetComponentsInChildren<Toggle>();
            bool flag = true;
            for (int j = 0; j < 4; j++)
            {
                if (toggles[j].isOn && toggles[j].gameObject.name == "a")
                {
                    score += 5;
                    record += "√";
                    flag = false;
                }
            }
            if (flag) record += "×";
        }
        BasicData.Scores = score;
        BasicData.levelSwitch[32] = true;
        marks[1].SetActive(true);
        submitRequest.Requse(record);
        UIPannel.SetActive(true);
        Sheet.SetActive(false);
        if (BasicData.levelSwitch[31] && BasicData.levelSwitch[32] && BasicData.levelSwitch[33])
            button4.GetComponent<UnityEngine.UI.Button>().interactable = true;

    }
}
