using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using ExitGames.Client.Photon;

namespace Assets.Script
{
    class SubmitRequest:Request
    {
        public override void Start()
        {
            base.Start();
        }
        
        public override void DefaultRequse()
        {

        }
        
        public void Requse(string ProblemRecord)
        {
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.Username, BasicData.username);
            data.Add((byte)ParameterCode.Score, BasicData.Scores);
            data.Add((byte)ParameterCode.ProblemRecord, ProblemRecord);
            print(BasicData.username);
            PhotonEngine.Peer.OpCustom((byte)OpCode, data, true);
        }

        public override void OnOperationResponse(OperationResponse operationResponse)
        {
            print("提交成功");
        }

    }
}
