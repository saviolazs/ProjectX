using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ClientSocket
{
    public partial class ServerResponse
    {
        private static ServerResponse s_instance = null;
        public static ServerResponse Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new ServerResponse();
                }
                return s_instance;
            }
        }
        public class ResponseData
        {
            public IResponse Resonse { get; set; }
            public string ErrorMsg { get; set; }
            public int ErrorCode { get; set; }
            public int ActionId { get; set; }
        }

        public ResponseData GetData(NetReader reader)
        {

            ResponseData data = new ResponseData();
            data.ActionId = reader.ActionId;
            data.ErrorCode = reader.StatusCode;
            data.ErrorMsg = reader.Description;
            IResponse ret = null;
            if (data.ErrorCode == Net.Instance.NetSuccess)
            {
                ret = GetResponse(reader, data.ActionId);
            }
            data.Resonse = ret;
            return data;
        }

    }


    public class Net
    {
        public enum Status
        {
            eStartRequest = 0,
            eEndRequest = 1,
        }
        public delegate void RequestNotifyDelegate(Status eStatus);
        static Net s_instance = null;
        private const int NETSUCCESS = 0;
        private SocketConnect mSocket = null;
        private Timer mTimer = null;
        public delegate bool CanRequestDelegate(int actionId, object userData);

        public enum eNetError
        {
            eConnectFailed = 0,
            eTimeOut = 1,
        }

        public delegate bool CommonDataCallback(NetReader reader);
        public delegate void NetError(eNetError nType, int ActionId, string strMsg);

        public NetError NetErrorCallback
        {
            get;
            set;
        }
        public CommonDataCallback CommonCallback
        {
            get;
            set;
        }


        public int NetSuccess
        {
            get { return NETSUCCESS; }
        }

        public Net()
        {
            CommonCallback = NetResponse.Instance.CommonData;
            NetErrorCallback = NetResponse.Instance.NetError;
            mTimer = new Timer(Update, null, 0, 1000);
        }

        public void RequestDelegate(Net.Status eState)
        {
            // RequestDelegate(eState, null);
            //todo user loading
            if (eState == Net.Status.eStartRequest)
            {
            }
            else//Net.Status.eEndRequest
            {

            }
        }
        /* public LoginLoadingLogo mLoadingLogo = null;
         public void RequestDelegate(Net.Status eState)
         {
             RequestDelegate(eState, null);
         }

         public void RequestDelegate(Net.Status eState, string strText)
         {
             if (eState == Net.Status.eStartRequest)
             {
                 if (mLoadingLogo != null)
                 {
                     mLoadingLogo.nCounter++;
                 }
                 else
                 {
                     mLoadingLogo = LoginLoadingLogo.Create(strText);
                 }
             }
             else
             {
                 if (mLoadingLogo != null)
                 {
                     mLoadingLogo.nCounter--;
                     if (mLoadingLogo.nCounter <= 0)
                     {
                         mLoadingLogo.CloseWindow();
                         mLoadingLogo = null;
                     }
                 }
             }
         }
         */

        void Update(object obj)
        {
            if (mSocket != null)
            {
                mSocket.ProcessTimeOut();
                SocketPackage data = mSocket.Dequeue();
                if (data != null)
                {
                    OnSocketRespond(data);
                }
            }
        }

        public static Net Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new Net();
                }
                return s_instance;
            }
        }
        /// <summary>
        /// CallBack的函数要保证它在网络回来时的生命周期依然可用
        /// </summary>
        /// <param name="actionId"></param>
        /// <param name="callback"></param>
        /// <param name="userData"></param>
        public void Request(int actionId, INetCallback callback, object userData)
        {
            Request(actionId, callback, userData, true);
        }
        public void Request(int actionId, INetCallback callback, object userData, bool bShowLoading)
        {
            if (NetWriter.IsSocket())
            {
                SocketRequest(actionId, callback, userData, bShowLoading);
            }
            else
            {
                HttpRequest(actionId, callback, userData, bShowLoading);
            }
        }
        //
        //NetWriter.Instance.writeInt32()
        //发送请求
        public void HttpRequest(int actionId, INetCallback callback, object userData)
        {
            HttpRequest(actionId, callback, userData, true);
        }

        public void HttpRequest(int actionId, INetCallback callback, object userData, bool bShowLoading)
        {
            //NetWriter writer = NetWriter.Instance;
            //writer.writeInt32("actionId", actionId);
            //StartCoroutine(Http.GetRequest(writer.generatePostData(), userData, actionId, callback, this, bShowLoading));
            //NetWriter.resetData();
            throw new NotImplementedException();
        }

        /// <summary>
        /// parse input data
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        /// <param name="ud"></param>
        public void SocketRequest(int actionId, INetCallback callback, object userData, bool bShowLoading)
        {

            if (mSocket == null)
            {
                string strUrl = NetWriter.GetUrl();
                Console.WriteLine("url" + strUrl);
                string[] arr = strUrl.Split(new char[] { ':' });
                int nPort = int.Parse(arr[1]);
                mSocket = new SocketConnect(arr[0], nPort);
            }
            NetWriter writer = NetWriter.Instance;
            writer.writeInt32("actionId", actionId);
            byte[] data = NetWriter.Instance.PostData();
            SocketPackage package = new SocketPackage();
            package.FuncCallback = callback;
            package.UserData = userData;
            package.MsgId = NetWriter.MsgId - 1;
            package.ActionId = actionId;
            package.HasLoading = bShowLoading;
            package.SendTime = DateTime.Now;
            NetWriter.resetData();

            if (bShowLoading)
            {
                RequestDelegate(Status.eStartRequest);
            }
            mSocket.Request(data, package);
        }
        public void SocketRequest(int actionId, INetCallback callback, object userData)
        {
            SocketRequest(actionId, callback, userData, true);
        }


        /// <summary>
        /// socket respond
        /// </summary>
        /// <param name="package"></param>
        /// <param name="userdata"></param>
        public void OnSocketRespond(SocketPackage package)
        {
            if (package.HasLoading)
            {
                RequestDelegate(Status.eEndRequest);
            }
            if (package.ErrorCode != 0)
            {
                if (package.ErrorCode == -2)
                {
                    OnNetTimeOut(package.ActionId);
                }
                else
                {
                    OnNetError(package.ActionId, package.ErrorMsg);
                }

            }
            else
            {
                ServerResponse.ResponseData data = null;
                NetReader reader = package.Reader;
                bool bRet = true;

                if (CommonCallback != null)
                {
                    bRet = CommonCallback(reader);
                }

                if (bRet)
                {
                    data = ServerResponse.Instance.GetData(reader);
                    if (package.FuncCallback != null)
                    {
                        ProcessBodyData(data, package.UserData, package.FuncCallback);
                    }
                    else
                    {
                        Console.WriteLine("poll message ");
                    }

                }
            }
        }

        public void OnNetError(int nActionId, string str)
        {
            if (NetErrorCallback != null)
            {
                NetErrorCallback(eNetError.eConnectFailed, nActionId, str);
            }
        }
        public void OnNetTimeOut(int nActionId)
        {
            if (NetErrorCallback != null)
            {
                NetErrorCallback(eNetError.eTimeOut, nActionId, null);
            }

        }

        public void ProcessBodyData(ServerResponse.ResponseData data, object userdata, INetCallback callback)
        {
            Console.WriteLine("Net ProcessBodyData " + data.ActionId + " ErrorCode " + data.ErrorCode + " ErrorMsg " + data.ErrorMsg);
            callback(data, userdata);
        }
    }
}
