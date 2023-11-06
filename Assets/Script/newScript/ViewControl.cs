using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewControl : MonoBehaviour
{
    public MouseLookTest mouseLook;

    public Vector2 fullAng;
    public float fullDis;
    public Vector3 fullPos;

    public Vector2 frontAng;
    public float frontDis;
    public Vector3 frontPos;

    public Vector2 sideAng;
    public float sideDis;
    public Vector3 sidePos;

    public void FullPress()
    {
        mouseLook.x = fullAng.x;
        mouseLook.y = fullAng.y;
        mouseLook.Distance = fullDis;
        mouseLook.CameraTargetPosition = fullPos;
        mouseLook.Start();
    }

    public void FrontPress()
    {
        mouseLook.x = frontAng.x;
        mouseLook.y = frontAng.y;
        mouseLook.Distance = frontDis;
        mouseLook.CameraTargetPosition = frontPos;
        mouseLook.Start();

    }

    public void SidePress()
    {
        mouseLook.x = sideAng.x;
        mouseLook.y = sideAng.y;
        mouseLook.Distance = sideDis;
        mouseLook.CameraTargetPosition = sidePos;
        mouseLook.Start();
    }
}
