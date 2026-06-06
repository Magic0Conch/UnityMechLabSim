using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMachine : MonoBehaviour
{
    [HideInInspector]
    public float rotateSpeed = 450f;
    [HideInInspector]
    public float movedDistance;
    [HideInInspector]
    public bool isDistanceMove;
    [HideInInspector]
    public float maxMoveDistance;
    [HideInInspector]
    public bool isAutoToolSetting;
    [HideInInspector]
    public bool isAutoToolSettingSuccess;
    public PannelUI pannelUI;

    float horizonSpeed = 2f;
    float verticalSpeed = 0.2f;
    float fowardSpeed  = 2f;
    const float jmYmax = 11.5f;
    const float zmax = -5.6f;//x!
    const float zmin = 3.98f;//x-!
    const float xmax = -3;//limit z+
    const float xmin = 1.7f;//limit z-
    const float frameInterval = 0.02f;
    RectTransform rect;
    Transform jointedMachine;
    Transform watchHand;
    Transform rorateCenter;
    Transform shell;
    Transform coliderItem;

    Vector3 initRectlocalScale;
    Vector3 initJointedMachinePos;
    Vector3 transformPos;
    float frameAngle;
    float floatRange;

    public bool freezeXp;
    public bool freezeXs;
    public bool freezeYp;
    public bool freezeYs;
    public bool freezeZp;
    public bool freezeZs;
    int freezeIndex;
    void adjustPos()
    {
        coliderItem = GameObject.Find("上部").transform.GetChild(0).GetComponent<Transform>();
        if (coliderItem == null) return;
        //定义一条射线，起点为Vector3.zero终点为物体坐标
        Ray ray = new Ray(Vector3.zero, transform.position);
        Ray[] rays = new Ray[6];
        print(coliderItem.name);
        Vector3 ori = coliderItem.position;
        rays[0] = new Ray(ori, coliderItem.forward);
        rays[1] = new Ray(ori, coliderItem.forward*-1);//0.125
        rays[2] = new Ray(ori, coliderItem.right*-1);
        rays[3] = new Ray(ori, coliderItem.right);
        rays[4] = new Ray(ori, coliderItem.up);
        rays[5] = new Ray(ori, coliderItem.up*-1);
        for(int i = 0;i<6;i++)
        {
            //定义一个光线投射碰撞
            RaycastHit hit;
            //发射射线长度为100
            int layerMask = 1 << 2;
            Physics.Raycast(rays[i], out hit, 1000,~layerMask);
            //print( hit.collider.name);
            print(i+":"+Vector3.Distance(hit.point,ori)+hit.collider.name);
            //在Scene中生成这条射线，起点为射线的起点，终点为射线与物体的碰撞点
            Debug.DrawLine(ori, hit.point,Color.red);
        }
    }
    private void Update()
    {
        
       // adjustPos();
    }
    private void Start()
    {
        freezeIndex = -1;
        jointedMachine = GameObject.Find("上部").GetComponent<Transform>();
        rect = GameObject.Find("软管").GetComponent<RectTransform>();
        initRectlocalScale = rect.localScale;
        initJointedMachinePos = jointedMachine.transform.position;
        transformPos = transform.position;
        shell = GameObject.Find("外壳").GetComponent<Transform>();
        isDistanceMove = false;
        isAutoToolSetting = false;
        isAutoToolSettingSuccess = false;

    freezeXp = false;
        freezeXs = false;
        freezeYp = false;
        freezeYs = false;
        freezeZp = false;
        freezeZs = false;
    }


    void EndPreciseMove()
    {
        isDistanceMove = false;
        movedDistance = 0;
    }

    void unlockAll()
    {
        freezeXp = false;
        freezeYp = false;
        freezeZp = false;
        freezeXs = false;
        freezeYs = false;
        freezeZs = false;
        switch(freezeIndex)
        {
            case 0:
                freezeXp = true;
                break;
            case 1:
                freezeXs = true;
                break;
            case 3:
                freezeYs = true;
                break;
            case 4:
                freezeZp = true;
                break;
            case 5:
                freezeZs = true;
                break;
        }
    }

    void getClock()
    {
        if (BasicData.levelSwitch[4])
        {
            watchHand = GameObject.Find("表针dy").GetComponent<Transform>();
            rorateCenter = GameObject.Find("ClockCenter").GetComponent<Transform>();
        }
        else if(BasicData.levelSwitch[10])
        {
            GameObject goo = GameObject.Find("表针dy2");
            if (goo == null)
            {
                Instantiate(GameObject.Find("Canvas").transform.GetChild(11), GameObject.Find("Canvas").transform).gameObject.SetActive(true);
                return;
            }
            watchHand = goo.transform;
            rorateCenter = GameObject.Find("ClockCenter2").GetComponent<Transform>();
            if(!BasicData.levelSwitch[11])
            rotateSpeed = 450f;
        }
        else if (BasicData.levelSwitch[14])
        {
            watchHand = GameObject.Find("表针dy3").GetComponent<Transform>();
            rorateCenter = GameObject.Find("ClockCenter3").GetComponent<Transform>();
            rotateSpeed = 450f;
        }

    }

    public void initialStatu()
    {
        BasicData.iscol = false;
        rect.localScale = initRectlocalScale;
        jointedMachine.transform.position = initJointedMachinePos;
        transform.position = transformPos;
    }

    public IEnumerator Yp()
    {
        while(true)
        {
            if (jointedMachine.position.y > 11.5f|| BasicData.levelSwitch[4] || BasicData.levelSwitch[10] || BasicData.levelSwitch[14])
            {
                EndPreciseMove();
                break;
            }
            if (freezeYp)
            {
                if (isAutoToolSetting&&BasicData.colZ)
                {
                    isAutoToolSetting = false;
                    isAutoToolSettingSuccess = true;
                    pannelUI.ResetCoordinates();
                    BasicData.levelSwitch[19] = true;
                }
                EndPreciseMove();
                break;
            }
            float frameDist = frameInterval * verticalSpeed * BasicData.speedMutiple;
            if(isDistanceMove)//{
            {
                bool flag = false;
                if(movedDistance+ frameDist * 0.18f>=maxMoveDistance*2)
                {
                    float nowMoveDistance = (maxMoveDistance * 2 - movedDistance);
                    rect.localScale -= Vector3.forward * nowMoveDistance /0.18f;
                    jointedMachine.position += Vector3.up * nowMoveDistance;
                    EndPreciseMove();
                    flag = true;
                }
                if (flag) break;
                movedDistance += frameDist * 0.18f;
            }//}
            rect.localScale -= Vector3.forward * frameDist;
            jointedMachine.position += Vector3.up * frameDist*0.18f;
            freezeIndex = 2;
            yield return null;
           
        }
    }
    public IEnumerator Ys()
    {
        while(true)
        {
            if (freezeYs)
            {
                if(isAutoToolSetting&& BasicData.colZ)
                {
                    isAutoToolSetting = false;
                    isAutoToolSettingSuccess = true;
                    pannelUI.ResetCoordinates();
                    BasicData.levelSwitch[19] = true;
                }
                EndPreciseMove();
                break;
            }
            if (BasicData.levelSwitch[4] || BasicData. banYMove || BasicData.levelSwitch[10] || BasicData.levelSwitch[14])
            {
                EndPreciseMove();
                break;
            }
            freezeYp = false;
            float frameDist = frameInterval * verticalSpeed * BasicData.speedMutiple;
            if (isDistanceMove)
            {
                bool flag = false;
                if (movedDistance + frameDist * 0.17f >= maxMoveDistance*2)
                {
                    float nowMoveDistance = (maxMoveDistance * 2 - movedDistance);
                    rect.localScale += Vector3.forward * nowMoveDistance / 0.17f;
                    jointedMachine.position += Vector3.down * nowMoveDistance;
                    EndPreciseMove();
                    flag = true;
                }
                if (flag) break;
                movedDistance += frameDist * 0.17f;
            }
            rect.localScale += Vector3.forward * frameDist;
            jointedMachine.position += Vector3.down * frameDist * 0.17f;
            if (BasicData.iscol)
            {
                rect.localScale -= Vector3.forward * frameDist;
                jointedMachine.position -= Vector3.down * frameDist * 0.17f;
                print(freezeIndex);
                freezeYs = true;
                freezeIndex = 3;
            }
            yield return null;
        }
    }
    public IEnumerator Xp()
    {
        while(true)
        {
            if (transform.position.z < zmax ||BasicData.levelSwitch[10])
            {
                EndPreciseMove();
                break;
            }
            if(freezeXp)
            {
                if (isAutoToolSetting&& BasicData.colZ)
                {
                    isAutoToolSetting = false;
                    isAutoToolSettingSuccess = true;
                    pannelUI.ResetCoordinates();
                    BasicData.levelSwitch[18] = true;
                }
                EndPreciseMove();
                break;
            }
            if (BasicData.iscol && freezeIndex == 1)
            {

            }
            else if (BasicData.iscol && freezeIndex != 1)
            {
                freezeIndex = 0;
                freezeXp = true;
            }
            else if (!BasicData.iscol)
                freezeIndex = freezeIndex == 1 ? 9 : freezeIndex;
            unlockAll();
            if (BasicData.levelSwitch[4])
            {
                if (shell.position.z < -0.4f)
                    break;
                frameAngle = rotateSpeed * frameInterval * BasicData.speedMutiple;
                watchHand.RotateAround(rorateCenter.position, watchHand.up, -frameAngle);
            }
            else if(BasicData.levelSwitch[14])
            {
                if (shell.position.z < -1.62f) break;
                if (!BasicData.levelSwitch[13])
                    frameAngle = rotateSpeed * frameInterval * BasicData.speedMutiple;
                else
                    frameAngle = rotateSpeed * frameInterval / 50 * BasicData.speedMutiple;
                
                watchHand.RotateAround(rorateCenter.position, watchHand.up, -frameAngle);
            }
            float frameDist = frameInterval * verticalSpeed * BasicData.speedMutiple;
            if (isDistanceMove)//{
            {
                bool flag = false;
                if (movedDistance + frameDist >= maxMoveDistance*2)
                {
                    float nowMoveDistance = (maxMoveDistance * 2 - movedDistance);
                    transform.position += Vector3.back * nowMoveDistance;
                    EndPreciseMove();
                    flag = true;
                }
                else
                    movedDistance += frameDist;
                if (flag) break;
            }//}

            transform.position += Vector3.back * frameDist;

            if (BasicData.iscol&&freezeIndex!=1 && freezeIndex != 9)
            {
                transform.position -= Vector3.back * frameDist;
                freezeIndex = 0;
                freezeXp = true;
                
            }
            yield return null;
        }
    }
    public IEnumerator Xs()
    {
        while(true)
        {
            if (transform.position.z > zmin || BasicData.levelSwitch[10])
            {
                EndPreciseMove();
                break;
            }
            if (freezeXs)
            {
                if (isAutoToolSetting && BasicData.colZ)
                {
                    isAutoToolSetting = false;
                    isAutoToolSettingSuccess = true;
                    pannelUI.ResetCoordinates();
                    BasicData.levelSwitch[18] = true;
                }
                EndPreciseMove();
                break;
            }
            if (BasicData.iscol && freezeIndex == 0)
            {

            }
            else if (BasicData.iscol && freezeIndex != 0)
            {
                freezeIndex = 1;
                freezeXs = true;
            }
            else if (!BasicData.iscol)
                freezeIndex = freezeIndex == 0 ? 9 : freezeIndex;
            unlockAll();
            if (BasicData.levelSwitch[4])
            {
                if (shell.position.z > 1.55f)
                    break;
                frameAngle = rotateSpeed*Time.deltaTime * BasicData.speedMutiple;
                watchHand.RotateAround(rorateCenter.position,watchHand.up,frameAngle);
            }
            else if (BasicData.levelSwitch[14])
            {
                if (shell.position.z > 0.072) break;
                if (!BasicData.levelSwitch[13])
                    frameAngle = rotateSpeed * Time.deltaTime * BasicData.speedMutiple;
                else
                    frameAngle = rotateSpeed * Time.deltaTime / 50 * BasicData.speedMutiple;
                
                watchHand.RotateAround(rorateCenter.position, watchHand.up, frameAngle);
            }
            float frameDist = frameInterval * verticalSpeed * BasicData.speedMutiple;
            if (isDistanceMove)//{
            {
                bool flag = false;
                if (movedDistance + frameDist >= maxMoveDistance*2)
                {
                    float nowMoveDistance = (maxMoveDistance *2- movedDistance);
                    transform.position += Vector3.forward * nowMoveDistance;
                    EndPreciseMove();
                    flag = true;
                }
                if (flag) break;
                movedDistance += frameDist;
            }//}
            transform.position += Vector3.forward * frameDist;

            if (BasicData.iscol&&freezeIndex != 0 && freezeIndex != 9)
            {
                print(freezeIndex);
                freezeXs = true;
                transform.position -= Vector3.forward * frameDist;
                freezeIndex = 1;

            }
            yield return null;
        }
    }
    public IEnumerator Zp()
    {
        while(true)
        {
            if (transform.position.x < xmax )
            {
                EndPreciseMove();
                break;
            }
            if (freezeZp)
            {
                if (isAutoToolSetting&&BasicData.colZ)
                {
                    isAutoToolSetting = false;
                    isAutoToolSettingSuccess = true;
                    BasicData.levelSwitch[20] = true;
                    pannelUI.ResetCoordinates();
                }
                EndPreciseMove();
                break;
            }
            if (BasicData.levelSwitch[4])
            {
                EndPreciseMove();
                break;
            }
            if(BasicData.levelSwitch[10])
            {
                if (shell.position.x < -0.125f)
                    break;
                frameAngle = rotateSpeed* frameInterval * BasicData.speedMutiple;
                watchHand.RotateAround(rorateCenter.position,watchHand.up,frameAngle);
            }
            else if (BasicData.levelSwitch[14])
            {
                if (shell.position.x < -0.32f) break;
                if(!BasicData.levelSwitch[12])
                    frameAngle = rotateSpeed * frameInterval * BasicData.speedMutiple;
                else
                    frameAngle = rotateSpeed * frameInterval / 45 * BasicData.speedMutiple;

                watchHand.RotateAround(rorateCenter.position, watchHand.up, frameAngle);
            }
            
            if (BasicData.iscol && freezeIndex == 5)
            {

            }
            else if (BasicData.iscol && freezeIndex != 5)
            {
                freezeIndex = 4;
                freezeZp = true;
            }
            else if (!BasicData.iscol)
                freezeIndex = freezeIndex == 5 ? 9 : freezeIndex;
            unlockAll();
            float frameDist = frameInterval * verticalSpeed * BasicData.speedMutiple;
            if (isDistanceMove)//{
            {
                bool flag = false;
                if (movedDistance + frameDist >= maxMoveDistance*2)
                {
                    float nowMoveDistance = (maxMoveDistance * 2 - movedDistance);
                    transform.position += Vector3.left * nowMoveDistance;
                    EndPreciseMove();
                    flag = true;
                }
                if (flag) break;
                movedDistance += frameDist;
            }//}
            transform.position += Vector3.left * frameDist;

            if (BasicData.iscol&&freezeIndex != 5 && freezeIndex != 9)
            {
                print(freezeIndex);
                freezeZp = true;
                transform.position -= Vector3.left * frameDist;
                freezeIndex = 4;
            }
            yield return null;
        }
    }
    public IEnumerator Zs()
    {
        while(true)
        {
            if (transform.position.x > xmin)
            {
                EndPreciseMove();
                break;
            }
            if (freezeZs)
            {
                if (isAutoToolSetting&&BasicData.colZ)
                {
                    isAutoToolSetting = false;
                    isAutoToolSettingSuccess = true;
                    pannelUI.ResetCoordinates();
                    BasicData.levelSwitch[20] = true;
                }
                EndPreciseMove();
                break;
            }
            if (BasicData.levelSwitch[4])
            {
                EndPreciseMove();
                break;
            }
            if (BasicData.levelSwitch[10])
            {
                if (shell.position.x > 1.09f)
                    break;
                frameAngle = rotateSpeed * frameInterval;
                watchHand.RotateAround(rorateCenter.position, watchHand.up, -frameAngle);
            }
            else if (BasicData.levelSwitch[14])
            {
                if (shell.position.x > 0.08) break;
                if (!BasicData.levelSwitch[12])
                    frameAngle = rotateSpeed * frameInterval;
                else
                    frameAngle = rotateSpeed * frameInterval / 45;
                watchHand.RotateAround(rorateCenter.position, watchHand.up, frameAngle);
            }

            if (BasicData.iscol&& freezeIndex == 4)
            {
                
            }
            else if(BasicData.iscol && freezeIndex != 4)
            {
                freezeIndex = 5;
                freezeZs = true;
            }
            else if(!BasicData.iscol)
                freezeIndex = freezeIndex == 4 ?9 : freezeIndex;
            unlockAll();
            float frameDist = frameInterval * verticalSpeed * BasicData.speedMutiple;
            if (isDistanceMove)//{
            {
                bool flag = false;
                if (movedDistance + frameDist >= maxMoveDistance*2)
                {
                    float nowMoveDistance = (maxMoveDistance *2 - movedDistance);
                    transform.position += Vector3.right * nowMoveDistance;
                    EndPreciseMove();
                    flag = true;
                }
                if (flag) break;
                movedDistance += frameDist;
            }//}
            transform.position += Vector3.right * frameDist;
            if (BasicData.iscol&& freezeIndex != 4 && freezeIndex != 9)
            {
                transform.position -= Vector3.right * frameDist;
                freezeZs = true;
                print(freezeIndex);
                freezeIndex = 5;
            }
            yield return null;
        }
    }
    public void onClickXp()
    {
        //if (!BasicData.isfixed && BasicData.levelSwitch[5]) drop();
        if (isDistanceMove) return;
        unlockAll();
        if(!isAutoToolSetting)
            getClock();
        if (BasicData.levelSwitch[4])
        {
            watchHand = GameObject.Find("表针dy").GetComponent<Transform>();
            rorateCenter = GameObject.Find("ClockCenter").GetComponent<Transform>();
        }

        if (transform.position.z > zmax)
            StartCoroutine(Xp());
    }
    public void onClickXs()
    {
        if (isDistanceMove) return;
        unlockAll();
        //if (!BasicData.isfixed && BasicData.levelSwitch[5]) drop();
        if(!isAutoToolSetting)
            getClock();
        if (transform.position.z < zmin)
            StartCoroutine(Xs());
    }
    public void onClickYp()
    {
        if (isDistanceMove) return;
        unlockAll();

        //if (!BasicData.isfixed && BasicData.levelSwitch[5]) drop();
        if (transform.position.y < 11.5f)
            StartCoroutine(Yp());
    }
    public void onClickYs()
    {
        if (isDistanceMove) return;
        unlockAll();
        //if (!BasicData.isfixed && BasicData.levelSwitch[5]) drop();
        if (!BasicData.banYMove)
            StartCoroutine(Ys());
    }
    public void onClickZp()
    {
        if (isDistanceMove) return;
        unlockAll();
        if(!isAutoToolSetting)
            getClock();
        //if (!BasicData.isfixed && BasicData.levelSwitch[5]) drop();
        if (transform.position.x > xmax)
            StartCoroutine(Zp());

    }
    public void onClickZs()
    {
        if (isDistanceMove) return;
        unlockAll();
        if(!isAutoToolSetting)
            getClock();
        //if (!BasicData.isfixed && BasicData.levelSwitch[5]) drop();
        if (transform.position.x < xmin)
            StartCoroutine(Zs());
    }
}