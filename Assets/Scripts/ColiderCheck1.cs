using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColiderCheck1 : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
        BasicData.iscol = true;
        if(collision.gameObject.name== "上极板d")
        {
            BasicData.colZ = true;
        }
        else
        {
            BasicData.colZ = false;
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        print("out");
        BasicData.iscol = false;
        
    }
















}
