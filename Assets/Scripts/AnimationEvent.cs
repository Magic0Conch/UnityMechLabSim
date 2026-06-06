using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public void DestorySelf()
    {
        Destroy(gameObject);
        BasicData.levelSwitch[9] = true;
    }
}
