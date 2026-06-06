using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class totalEngine : MonoBehaviour
{
    GameObject positionCenterGameobject;
    GameObject JointedMachine;
    // Start is called before the first frame update
    void Start()
    {
        //-387 225
        GameObject peng = GameObject.Find("PhotonEngine");
        if(peng!=null)
        {
            RectTransform rt1 = peng.transform.GetChild(0).GetComponent<RectTransform>();//connected status
            rt1.localPosition = new Vector2(396, 209);
        }
        BasicData.offset = Vector3.zero;
        positionCenterGameobject = GameObject.Find("外壳");
        JointedMachine = GameObject.Find("上部");
        BasicData.offset.y = -JointedMachine.transform.position.y*80;
    }

    // Update is called once per frame
    void Update()
    {
        BasicData.X = positionCenterGameobject.transform.position.x*0.5f;
        BasicData.Y = JointedMachine.transform.position.y*0.5f;
        BasicData.Z = positionCenterGameobject.transform.position.z*0.5f;
    }
}
