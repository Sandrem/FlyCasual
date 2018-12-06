using Upgrade;
using Ship;

namespace UpgradesList.FirstEdition
{
    public class R2F2 : GenericUpgrade
    {
        public R2F2() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R2-F2",
                UpgradeType.Astromech,
                cost: 3,
                isLimited: true,
                abilityType: typeof(Abilities.FirstEdition.R2F2Ability)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class R2F2Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += R2F2AddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= R2F2AddAction;
        }

        private void R2F2AddAction(Ship.GenericShip host)
        {
            ActionsList.GenericAction action = new ActionsList.R2F2Action()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip
            };
            host.AddAvailableAction(action);
        }
    }
}

namespace ActionsList
{

    public class R2F2Action : GenericAction
    {
        public R2F2Action()
        {
            Name = DiceModificationName = "R2-F2: Increase Agility";
        }

        public override void ActionTake()
        {
            Sounds.PlayShipSound("Astromech-Beeping-and-whistling");

            Host.ChangeAgilityBy(+1);
            Phases.Events.OnEndPhaseStart_NoTriggers += R2F2DecreaseAgility;
            Host.Tokens.AssignCondition(typeof(Conditions.R2F2Condition));
            Phases.CurrentSubPhase.CallBack();
        }

        public override int GetActionPriority()
        {
            int result = 0;
            result = 10 * (ActionsHolder.CountEnemiesTargeting(Selection.ThisShip));
            return result;
        }

        private void R2F2DecreaseAgility()
        {
            Host.ChangeAgilityBy(-1);
            Host.Tokens.RemoveCondition(typeof(Conditions.R2F2Condition));
            Phases.Events.OnEndPhaseStart_NoTriggers -= R2F2DecreaseAgility;
        }

    }

}

namespace Conditions
{

    public class R2F2Condition : Tokens.GenericToken
    {
        public R2F2Condition(GenericShip host) : base(host)
        {
            Name = "Buff Token";
            Temporary = false;
            Tooltip = new UpgradesList.FirstEdition.R2F2().ImageUrl;
        }
    }

}