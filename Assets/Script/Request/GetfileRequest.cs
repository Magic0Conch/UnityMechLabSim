using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Assets.Script
{
    class GetfileRequest : Request
    {
        //[HideInInspector]
        //public string Username;
        //[HideInInspector]
        //public string Password;
        //[HideInInspector]
        //public string Usertype;

        //private RegistrerPanel registerPanel;
        public override void Start()
        {
            base.Start();
        }
        //发起请求
        public override void DefaultRequse()
        {

            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.Username, BasicData.theUsername);
            PhotonEngine.Peer.OpCustom((byte)OpCode, data, true);

        }
        ////得到服务器的响应
        public override void OnOperationResponse(OperationResponse operationResponse)
        {
            object stro;
            operationResponse.Parameters.TryGetValue((byte)ParameterCode.Report, out stro);
            print("取得文件响应.");
            //FileBinaryConvertHelper.Bytes2File((byte[])stro, Application.streamingAssetsPath +"/" +BasicData.theUsername + ".doc");
            FileBinaryConvertHelper.Bytes2File((byte[])stro, BasicData. folderPath + "/" +BasicData.theUsername + ".doc");
        }
    }
}
