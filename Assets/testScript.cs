using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public void buttonTest()
    {
        string thePath = Application.streamingAssetsPath + "/Test.doc";
        byte[] byteArray = FileBinaryConvertHelper.File2Bytes(thePath);//文件转成byte二进制数组
        string JarContent = Convert.ToBase64String(byteArray);//将二进制转成string类型，可以存到数据库里面了
        Dictionary<byte, object> data = new Dictionary<byte, object>();
        print(JarContent.Substring(0,20));
        data.Add((byte)ParameterCode.Username, BasicData.username);
        data.Add((byte)ParameterCode.Report, byteArray);
        PhotonEngine.Peer.OpCustom((byte)OperationCode.Wordfile, data, true);
    }
}
