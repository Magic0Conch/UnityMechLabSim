using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PannelEngine : MonoBehaviour
{
    public Text XText;
    public Text YText;
    public Text ZText;


    void Update()
    {
        XText.text = (BasicData.X*100 + BasicData.offset.x).ToString("0.000");
        YText.text = (BasicData.Y*80 + BasicData.offset.y).ToString("0.000");
        ZText.text = (-BasicData.Z*100 - BasicData.offset.z).ToString("0.000");
    }
}
