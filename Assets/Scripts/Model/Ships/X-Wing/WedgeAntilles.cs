using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using RuleSets;

namespace Ship
{
    namespace XWing
    {
        public class WedgeAntilles : XWing, ISecondEditionPilot
        {
            public WedgeAntilles() : base()
            {
                PilotName = "Wedge Antilles";
                PilotSkill = 9;
                Cost = 29;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.WedgeAntillesAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 6;
                Cost = 70;
            }
        }
    }
}

namespace Abilities
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
            Combat.Defender.Tokens.AssignCondition(typeof(Conditions.WedgeAntillesCondition));
        }
    }
}

namespace Conditions
{
    public class WedgeAntillesCondition : Tokens.GenericToken
    {
        bool AgilityWasDecreased = false;

        public WedgeAntillesCondition(GenericShip host) : base(host)
        {
            Name = "Debuff Token";
            TooltipType = typeof(Ship.XWing.WedgeAntilles);

            Temporary = false;
        }

        public override void WhenAssigned()
        {
            if (Combat.Defender.Agility != 0)
            {
                AgilityWasDecreased = true;

                Messages.ShowError("Wedge Antilles: Agility is decreased");
                Host.ChangeAgilityBy(-1);
            }

            Combat.Attacker.OnAttackFinish += RemoveWedgeAntillesAbility;
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

            Combat.Attacker.OnAttackFinish -= RemoveWedgeAntillesAbility;
        }
    }
}
