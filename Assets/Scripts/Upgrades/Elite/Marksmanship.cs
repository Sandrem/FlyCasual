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
            //if (host.CanPerformFreeAction("Marksmanship", afterMovement))
            host.AvailableActionsList.Add(new MarksmanshipAction());
        }

    }

    public class MarksmanshipAction : Actions.GenericAction
    {
        private Ship.GenericShip host;

        public MarksmanshipAction()
        {
            Name = EffectName = "Marksmanship";
        }

        public override void ActionTake()
        {
            host = Game.Selection.ThisShip;
            host.AfterGenerateDiceModifications += MarksmanshipAddDiceModification;
            Game.PhaseManager.OnEndPhaseStart += MarksmanshipUnSubscribeToFiceModification;
        }

        private void MarksmanshipAddDiceModification(ref List<Actions.GenericAction> list)
        {
            if (Game.Combat.AttackStep == CombatStep.Attack)
            {
                list.Add(this);
            }
        }

        private void MarksmanshipUnSubscribeToFiceModification()
        {
            host.AfterGenerateDiceModifications -= MarksmanshipAddDiceModification;
        }

        public override void ActionEffect()
        {
            Game.Combat.CurentDiceRoll.ChangeOne(DiceSide.Focus, DiceSide.Crit);
            Game.Combat.CurentDiceRoll.ChangeAll(DiceSide.Focus, DiceSide.Success);
        }

    }

}
