using Common;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script
{
    class BatchDownloadRequest:Request
    {
        public override void Start()
        {
            base.Start();
        }
        //发起请求
        public override void DefaultRequse()
        {

            

        }
        ////得到服务器的响应
        public override void OnOperationResponse(OperationResponse operationResponse)
        {
            object stro;
            operationResponse.Parameters.TryGetValue((byte)ParameterCode.Usernames, out stro);
            print("取得文件响应Batch.");
            //FileBinaryConvertHelper.Bytes2File((byte[])stro, Application.streamingAssetsPath +"/" +BasicData.theUsername + ".doc");
            FileBinaryConvertHelper.Bytes2File((byte[])stro, BasicData.folderPath + "/" + BasicData.theUsername + ".zip");
        }

    }
}
