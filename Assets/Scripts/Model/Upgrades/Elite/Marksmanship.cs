using Ship;
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
            Types.Add(UpgradeType.Elite);
            Name = "Marksmanship";
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

        public MarksmanshipAction()
        {
            Name = EffectName = "Marksmanship";

            IsTurnsAllFocusIntoSuccess = true;
        }

        public override void ActionTake()
        {
            Host = Selection.ThisShip;
            Host.AfterGenerateAvailableActionEffectsList += MarksmanshipAddDiceModification;
            Phases.OnEndPhaseStart_NoTriggers += MarksmanshipUnSubscribeToFiceModification;
            Host.Tokens.AssignCondition(new Conditions.MarksmanshipCondition(Host));
            Phases.CurrentSubPhase.CallBack();
        }

        public override int GetActionPriority()
        {
            int result = 0;
            if (Actions.HasTarget(Selection.ThisShip)) result = 60;
            return result;
        }

        private void MarksmanshipAddDiceModification(Ship.GenericShip ship)
        {
            ship.AddAvailableActionEffect(this);
        }

        private void MarksmanshipUnSubscribeToFiceModification()
        {
            Host.Tokens.RemoveCondition(typeof(Conditions.MarksmanshipCondition));
            Host.AfterGenerateAvailableActionEffectsList -= MarksmanshipAddDiceModification;
            Phases.OnEndPhaseStart_NoTriggers -= MarksmanshipUnSubscribeToFiceModification;
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
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Crit);
            Combat.CurrentDiceRoll.ChangeAll(DieSide.Focus, DieSide.Success);
            callBack();
        }

    }

}

namespace Conditions
{

    public class MarksmanshipCondition : Tokens.GenericToken
    {
        public MarksmanshipCondition(GenericShip host) : base(host)
        {
            Name = "Buff Token";
            Temporary = false;
            Tooltip = new UpgradesList.Marksmanship().ImageUrl;
        }
    }

}
