using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CloseDoor : MonoBehaviour, IPointerClickHandler
{
    public bool isOn = false;

    public Transform rotationCenter;
    public short mutiple = 1;
    public float rotationMaxAngle = 100f;
    public short maxClickCount = 1;

    int currentClickTimes = 0;
    float rotationAngle;
    float frameAngle;
    bool isRotating = false;

    const float rotateSpeed = 200f;

    IEnumerator BeRotating()
    {
        while (true)
        {
            frameAngle = 0.02f * rotateSpeed * mutiple;
            if (Mathf.Abs(rotationAngle) + Mathf.Abs(frameAngle) > rotationMaxAngle)
            {
                print(rotationAngle);
                isRotating = false;
                if (currentClickTimes >= maxClickCount)
                {
                    if (gameObject.name == "S1")
                    {
                        BasicData.levelSwitch[22] = true;
                    }
                    else if (gameObject.name == "S2")
                    {
                        BasicData.levelSwitch[23] = true;
                    }
                    else if (gameObject.name == "Door1")
                    {
                        BasicData.levelSwitch[24] = true;
                    }
                    //------------------------------- Add over events ---------------------------\\
                    Destroy(this);
                }
                yield break;
            }
            else
                yield return null; // 下一帧再执行后续代码
            rotationAngle += frameAngle;
            transform.RotateAround(rotationCenter.position, Vector3.up, -frameAngle);
        }
    }

    public void StartRotate()
    {
        rotationAngle = 0;
        StartCoroutine(BeRotating());
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isRotating && currentClickTimes < maxClickCount)
        {
            //if (!(BasicData.levelSwitch[18] && BasicData.levelSwitch[19] && BasicData.levelSwitch[20]))
            //{
            //    Instantiate(GameObject.Find("Canvas").transform.GetChild(11), GameObject.Find("Canvas").transform).gameObject.SetActive(true);
            //}
            if (BasicData.levelSwitch[18]&& BasicData.levelSwitch[19]&& BasicData.levelSwitch[20])
            {
                if(BasicData.levelSwitch[24])
                {
                    ++currentClickTimes;
                    StartRotate();
                }
                else if(gameObject.name == "Door1")
                {
                    ++currentClickTimes;
                    StartRotate();
                }
            }
        }

    }
}

