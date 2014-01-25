using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.RPC.Sockets;
using System;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Game.Runtime;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Game.Context;

namespace logic_server
{
    class LogicServer : GameSocketHost
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
                CacheSetting cSetting = new CacheSetting() { AutoRunEvent = true, ExpiredInterval = 600, UpdateInterval = 100 };
                GameEnvironment.Start(eSetting, cSetting);
                Console.WriteLine("The server is staring...");

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
    }
}
