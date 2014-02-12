using System;
using System.Collections.Generic;

namespace ClientSocket
{
    public partial class ServerResponse
    {
        public interface IResponse { }
        public IResponse GetResponse(NetReader reader, int ActionId)
        {
            IResponse r = null;
            switch (ActionId)
            {
                case 1000:
                    r = Decode_1000(reader);
                    break;
                case 1001:
                    r = Decode_1001(reader);
                    break;
                default:
                    r = null;
                    break;
            }
            return r;
        }
        public class ScoreObject : ServerResponse.IResponse
        {
            /// <summary> 
            ///  
            /// </summary> 
        }
        /// <summary> 
        /// [系统邮件]获取系统邮件 
        /// </summary> 
        private IResponse Decode_1000(NetReader reader)
        {
            if (reader.getInt() > 0)
            {

            }
            return new ScoreObject();
        }

        public class Resopnse_1001 : ServerResponse.IResponse
        {
            public string Content { set; get; }
        }
        public class Item
        {
            public string UserName { set; get; }
            public int Score { set; get; }
        }
        private IResponse Decode_1001(NetReader reader)
        {
            Resopnse_1001 ret = null;
            if (reader.StatusCode == 0)
            {
                ret = new Resopnse_1001();
                ret.Content = reader.readString();
            }
            return ret;
        }
    }

}
