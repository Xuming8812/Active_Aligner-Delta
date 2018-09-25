using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delta
{
    public enum CommandType
    {
        Start,
        End,
        
        Delay =10,
    }


    public class ProcessComand
    {
        int _Iindex;
        CommandType _CommandType;
        string _CommandName;
        string _CommandComment;
        string[] _CommandParameter;
        int _OnErrorIndex;
        int _CallIndex;
        int _NextIndex;
        bool _Enable;
        bool _IsFunction;


    }
}
