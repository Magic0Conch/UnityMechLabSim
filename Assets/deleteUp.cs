using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class deleteUp : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button.ToString() == "Right")
        {
            OnSure action = delegate ()
            {
                GameObject.Find("外壳").transform.GetComponent<MoveMachine>().initialStatu();
                GameObject go = GameObject.Find("上部");
                go.transform.GetChild(0).gameObject.SetActive(false);
                BasicData.levelSwitch[5] = false;
            };
            Assets.Message.MessageBox("确定移除该工具吗？",1,action,null);

            //int index = Assets.Message.MessageBox(IntPtr.Zero, "确定移除该工具吗？", "注意", 1);
//            if (index != 1) return;
        }
    }
}
