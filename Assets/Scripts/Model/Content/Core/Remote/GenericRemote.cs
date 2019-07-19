using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote
{
    public abstract class GenericRemote
    {
        public RemoteInfo RemoteInfo { get; protected set; }
        public RemoteTokensHolder Tokens { get; protected set; }
        public RemoteState State { get; protected set; }
    }
}
