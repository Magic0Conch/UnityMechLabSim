using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Common;

public class RegisterRequest : Request
{
    [HideInInspector]
    public string Username;
    [HideInInspector]
    public string Password;
    [HideInInspector]
    public string Usertype;

    private RegistrerPanel registerPanel;
    public override void Start()
    {
        base.Start();
        registerPanel = GetComponent<RegistrerPanel>();
    }
    //发起请求
    public override void DefaultRequse()
    {
        Dictionary<byte, object> data = new Dictionary<byte, object>();
        data.Add((byte)ParameterCode.Username, Username);
        data.Add((byte)ParameterCode.Password, Password);
        data.Add((byte)ParameterCode.Usertype, Usertype);
        print("register opcode: " + (byte)OpCode);
        PhotonEngine.Peer.OpCustom((byte)OpCode, data, true);
    }
    ////得到服务器的响应
    public override void OnOperationResponse(OperationResponse operationResponse)
    {
        ReturnCode returnCode = (ReturnCode)operationResponse.ReturnCode;
        registerPanel.OnRegisterResponse(returnCode);
        print("register响应");
    }
}