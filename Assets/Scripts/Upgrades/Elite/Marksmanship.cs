using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Upgrade
{

    public class Marksmanship : GenericUpgrade
    {

        public Marksmanship(Ship.GenericShip host) : base(host)
        {
            Type = UpgradeSlot.Elite;
            Name = ShortName = "Marksmanship";

            host.AfterAvailableActionListIsBuilt += MarksmanshipAddAction;
        }

        private void MarksmanshipAddAction(Ship.GenericShip host, bool afterMovement)
        {
            if (host.CanPerformFreeAction("Marksmanship", afterMovement)) host.AvailableActionsList.Add("Marksmanship", MarksmanshipSubscribeToFiceModification);
        }

        private void MarksmanshipSubscribeToFiceModification()
        {
            Host.AfterGenerateDiceModifications += MarksmanshipAddDiceModification;
            Game.PhaseManager.OnEndPhaseStart += MarksmanshipUnSubscribeToFiceModification;
        }

        private void MarksmanshipUnSubscribeToFiceModification()
        {
            Host.AfterGenerateDiceModifications -= MarksmanshipAddDiceModification;
        }

        private void MarksmanshipAddDiceModification(ref Dictionary<string, DiceModification> dict)
        {
            if (Game.Combat.AttackStep == CombatStep.Attack)
            {
                dict.Add("Marksmanship", MarksmanshipUseDiceModification);
            }
        }

        private void MarksmanshipUseDiceModification()
        {
            Game.Combat.CurentDiceRoll.ChangeOne(DiceSide.Focus, DiceSide.Crit);
            Game.Combat.CurentDiceRoll.ChangeAll(DiceSide.Focus, DiceSide.Success);
        }

    }

}
