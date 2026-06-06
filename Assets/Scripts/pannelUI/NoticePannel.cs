using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticePannel : MonoBehaviour
{
    public GameObject[] theImage = new GameObject[14];
    public GameObject thePannel;
    public GameObject shell;

    private void OnEnable()
    {
        if (BasicData.levelSwitch[2])
            theImage[0].SetActive(true);
        if (BasicData.levelSwitch[3])
            theImage[1].SetActive(true);
        if (BasicData.levelSwitch[7])
            theImage[2].SetActive(true);
        if (BasicData.levelSwitch[8])
            theImage[3].SetActive(true);
        if (BasicData.levelSwitch[30])
            theImage[4].SetActive(true);
        if (BasicData.levelSwitch[5])
            theImage[5].SetActive(true);//放置工具电极
        if (BasicData.levelSwitch[13]&& BasicData.levelSwitch[12])
            theImage[6].SetActive(true);//调平工具电极
        if (BasicData.levelSwitch[17])
            theImage[7].SetActive(true);//检查是否被调平
        if (BasicData.levelSwitch[18])
            theImage[8].SetActive(true);
        if (BasicData.levelSwitch[20])
            theImage[9].SetActive(true);
        if (BasicData.levelSwitch[19])
            theImage[10].SetActive(true);
        if (Mathf.Abs(shell.transform.position.x + 0.6f)<0.5f&& Mathf.Abs(shell.transform.position.z - 0.35f) < 0.4f&&BasicData.levelSwitch[33])//
        {
            theImage[11].SetActive(true);

        }
        if (BasicData.levelSwitch[24])
            theImage[12].SetActive(true);
        if (BasicData.levelSwitch[26])
            theImage[13].SetActive(true);
        if (BasicData.levelSwitch[27])
            theImage[14].SetActive(true);
        if (BasicData.levelSwitch[29])
            theImage[15].SetActive(true);
    }
    public void close()
    {
        thePannel.SetActive(false);
    }
}
