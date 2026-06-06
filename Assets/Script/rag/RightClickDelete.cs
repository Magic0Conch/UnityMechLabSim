using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClickDelete : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button.ToString() == "Right")
        {
            OnSure os = delegate ()
            {
                BasicData.ExistClock = false;
                if (BasicData.levelSwitch[4])
                {
                    GameObject.Find("百分表带支架").SetActive(false);
                    BasicData.levelSwitch[4] = false;
                    BasicData.levelSwitch[6] = true;
                    if (BasicData.levelSwitch[7])
                        BasicData.levelSwitch[8] = true;
                }
                if (BasicData.levelSwitch[10])
                {
                    GameObject.Find("clock2").SetActive(false);
                    BasicData.levelSwitch[15] = true;
                    if (BasicData.levelSwitch[11])
                    {
                        BasicData.levelSwitch[16] = true;
                        BasicData.levelSwitch[10] = false;
                    }
                }
                else if (BasicData.levelSwitch[14])
                {
                    GameObject.Find("clock3").SetActive(false);
                    BasicData.levelSwitch[14] = false;
                    BasicData.levelSwitch[17] = true;
                }

            };
            Assets.Message.MessageBox("确定移除该工具吗？", 1, os);

            //transform.parent.gameObject.SetActive(false);
        }       
    }
}
