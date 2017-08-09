using System.Collections.Generic;
using UnityEngine;

namespace RulesList
{
    public class TargetIsLegalForShotRule
    {
        //EVENTS
        public delegate void EventHandler2Ships(ref bool result, Ship.GenericShip attacker, Ship.GenericShip defender);
        public static event EventHandler2Ships OnCheckTargetIsLegal;

        public bool IsLegal()
        {
            bool result = true;
            result = Selection.ThisShip.CallCanPerformAttack(result);

            if (result) OnCheckTargetIsLegal(ref result, Selection.ThisShip, Selection.AnotherShip);
            return result;
        }
    }
}
