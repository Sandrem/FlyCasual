using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class GenericTargetLockToken : GenericToken
    {
        public char Letter;

        public GenericTargetLockToken() {
            Temporary = false;
        }
    }

}
