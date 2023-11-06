using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    Animator animator;
    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        animator.speed = 0;
    }
    public void End()
    {
        animator.speed = 0;
        print("end");
        if(gameObject.name== "Oil")
        {
            print("oil");
            BasicData.levelSwitch[26] = true;
        }
    }
}
