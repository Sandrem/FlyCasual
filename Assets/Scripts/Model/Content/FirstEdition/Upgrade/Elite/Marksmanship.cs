using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Ship;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class Marksmanship : GenericUpgrade
    {
        public Marksmanship() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Marksmanship",
                UpgradeType.Elite,
                cost: 3,
                abilityType: typeof(Abilities.FirstEdition.MarksmanshipAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class MarksmanshipAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += MarksmanshipAddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= MarksmanshipAddAction;
        }

        private void MarksmanshipAddAction(GenericShip host)
        {
            GenericAction newAction = new MarksmanshipAction
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host
            };
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
            Name = DiceModificationName = "Marksmanship";

            IsTurnsAllFocusIntoSuccess = true;
        }

        public override void ActionTake()
        {
            Host = Selection.ThisShip;
            Host.OnGenerateDiceModifications += MarksmanshipAddDiceModification;
            Phases.Events.OnEndPhaseStart_NoTriggers += MarksmanshipUnSubscribeToFiceModification;
            Host.Tokens.AssignCondition(typeof(Conditions.MarksmanshipCondition));
            Phases.CurrentSubPhase.CallBack();
        }

        public override int GetActionPriority()
        {
            int result = 0;
            if (ActionsHolder.HasTarget(Selection.ThisShip)) result = 60;
            return result;
        }

        private void MarksmanshipAddDiceModification(GenericShip ship)
        {
            ship.AddAvailableDiceModification(this);
        }

        private void MarksmanshipUnSubscribeToFiceModification()
        {
            Host.Tokens.RemoveCondition(typeof(Conditions.MarksmanshipCondition));
            Host.OnGenerateDiceModifications -= MarksmanshipAddDiceModification;
            Phases.Events.OnEndPhaseStart_NoTriggers -= MarksmanshipUnSubscribeToFiceModification;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack) result = true;
            return result;
        }

        public override int GetDiceModificationPriority()
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
            Tooltip = new UpgradesList.FirstEdition.Marksmanship().ImageUrl;
        }
    }

}