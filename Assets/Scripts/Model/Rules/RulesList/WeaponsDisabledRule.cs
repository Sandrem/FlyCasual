using System.Collections.Generic;
using UnityEngine;

namespace RulesList
{
    public class WeaponsDisabledRule
    {

        public WeaponsDisabledRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Ship.GenericShip.OnTryPerformAttackGlobal += CanPerformAttack;
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
