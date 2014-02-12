using System;
using System.Data;
using ZyGames.Framework.Game.Service;


namespace ProjectX.Script.Action
{

    /// <summary>
    /// 1001_Hello【未完成】
    /// </summary>
    public class Action1001 : BaseAction
    {
        private string name;
        private string Content;


        public Action1001(HttpGet httpGet)
            : base(ActionIDDefine.Cst_Action1001, httpGet)
        {

        }

        public override void BuildPacket()
        {
            this.PushIntoStack(Content);

        }

        public override bool GetUrlElement()
        {
            if (httpGet.GetString("name", ref name))
            {
                return true;
            }
            return false;
        }

        public override bool TakeAction()
        {
            Content = "Hello " + name;
            return true;
        }
    }
}
