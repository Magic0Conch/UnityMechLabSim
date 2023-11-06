using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenFileController : MonoBehaviour
{
    string path;


    private void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.F))
        {
            path = Application.dataPath;
            Application.OpenURL("file:///" + path + "/StreamingAssets/" + "Test.doc");
        }
    }

}