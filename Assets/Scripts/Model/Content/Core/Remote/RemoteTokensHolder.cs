using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokens;
using UnityEngine;

namespace Remote
{
    public class RemoteTokensHolder
    {
        public List<GenericToken> AssignedTokens { get; private set; } = new List<GenericToken>();

        public void AssignToken() { }
        public void RemoveToken() { }
    }
}
