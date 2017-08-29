using System.Collections;
using UnityEngine;
using System;
using System.Linq;
using Players;
using Tokens;
using Ship;

namespace RulesList
{
    public class TargetLocksRule
    {
        public void RemoveTargetLocksOnDestruction(GenericShip ship)
        {
            while (true)
            {
                BlueTargetLockToken token = ship.GetToken(typeof(BlueTargetLockToken), '*') as BlueTargetLockToken;
                if (token != null)
                {
                    ship.RemoveToken(token.GetType(), token.Letter);
                }
                else
                {
                    break;
                }
            }

            while (true)
            {
                RedTargetLockToken token = ship.GetToken(typeof(RedTargetLockToken), '*') as RedTargetLockToken;
                if (token != null)
                {
                    ship.RemoveToken(token.GetType(), token.Letter);
                }
                else
                {
                    break;
                }
            }

        }
    } 
}