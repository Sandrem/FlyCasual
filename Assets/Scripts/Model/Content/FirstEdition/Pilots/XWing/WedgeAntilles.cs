using System.Collections;
using System.Collections.Generic;
using Ship;
using Abilities.FirstEdition;
using System;
using Conditions;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class WedgeAntilles : XWing
        {
            public WedgeAntilles() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Wedge Antilles",
                    9,
                    29,
                    isLimited: true,
                    abilityType: typeof(WedgeAntillesAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
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
            Name = "Debuff Token";
            TooltipType = source.GetType();
            Temporary = false;
        }

        public override void WhenAssigned()
        {
            if (Host.State.Agility != 0)
            {
                AgilityWasDecreased = true;

                Messages.ShowError("Wedge Antilles: Agility is decreased");
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
                Messages.ShowInfo("Agility is restored");
                Host.ChangeAgilityBy(+1);
            }

            Host.OnAttackFinishAsDefender -= RemoveWedgeAntillesAbility;
        }
    }
}
