using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLoom : MonoBehaviour
{
    void Start()
    {
        Loom.Initialize();
        DontDestroyOnLoad(this.gameObject);
    }


}
