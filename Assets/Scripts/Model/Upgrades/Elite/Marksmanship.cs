using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class Marksmanship : GenericUpgrade
    {

        public Marksmanship() : base()
        {
            Type = UpgradeSlot.Elite;
            Name = ShortName = "Marksmanship";
            ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/6/69/Marksmanship.png";
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterAvailableActionListIsBuilt += MarksmanshipAddAction;
        }

        private void MarksmanshipAddAction(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.MarksmanshipAction();
            newAction.ImageUrl = ImageUrl;
            host.AddAvailableAction(newAction);
        }

    }
}

namespace ActionsList
{

    public class MarksmanshipAction : ActionsList.GenericAction
    {
        private Ship.GenericShip host;

        public MarksmanshipAction()
        {
            Name = EffectName = "Marksmanship";
        }

        public override void ActionTake()
        {
            host = Selection.ThisShip;
            host.AfterGenerateDiceModifications += MarksmanshipAddDiceModification;
            Phases.OnEndPhaseStart += MarksmanshipUnSubscribeToFiceModification;
        }

        private void MarksmanshipAddDiceModification(ref List<ActionsList.GenericAction> list)
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
            if (Combat.AttackStep == CombatStep.Attack) result = true;
            return result;
        }

        public override void ActionEffect()
        {
            Combat.CurentDiceRoll.ChangeOne(DiceSide.Focus, DiceSide.Crit);
            Combat.CurentDiceRoll.ChangeAll(DiceSide.Focus, DiceSide.Success);
        }

    }

}
