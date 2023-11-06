using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Click : MonoBehaviour,IPointerClickHandler
{
    bool isc = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!BasicData.levelSwitch[25])
        {
            Instantiate(GameObject.Find("Canvas").transform.GetChild(11), GameObject.Find("Canvas").transform).gameObject.SetActive(true);
        }
        if (BasicData.levelSwitch[25]&&!isc)
        {
            isc = true;
            GameObject.Find("MovablePole").GetComponent<Animator>().speed = 1;
            GameObject.Find("Oil").GetComponent<Animator>().speed = 1;
        }
    }
}
