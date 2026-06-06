using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public void PersonalInfo()
    {
        GameObject pannel = GameObject.Find("Canvas").transform.GetChild(4).gameObject;
        pannel.SetActive(true);
        Text[] infos = transform.GetComponentsInChildren<Text>();
        Text[] pannelinfo = pannel.transform.GetComponentsInChildren<Text>();
        InputField[] inputField = pannel.transform.GetComponentsInChildren<InputField>();
        for (int i = 0;i<5;i++)
        {
            pannelinfo[i].text = infos[i].text;
            if (i == 0) continue;
            inputField[i-1].text = infos[i].text;
        }
    }
}
