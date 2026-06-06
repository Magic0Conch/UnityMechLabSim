using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolUiController : MonoBehaviour
{
    public GameObject[] go = new GameObject[5];
    GameObject dragGameobject;
    Vector2 startPos;
    int chooseIndex=9;
    Ray ray;
    RaycastHit hit;
    public void Item0DragBegin()
    {
        dragGameobject = go[0];
        chooseIndex = 0;
        startPos = dragGameobject.GetComponent<RectTransform>().position;
        dragGameobject.GetComponent<RectTransform>().anchoredPosition=  new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        StartCoroutine(OnMouseDown());
    }
    public void Item1DragBegin()
    {
        dragGameobject = go[1];
        chooseIndex = 1;
        startPos = dragGameobject.GetComponent<RectTransform>().position;
        dragGameobject.GetComponent<RectTransform>().anchoredPosition=  new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        StartCoroutine(OnMouseDown());
    }
    public void Item2DragBegin()
    {
        dragGameobject = go[2];
        chooseIndex = 2;
        startPos = dragGameobject.GetComponent<RectTransform>().position;
        dragGameobject.GetComponent<RectTransform>().anchoredPosition=  new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        StartCoroutine(OnMouseDown());
    }
    public void Item3DragBegin()
    {
        dragGameobject = go[3];
        chooseIndex = 3;
        startPos = dragGameobject.GetComponent<RectTransform>().position;
        dragGameobject.GetComponent<RectTransform>().anchoredPosition=  new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        StartCoroutine(OnMouseDown());
    }
    public void Item4DragBegin()
    {
        dragGameobject = go[4];
        chooseIndex = 4;
        startPos = dragGameobject.GetComponent<RectTransform>().position;
        dragGameobject.GetComponent<RectTransform>().anchoredPosition=  new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        StartCoroutine(OnMouseDown());
    }
    public void ItemDragEnd()
    {
        StopAllCoroutines();
        dragGameobject.GetComponent<RectTransform>().position = startPos;
        if(Physics.Raycast(ray, out hit))
        {
                    print(hit.collider.gameObject.name);
            switch (chooseIndex)
            {
                case 0:
                    if (BasicData.ExistClock) break;
                    if (hit.collider.gameObject.name == "Item0"||hit.collider.gameObject.name == "固定器")
                    {
                        if (!BasicData.levelSwitch[3])
                        {
                            Instantiate(GameObject.Find("Canvas").transform.GetChild(11), GameObject.Find("Canvas").transform).gameObject.SetActive(true);
                            return;
                        }
                        print("百分表放到下面4开启");
                        GameObject.Find("外壳").GetComponent<MoveMachine>().initialStatu();
                        GameObject.Find("固定器").transform.GetChild(0).gameObject.SetActive(true);
                        //GameObject.Find("Back").transform.GetChild(0).gameObject.SetActive(true);
                        BasicData.levelSwitch[4] = true;
                        BasicData.ExistClock = true;

                    }
                    else if(hit.collider.gameObject.name== "上极板d"&&!BasicData.levelSwitch[16])
                    {
                        print(hit.collider.gameObject.name + "!");
                        GameObject.Find("外壳").transform.GetChild(1).gameObject.SetActive(true);
                        BasicData.levelSwitch[10] = true;
                        //GameObject.Find("Back").transform.GetChild(0).gameObject.SetActive(true);
                        GameObject.Find("外壳").GetComponent<MoveMachine>().initialStatu();
                        BasicData.ExistClock = true;
                    }
                    else if (hit.collider.gameObject.name == "上极板d" && BasicData.levelSwitch[16])
                    {
                        GameObject.Find("外壳").transform.GetChild(2).gameObject.SetActive(true);
                        //GameObject.Find("Back").transform.GetChild(0).gameObject.SetActive(true);
                        BasicData.levelSwitch[14] = true;
                        BasicData.levelSwitch[31] = true;
                        GameObject.Find("外壳").GetComponent<MoveMachine>().initialStatu();
                        BasicData.ExistClock = true;
                    }
                    break;

                case 1:
                    if (hit.collider.gameObject.name == "上部" )
                    {
                        if (!BasicData.levelSwitch[30])
                        {
                            Instantiate(GameObject.Find("Canvas").transform.GetChild(11), GameObject.Find("Canvas").transform).gameObject.SetActive(true);
                            return;//没有锁紧无法执行下一步
                        }
                        GameObject.Find("外壳").transform.GetComponent<MoveMachine>().initialStatu();
                        hit.collider.transform.GetChild(0).gameObject.SetActive(true);
                        BasicData.levelSwitch[5] = true;
                    }
                    break;


                case 2:
                    
                    break;
                case 3:
                    if (hit.collider.gameObject.name == "plat")
                    {
                        print("hithit");
                        if(!BasicData.levelSwitch[2])
                        {
                            print(GameObject.Find("Canvas").transform.GetChild(11));
                            Instantiate(GameObject.Find("Canvas").transform.GetChild(11), GameObject.Find("Canvas").transform).gameObject.SetActive(true);
                            return;
                        }
                        hit.collider.transform.GetChild(0).gameObject.SetActive(true);
                        BasicData.levelSwitch[3] = true;
                    }
                    break;
                case 4:
                    if(hit.collider.gameObject.name=="Item0")
                    {
                        
                        if (GameObject.Find("百分表带支架")==null)
                        {
                            Instantiate(GameObject.Find("Canvas").transform.GetChild(11), GameObject.Find("Canvas").transform).gameObject.SetActive(true);
                            return;
                        }

                        StartCoroutine(Hiting());
                        GameObject.Find("外壳").transform.GetComponent<MoveMachine>().initialStatu();
                        GameObject tmp = GameObject.Find("WorldCenter").transform.GetChild(0).gameObject;
                        tmp.SetActive(true);
                    }
                    else if(hit.collider.gameObject.name == "plat")
                    {
                        if (!BasicData.levelSwitch[8])
                        {
                            Instantiate(GameObject.Find("Canvas").transform.GetChild(11), GameObject.Find("Canvas").transform).gameObject.SetActive(true);
                            return;
                        }
                        print(30);
                        BasicData.levelSwitch[30] = true;
                        GameObject.Find("外壳").transform.GetComponent<MoveMachine>().initialStatu();
                        GameObject go=null;
                        try
                        {
                         go = GameObject.Find("WorldCenter").transform.GetChild(1).gameObject;

                        }
                        catch (Exception e) { }
                        if (go != null)
                            go.SetActive(true);
                        if (BasicData.levelSwitch[9]) return;
                    }
                    break;
            }
        }
    }

    IEnumerator Hiting()
    {
        Animator animator = GameObject.Find("固定器").transform.GetChild(0).GetChild(9).GetChild(2).GetComponent<Animator>();
        animator.enabled = true;
        GameObject tmp = GameObject.Find("WorldCenter").transform.GetChild(0).gameObject;
        yield return new WaitForSeconds(1.5f);
        tmp.SetActive(false);
        GameObject.Find("Item0").gameObject.transform.localEulerAngles= Vector3.zero;
        BasicData.levelSwitch[7] = true;
        GameObject.Find("外壳").transform.GetComponent<MoveMachine>().rotateSpeed = 10f;
        tmp.SetActive(false);
        animator.enabled = false;
    }

    IEnumerator OnMouseDown()
    {
        while(true)
        {
            dragGameobject.GetComponent<RectTransform>().position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            yield return null;
        }
    }
}