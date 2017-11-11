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

        public void CanPerformAttack(ref bool result)
        {
            result = Selection.ThisShip.AreWeaponsNotDisabled();

            if (result == false) Messages.ShowErrorToHuman("Cannot attack: Weapons Disabled Token");
        }

    }
}
