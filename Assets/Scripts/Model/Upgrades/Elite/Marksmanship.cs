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

        private void MarksmanshipAddAction(Ship.GenericShip host)
        {
            //if (host.CanPerformFreeAction("Marksmanship", afterMovement))
            host.AddAvailableAction(new MarksmanshipAction());
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
            Game.Phases.OnEndPhaseStart += MarksmanshipUnSubscribeToFiceModification;
        }

        private void MarksmanshipAddDiceModification(ref List<Actions.GenericAction> list)
        {
            list.Add(this);
        }

        private void MarksmanshipUnSubscribeToFiceModification()
        {
            host.AfterGenerateDiceModifications -= MarksmanshipAddDiceModification;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Game.Combat.AttackStep == CombatStep.Attack) result = true;
            return result;
        }

        public override void ActionEffect()
        {
            Game.Combat.CurentDiceRoll.ChangeOne(DiceSide.Focus, DiceSide.Crit);
            Game.Combat.CurentDiceRoll.ChangeAll(DiceSide.Focus, DiceSide.Success);
        }

    }

}
