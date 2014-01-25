using UnityEngine;
using System.Collections;
using System;

public class SocketPackage 
{
    public bool IsOverTime
    {
        get;
        set;
    }
    public string ErrorMsg
    {
       set;
       get;
    }

    public INetCallback FuncCallback
    {
        set;
        get;
    }

    /// <summary>
    /// ������ 0����ɹ� ��0ʧ��
    /// </summary>
    public int ErrorCode
    {
        set;
        get;
    }
   
    public object UserData{set;get;}

    public NetReader Reader{get;set;}

    public int MsgId{set;get;}

    public int ActionId{set;get;}

    public bool HasLoading{set;get;}

    public DateTime SendTime{set;get;}
}
