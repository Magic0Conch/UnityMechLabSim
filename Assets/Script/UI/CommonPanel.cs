using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonPanel : MonoBehaviour
{
    public GameObject IpSettingPannel;

    public void ChangeIpButton()
    {
        GameObject peng = GameObject.Find("PhotonEngine");
        RectTransform rt1 = peng.transform.GetChild(0).GetComponent<RectTransform>();//connected status
        RectTransform rt2 = peng.transform.GetChild(1).GetComponent<RectTransform>();//
        print(rt1.localPosition.x + ":" + rt1.localPosition.y + ":" + rt1.localPosition.z);
        print(rt2.localPosition.x + ":" + rt2.localPosition.y + ":" + rt2.localPosition.z);
        IpSettingPannel.SetActive(!IpSettingPannel.activeInHierarchy);
        if(IpSettingPannel.activeInHierarchy)
        {
            InputField inputField = IpSettingPannel.GetComponentInChildren<InputField>();
            if (inputField != null)
                inputField.text = PhotonEngine.connectString;
        }
    }
    public void ConfirmIpSetting()
    {
        InputField inputField = IpSettingPannel.GetComponentInChildren<InputField>();
        if(inputField!=null)
        {
            PhotonEngine.Peer.Disconnect();
            PhotonEngine.connectString = inputField.text;
            PhotonEngine.Peer.Connect(PhotonEngine.connectString, "MyGame1");
        }
        IpSettingPannel.SetActive(!IpSettingPannel.activeInHierarchy);
    }
}
