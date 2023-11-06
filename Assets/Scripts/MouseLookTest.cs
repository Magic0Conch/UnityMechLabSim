using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookTest : MonoBehaviour
{

    public Transform target;
    public int MouseWheelSensitivity = 1; 
    public int MouseZoomMin = 1; 
    public int MouseZoomMax = 20; 
    public float Distance = 5; 

    public float moveSpeed = 10; 

    public float xSpeed = 250.0f; 
    public float ySpeed = 120.0f; 

    private int yMinLimit = -360;
    private int yMaxLimit = 360;

    public float x = 0.0f; 
    public float y = 0.0f;

    private Vector3 targetOnScreenPosition; 
    public Quaternion storeRotation; 
    public Vector3 CameraTargetPosition; 
    private Vector3 initPosition; 
    private Vector3 cameraX; 
    private Vector3 cameraY; 
    private Vector3 cameraZ; 

    private Vector3 initScreenPos; 
    private Vector3 curScreenPos;

    public void Awake()
    {
        for (int i = 0; i < BasicData.levelSwitch.Length; i++)
            BasicData.levelSwitch[i] = false;
    }

    public void Start()
    {

//        var angles = transform.eulerAngles;
        CameraTargetPosition = target.position;
        storeRotation = Quaternion.Euler(y + 60, x, 0);
        transform.rotation = storeRotation; 
  //      Vector3 position = storeRotation * new Vector3(0.0F, 0.0F, -Distance) + CameraTargetPosition; 
        transform.position = storeRotation * new Vector3(0, 0, -Distance) + CameraTargetPosition; 
    }

    void Update()
    {

        if (Input.GetMouseButton(1))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            storeRotation = Quaternion.Euler(y + 60, x, 0);
            var position = storeRotation * new Vector3(0.0f, 0.0f, -Distance) + CameraTargetPosition;

            transform.rotation = storeRotation;
            transform.position = position;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") != 0) 
        {
            if (Distance >= MouseZoomMin && Distance <= MouseZoomMax)
            {
                Distance -= Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity;
            }
            if (Distance < MouseZoomMin)
            {
                Distance = MouseZoomMin;
            }
            if (Distance > MouseZoomMax)
            {
                Distance = MouseZoomMax;
            }
            var rotation = transform.rotation;

            transform.position = storeRotation * new Vector3(0.0F, 0.0F, -Distance) + CameraTargetPosition;
        }

        if (Input.GetMouseButtonDown(2))
        {
            cameraX = transform.right;
            cameraY = transform.up;
            cameraZ = transform.forward;

            initScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, targetOnScreenPosition.z);

            targetOnScreenPosition = Camera.main.WorldToScreenPoint(CameraTargetPosition);
            initPosition = CameraTargetPosition;
        }

        if (Input.GetMouseButton(2))
        {
            curScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, targetOnScreenPosition.z);

            target.position = initPosition - 0.01f * ((curScreenPos.x - initScreenPos.x) * cameraX + (curScreenPos.y - initScreenPos.y) * cameraY);
            Vector3 mPosition = storeRotation * new Vector3(0.0F, 0.0F, -Distance) + target.position;
            transform.position = mPosition;

        }
        if (Input.GetMouseButtonUp(2))
        {
            CameraTargetPosition = target.position;
        }
    }

    public void Fresh()
    {
        cameraX = transform.right;
        cameraY = transform.up;
        cameraZ = transform.forward;

        initScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, targetOnScreenPosition.z);

        targetOnScreenPosition = Camera.main.WorldToScreenPoint(CameraTargetPosition);
        initPosition = CameraTargetPosition;
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}