using Conditions;
using Ship;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class WedgeAntilles : T65XWing
        {
            public WedgeAntilles() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Wedge Antilles",
                    6,
                    54,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WedgeAntillesAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 1
                );

                ModelInfo.SkinName = "Wedge Antilles";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WedgeAntillesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddWedgeAntillesAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= AddWedgeAntillesAbility;
        }

        public void AddWedgeAntillesAbility()
        {
            WedgeAntillesCondition condition = new WedgeAntillesCondition(Combat.Defender, HostShip);
            Combat.Defender.Tokens.AssignCondition(condition);
        }
    }
}

namespace Conditions
{
    public class WedgeAntillesCondition : GenericToken
    {
        bool AgilityWasDecreased = false;

        public WedgeAntillesCondition(GenericShip host, GenericShip source) : base(host)
        {
            Name = ImageName = "Debuff Token";
            TooltipType = source.GetType();
            Temporary = false;
        }

        public override void WhenAssigned()
        {
            if (Host.State.Agility != 0)
            {
                AgilityWasDecreased = true;

                Messages.ShowInfo("Wedge Antilles: The defender's agility has been decreased by 1");
                Host.ChangeAgilityBy(-1);
            }

            Host.OnAttackFinishAsDefender += RemoveWedgeAntillesAbility;
        }

        public void RemoveWedgeAntillesAbility(GenericShip ship)
        {
            Host.Tokens.RemoveCondition(this);
        }

        public override void WhenRemoved()
        {
            if (AgilityWasDecreased)
            {
                Messages.ShowInfo("Wedge Antilles: The defender's agility has been restored");
                Host.ChangeAgilityBy(+1);
            }

            Host.OnAttackFinishAsDefender -= RemoveWedgeAntillesAbility;
        }
    }
}
