using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.RPC.Sockets;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Game.Runtime;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Game.Context;
using ClientSocket;

namespace gate_server
{
    public class GateServer : GameSocketHost
    {
        
        protected override void OnConnectCompleted(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("Client:{0} connect to server.", e.Socket.RemoteEndPoint);
        }

        protected override void OnRequested(HttpGet httpGet, IGameResponse response)
        {
            Console.WriteLine("Request data ActionId:{0},ip:{1}", httpGet.ActionId, httpGet.RemoteAddress);
            ActionFactory.Request(httpGet, response, null);
        }

        protected override void OnStartAffer()
        {
            try
            {
                EnvironmentSetting eSetting = new EnvironmentSetting() { };
                CacheSetting cSetting = new CacheSetting() { AutoRunEvent = true, ExpiredInterval = 600, UpdateInterval = 100};
                GameEnvironment.Start(eSetting, cSetting);
                Console.WriteLine("The server is staring...");
                ConnectLogicServer();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:{0}", ex.Message);
            }
        }

        protected override void OnServiceStop()
        {
            GameEnvironment.Stop();
            Console.WriteLine("The server is stoped");
        }

        protected override BaseUser GetUser(int userId)
        {
            throw new NotImplementedException();
        }

        private void ConnectLogicServer()
        {
            NetWriter.SetUrl("127.0.0.1:8001");
            NetWriter writer = NetWriter.Instance;
            writer.writeString("PageIndex", "1");
            writer.writeInt32("PageSize", 10);
            Net.Instance.Request(1001, ConnLogicServerCallback, null);
        }

        void ConnLogicServerCallback(ServerResponse.ResponseData data, object userdata)
        {
            Console.WriteLine("ConnLogicServerCallback");
        }
    }
}
