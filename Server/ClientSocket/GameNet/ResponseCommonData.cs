using System;
using System.Collections.Generic;
//用于处理 通用协议头部分的逻辑 跟具体游戏相关逻辑处理 
namespace ClientSocket
{
    public class NetResponse
    {
        public static NetResponse s_instance;
        //记录10003错误码最后的登录时间  确保2S内不会自动重新 登录请求
        private DateTime m_last10003ErrorCode_Request = DateTime.Now;
        public static NetResponse Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new NetResponse();
                }
                return s_instance;
            }
        }
        public bool CommonData(NetReader reader)
        {
            bool bRet = true;
            return bRet;
        }


        void OnQuit(object userData)
        {
            
        }

        public void NetError(Net.eNetError eType, int nActionId, string strMsg)
        {
            Console.WriteLine("Net Error" + strMsg);
        }


    }
}

