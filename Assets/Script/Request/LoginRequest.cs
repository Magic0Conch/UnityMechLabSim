using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Common;

public class LoginRequest : Request
{
    [HideInInspector]
    public string Username;
    [HideInInspector]
    public string Password;

    private Loginpanel loginpanel;
    public override void Start()
    {
        base.Start();
        loginpanel = GetComponent<Loginpanel>();
    }


    //发起请求
    public override void DefaultRequse()
    {
        Dictionary<byte, object> data = new Dictionary<byte, object>();
        data.Add((byte)ParameterCode.Username, Username);
        data.Add((byte)ParameterCode.Password, Password);
        print("login opcode: " + (byte)OpCode);
        PhotonEngine.Peer.OpCustom((byte)OpCode, data, true);//把用户名和密码传递给服务器
    }
    //得到响应
    public override void OnOperationResponse(OperationResponse operationResponse)
    {
        ReturnCode returnCode = (ReturnCode)operationResponse.ReturnCode;
        loginpanel.OnLoginResponse(returnCode);
        print("login响应");
    }
}
