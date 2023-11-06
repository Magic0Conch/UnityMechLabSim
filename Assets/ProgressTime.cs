using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ProgressTime : MonoBehaviour
{
    public Text timeText;
    public GameObject nextGo;
    int totalTime = 600;
    void Update()
    {
        totalTime--;
        timeText.text = totalTime / 60 + ":" + totalTime % 60;
        if(totalTime==0)
        {
            gameObject.SetActive(false);
            BasicData.levelSwitch[28] = true;
            
            Assets.Message.MessageBox("加工结束");
            nextGo.SetActive(true);
        }
    }
}
