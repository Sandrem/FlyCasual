using Ship;
using Upgrade;
using Abilities;
using ActionsList;
using RuleSets;

namespace UpgradesList
{

    public class Marksmanship : GenericUpgrade, ISecondEditionUpgrade
    {
        public Marksmanship() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Marksmanship";
            Cost = 3;

            UpgradeAbilities.Add(new MarksmanshipAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            UpgradeAbilities.RemoveAll(a => a is MarksmanshipAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.MarksmanshipAbility());
        }
    }
}

namespace Abilities.SecondEdition
{
    //While performing an attack, if the defender is in your bullseye firing arc, you may change one hit result to a critical hit result.
    public class MarksmanshipAbility : GenericDiceModAbility
    {
        public MarksmanshipAbility()
        {
            AllowChange(DieSide.Success, DieSide.Crit, 1);
        }

        public override bool IsActionEffectAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack && Combat.Attacker == HostShip && Combat.ShotInfo.InArcByType(Arcs.ArcTypes.Bullseye));
        }

        public override int GetActionEffectPriority()
        {
            return 20;
        }
    }
}

namespace Abilities
{
    public class MarksmanshipAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList += MarksmanshipAddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList -= MarksmanshipAddAction;
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
            Name = EffectName = "Marksmanship";

            IsTurnsAllFocusIntoSuccess = true;
        }

        public override void ActionTake()
        {
            Host = Selection.ThisShip;
            Host.AfterGenerateAvailableActionEffectsList += MarksmanshipAddDiceModification;
            Phases.Events.OnEndPhaseStart_NoTriggers += MarksmanshipUnSubscribeToFiceModification;
            Host.Tokens.AssignCondition(typeof(Conditions.MarksmanshipCondition));
            Phases.CurrentSubPhase.CallBack();
        }

        public override int GetActionPriority()
        {
            int result = 0;
            if (Actions.HasTarget(Selection.ThisShip)) result = 60;
            return result;
        }

        private void MarksmanshipAddDiceModification(GenericShip ship)
        {
            ship.AddAvailableActionEffect(this);
        }

        private void MarksmanshipUnSubscribeToFiceModification()
        {
            Host.Tokens.RemoveCondition(typeof(Conditions.MarksmanshipCondition));
            Host.AfterGenerateAvailableActionEffectsList -= MarksmanshipAddDiceModification;
            Phases.Events.OnEndPhaseStart_NoTriggers -= MarksmanshipUnSubscribeToFiceModification;
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
