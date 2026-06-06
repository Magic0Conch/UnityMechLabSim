using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using ExitGames.Client.Photon;

namespace Assets.Script
{
    class GetAllUserRequest:Request
    {
        public override void Start()
        {
            base.Start();
        }
        //发起请求
        public override void DefaultRequse()
        {
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            PhotonEngine.Peer.OpCustom((byte)OpCode, data, true);
        }
        ////得到服务器的响应
        public override void OnOperationResponse(OperationResponse operationResponse)
        {
            print("取得响应.");
            object stro;
            operationResponse.Parameters.TryGetValue((byte)ParameterCode.Getalluser, out stro);
            ///////////////////////
            ///object转list....怎么办
            BasicData.allUsers = stro.ToString();

            //foreach (var item in list)
            //{
            //    ++cnt;
            //    //ReportColumn r = new ReportColumn();
            //    //r.BindingField = item as QueryFieldBase;
            //    //SelectedColumnCollection.Add(r);
            //}


        }

    }
}
