using Common;
using ExitGames.Client.Photon;
using UnityEngine;

public abstract class Request : MonoBehaviour
{

    public OperationCode OpCode;

    public abstract void DefaultRequse();//向服务器发起请求方法
    public abstract void OnOperationResponse(OperationResponse operationResponse);//服务器收到消息响应给客户端的方法

    //当这个组件初始化的时候添加这个Request
    public virtual void Start()
    {
        PhotonEngine.Instance.AddRequst(this);
    }
    //当这个组件被销毁的时候移除这个Request
    public void OnDestroy()
    {
        PhotonEngine.Instance.RemoveRequst(this);
    }
}