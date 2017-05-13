using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class BlueTargetLockToken : GenericToken
    {
        public BlueTargetLockToken() {
            Name = "Blue Target Lock Token";
            Temporary = false;
            Action = new Actions.TargetLockAction();
        }

        public override void GetAvailableEffects(ref List<Actions.GenericAction> availableActionEffects)
        {
            availableActionEffects.Add(Action);
        }
    }

}
