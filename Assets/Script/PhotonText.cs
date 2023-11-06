using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine;
public class PhotonText : MonoBehaviour
{

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            SendRequest();
        }
    }
    void SendRequest()
    {
        Dictionary<byte, object> data = new Dictionary<byte, object>();
        data.Add(1, 100);
        data.Add(2, "ads你好啊");

        PhotonEngine.Peer.OpCustom(1, data, true);
    }

    
    
}