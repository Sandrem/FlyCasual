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
            Cost = 3;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionsList += MarksmanshipAddAction;
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

    public class MarksmanshipAction : GenericAction
    {
        private Ship.GenericShip host;

        public MarksmanshipAction()
        {
            Name = EffectName = "Marksmanship";

            IsTurnsAllFocusIntoSuccess = true;
        }

        public override void ActionTake()
        {
            host = Selection.ThisShip;
            host.AfterGenerateAvailableActionEffectsList += MarksmanshipAddDiceModification;
            host.AssignToken(new Conditions.MarksmanshipCondition());
            Phases.OnEndPhaseStart += MarksmanshipUnSubscribeToFiceModification;
            Phases.CurrentSubPhase.CallBack();
        }

        private void MarksmanshipAddDiceModification(Ship.GenericShip ship)
        {
            ship.AddAvailableActionEffect(this);
        }

        private void MarksmanshipUnSubscribeToFiceModification()
        {
            host.RemoveToken(typeof(Conditions.MarksmanshipCondition));
            host.AfterGenerateAvailableActionEffectsList -= MarksmanshipAddDiceModification;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack) result = true;
            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.Focuses;
                if (attackFocuses > 0) result = 60;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurentDiceRoll.ChangeOne(DiceSide.Focus, DiceSide.Crit);
            Combat.CurentDiceRoll.ChangeAll(DiceSide.Focus, DiceSide.Success);
            callBack();
        }

    }

}

namespace Conditions
{

    public class MarksmanshipCondition : Tokens.GenericToken
    {
        public MarksmanshipCondition()
        {
            Name = "Buff Token";
            Temporary = false;
            Tooltip = new UpgradesList.Marksmanship().ImageUrl;
        }
    }

}
