using System;
using System.IO;
using System.Text;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Util;
namespace Assets.UpDowns
{
    class WithCallBack
    {
        static void Main(string[] args)
        {
            WithCallBack.PutObjectCallback();
            Console.ReadKey();
        }
        public static void PutObjectCallback()
        {
            var endpoint = "<yourEndpoint>";
            var accessKeyId = "<yourAccessKeyId>";
            var accessKeySecret = "<yourAccessKeySecret>";
            var bucketName = "<yourBucketName>";
            var objectName = "<yourObjectName>";
            var localFilename = "<yourLocalFilename>";
            // 设置回调请求的服务器地址。
            const string callbackUrl = "http://oss-demo.aliyuncs.com:23450";
            // 设置发起回调时请求body的值。
            const string callbackBody = "bucket=${bucket}&object=${object}&etag=${etag}&size=${size}&mimeType=${mimeType}&" +
                                        "my_var1=${x:var1}&my_var2=${x:var2}";
            // 创建OssClient实例。
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                string responseContent = "";
                var metadata = BuildCallbackMetadata(callbackUrl, callbackBody);
                using (var fs = File.Open(localFilename, FileMode.Open))
                {
                    var putObjectRequest = new PutObjectRequest(bucketName, objectName, fs, metadata);
                    var result = client.PutObject(putObjectRequest);
                    responseContent = GetCallbackResponse(result);
                }
                Console.WriteLine("Put object:{0} succeeded, callback response content:{1}", objectName, responseContent);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID: {2}\tHostID: {3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
        }
        // 设置上传回调。
        private static ObjectMetadata BuildCallbackMetadata(string callbackUrl, string callbackBody)
        {
            string callbackHeaderBuilder = new CallbackHeaderBuilder(callbackUrl, callbackBody).Build();
            string CallbackVariableHeaderBuilder = new CallbackVariableHeaderBuilder().
                AddCallbackVariable("x:var1", "x:value1").AddCallbackVariable("x:var2", "x:value2").Build();
            var metadata = new ObjectMetadata();
            metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);
            metadata.AddHeader(HttpHeaders.CallbackVar, CallbackVariableHeaderBuilder);
            return metadata;
        }
        // 读取上传回调返回的消息内容。
        private static string GetCallbackResponse(PutObjectResult putObjectResult)
        {
            string callbackResponse = null;
            using (var stream = putObjectResult.ResponseStream)
            {
                var buffer = new byte[4 * 1024];
                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                callbackResponse = Encoding.Default.GetString(buffer, 0, bytesRead);
            }
            return callbackResponse;
        }
    }
}