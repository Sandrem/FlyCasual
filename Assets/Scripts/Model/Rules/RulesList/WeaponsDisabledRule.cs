using System.Collections.Generic;
using UnityEngine;

namespace RulesList
{
    public class WeaponsDisabledRule
    {
        static bool RuleIsInitialized = false;

        public WeaponsDisabledRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (!RuleIsInitialized)
            {
                Ship.GenericShip.OnTryPerformAttackGlobal += CanPerformAttack;
                RuleIsInitialized = true;
            }
        }

        public void CanPerformAttack(ref bool result, List<string> stringList)
        {
            if (Selection.ThisShip.AreWeaponsDisabled())
            {
                stringList.Add("Cannot attack while weapons are disabled");
                result = false;
            }
        }

    }
}
