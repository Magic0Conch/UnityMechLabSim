using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class RotateAroundOnePoint : MonoBehaviour, IPointerClickHandler
{
    public Transform rotationCenter;
    public short mutiple = 1;
    public float rotationMaxAngle = 100f;
    public short maxClickCount = 1;
    public Material higgLight;
    public Material normai;
    int currentClickTimes = 0;
    float rotationAngle;
    float frameAngle;
    bool isRotating = false;

    const float rotateSpeed = 200f;

    IEnumerator BeRotating()
    {
        if(higgLight!=null)
        {
            foreach(Renderer r in transform.GetComponentsInChildren<Renderer>())
            {
                r.material = higgLight;
            }
            if (gameObject.name == "螺旋1" && BasicData.levelSwitch[10])
            {
                Animator animator = GameObject.Find("clock2").transform.GetChild(10).GetChild(2).GetComponent<Animator>();
                animator.enabled = true;
            }
            else if (gameObject.name == "螺旋2")
            {
                Animator animator = GameObject.Find("clock3").transform.GetChild(10).GetChild(2).GetComponent<Animator>();
                animator.enabled = true;
            }
            else if (gameObject.name == "螺旋3")
            {
                Animator animator = GameObject.Find("clock3").transform.GetChild(10).GetChild(2).GetComponent<Animator>();
                animator.enabled = true;
            }


        }
        while (true)
        {
            frameAngle = 0.02f * rotateSpeed*mutiple;
            if(Mathf.Abs(rotationAngle)+ Mathf.Abs(frameAngle) >rotationMaxAngle)
            {
                print(rotationAngle);

                //if (BasicData.levelSwitch[10] || BasicData.levelSwitch[14])
                //    transform.RotateAround(transform.position, transform.forward, Mathf.Abs(rotationMaxAngle) - Mathf.Abs(rotationAngle));
                //else
                //    transform.RotateAround(rotationCenter.position, Vector3.up, -(Mathf.Abs(rotationMaxAngle) - Mathf.Abs(rotationAngle)));

                isRotating = false;
                if (currentClickTimes >= maxClickCount)
                {
                    if(gameObject.name=="S1")
                    {
                        BasicData.levelSwitch[0] = true;
                    }
                    else if(gameObject.name == "S2")
                    {
                        BasicData.levelSwitch[1] = true;
                    }
                    else if(gameObject.name=="Door1")
                    {
                        BasicData.levelSwitch[2] = true;
                    }
                    else if (gameObject.name == "螺旋1"&&BasicData.levelSwitch[10])
                    {
                        BasicData.levelSwitch[11] = true;
                        GameObject.Find("外壳").transform.GetComponent<MoveMachine>().rotateSpeed = 10f;
                        Animator animator = GameObject.Find("clock2").transform.GetChild(10).GetChild(2).GetComponent<Animator>();
                        animator.enabled = false;
                    }
                    else if (gameObject.name == "螺旋2")
                    {
                        Animator animator = GameObject.Find("clock3").transform.GetChild(10).GetChild(2).GetComponent<Animator>();
                        animator.enabled = false;
                        BasicData.levelSwitch[12] = true;
                    }
                    else if (gameObject.name == "螺旋3" )
                    {
                        Animator animator = GameObject.Find("clock3").transform.GetChild(10).GetChild(2).GetComponent<Animator>();
                        animator.enabled = false;
                        BasicData.levelSwitch[13] = true;
                    }
                    //------------------------------- Add over events ---------------------------\\
                    if (normai!=null)
                    {
                        foreach (Renderer r in transform.GetComponentsInChildren<Renderer>())
                        {
                            r.material = normai;
                        }

                    }
                    Destroy(this);
                }
                yield break;
            }
            else
                yield return null; // 下一帧再执行后续代码
            rotationAngle += frameAngle;
            if(BasicData.levelSwitch[10]|| BasicData.levelSwitch[14]||gameObject.name=="螺旋2"|| gameObject.name == "螺旋3" || gameObject.name == "螺旋1")
                transform.RotateAround(transform.position, transform.forward, frameAngle);
            else
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
        if(!isRotating&&currentClickTimes<maxClickCount)
        {
            Debug.Log(gameObject.name);
            if(gameObject.name=="Door1")
                if (!BasicData.levelSwitch[0] || !BasicData.levelSwitch[1])     return;
            if (gameObject.name == "螺旋1")
            {
                if (!BasicData.levelSwitch[10] || GameObject.Find("clock2") == null)
                    return;
            }

            if (gameObject.name == "螺旋2")
            {
                if (BasicData.levelSwitch[12]||!BasicData.levelSwitch[31]||GameObject.Find("clock3")==null)
                {

                    return;
                }

            }
            if (gameObject.name == "螺旋3")
            {
                if (BasicData.levelSwitch[13] || !BasicData.levelSwitch[31] || GameObject.Find("clock3") == null)
                {
                    
                    return;
                }

            }

            ++currentClickTimes;
            StartRotate();
        }
        
    }
}

