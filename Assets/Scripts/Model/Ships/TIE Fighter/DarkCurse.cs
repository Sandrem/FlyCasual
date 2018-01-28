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
            HostShip.OnDefence += RemoveDarkCursePilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsDefender -= AddDarkCursePilotAbility;
            HostShip.OnDefence -= RemoveDarkCursePilotAbility;
        }

        private void AddDarkCursePilotAbility()
        {
            if ((Combat.AttackStep == CombatStep.Attack) && (Combat.Defender.ShipId == HostShip.ShipId))
            {
                Combat.Attacker.OnTryAddAvailableActionEffect += UseDarkCurseRestriction;
                //TODO: Use assign condition token instead
                Combat.Attacker.AssignToken(new Conditions.DarkCurseCondition(), delegate { });
            }
        }

        private void UseDarkCurseRestriction(ActionsList.GenericAction action, ref bool canBeUsed)
        {
            if (action.IsSpendFocus)
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
                Combat.Attacker.RemoveCondition(typeof(Conditions.DarkCurseCondition));
            }
        }
    }
}

namespace Conditions
{
    public class DarkCurseCondition : Tokens.GenericToken
    {
        public DarkCurseCondition()
        {
            Name = "Debuff Token";
            Temporary = false;
            Tooltip = new Ship.TIEFighter.DarkCurse().ImageUrl;
        }
    }
}