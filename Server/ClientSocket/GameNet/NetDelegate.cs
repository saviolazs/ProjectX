using System;

/*
      data.ActionId;
      data.ErrorCode;
      data.ErrorMsg;
      data.Resonse;
      data.Resonse
*/
namespace ClientSocket
{
    public delegate void INetCallback(ServerResponse.ResponseData data, object userdata);
}