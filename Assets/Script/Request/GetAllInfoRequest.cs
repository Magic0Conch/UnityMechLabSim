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
    class GetAllInfoRequest:Request
    {
        public override void Start()
        {
            base.Start();
        }
        //发起请求
        public override void DefaultRequse()
        {

        }

        public void GetInfo(string username)
        {
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.Username, username);
            BasicData.theUsername = username;
            PhotonEngine.Peer.OpCustom((byte)OpCode, data, true);
        }
        ////得到服务器的响应
        public override void OnOperationResponse(OperationResponse operationResponse)
        {
            object[] tmp = new object[3];
            print("无武器相应");
            operationResponse.Parameters.TryGetValue((byte)ParameterCode.Password, out tmp[0]);
            operationResponse.Parameters.TryGetValue((byte)ParameterCode.Score, out tmp[1]);
            operationResponse.Parameters.TryGetValue((byte)ParameterCode.ProblemRecord, out tmp[2]);
            BasicData.thePassword = tmp[0].ToString();
            if (tmp[1] != null)
                BasicData.Scores = int.Parse(tmp[1].ToString());
            else
                BasicData.Scores = 0;
            if (tmp[2] != null)
                BasicData.theProblemRecord = tmp[2].ToString();
            else
                BasicData.theProblemRecord = "暂无";
        }
    }
}
