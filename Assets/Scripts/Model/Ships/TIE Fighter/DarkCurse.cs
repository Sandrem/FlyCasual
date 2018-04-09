using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class DarkCurse : TIEFighter
        {
            public DarkCurse() : base()
            {
                PilotName = "\"Dark Curse\"";
                PilotSkill = 6;
                Cost = 16;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.DarkCurseAbility());
            }
        }
    }
}

namespace Abilities
{
    public class DarkCurseAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsDefender += AddDarkCursePilotAbility;
            HostShip.OnDefenceStartAsDefender += RemoveDarkCursePilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsDefender -= AddDarkCursePilotAbility;
            HostShip.OnDefenceStartAsDefender -= RemoveDarkCursePilotAbility;
        }

        private void AddDarkCursePilotAbility()
        {
            if (Combat.AttackStep == CombatStep.Attack)
            {
                Combat.Attacker.OnTryAddAvailableActionEffect += UseDarkCurseRestriction;
                Combat.Attacker.Tokens.AssignCondition(new Conditions.DarkCurseCondition(Combat.Attacker));
            }
        }

        private void UseDarkCurseRestriction(ActionsList.GenericAction action, ref bool canBeUsed)
        {
            if (action.TokensSpend.Contains(typeof(Tokens.FocusToken)))
            {
                Messages.ShowErrorToHuman("Dark Curse: Cannot spend focus");
                canBeUsed = false;
            }
            if (action.IsReroll)
            {
                Messages.ShowErrorToHuman("Dark Curse: Cannot reroll");
                canBeUsed = false;
            }
        }

        private void RemoveDarkCursePilotAbility()
        {
            if ((Combat.AttackStep == CombatStep.Defence) && (Combat.Defender.ShipId == HostShip.ShipId))
            {
                Combat.Attacker.OnTryAddAvailableActionEffect -= UseDarkCurseRestriction;
                Combat.Attacker.Tokens.RemoveCondition(typeof(Conditions.DarkCurseCondition));
            }
        }
    }
}

namespace Conditions
{
    public class DarkCurseCondition : Tokens.GenericToken
    {
        public DarkCurseCondition(GenericShip host) : base(host)
        {
            Name = "Debuff Token";
            Temporary = false;
            Tooltip = new Ship.TIEFighter.DarkCurse().ImageUrl;
        }
    }
}