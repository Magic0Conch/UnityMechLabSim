using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left*Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward*Time.deltaTime);
        }
    }
}
