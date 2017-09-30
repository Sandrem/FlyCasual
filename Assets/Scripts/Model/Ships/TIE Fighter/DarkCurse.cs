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
                ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/4/49/Dark_Curse.png";
                IsUnique = true;
                PilotSkill = 6;
                Cost = 16;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                OnAttack += AddDarkCursePilotAbility;
                OnDefence += RemoveDarkCursePilotAbility;
            }

            public void AddDarkCursePilotAbility()
            {
                if ((Combat.AttackStep == CombatStep.Attack) && (Combat.Defender.PilotName == PilotName))
                {
                    Combat.Attacker.OnTryAddAvailableActionEffect += UseDarkCurseRestriction;
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

            public void RemoveDarkCursePilotAbility()
            {
                if ((Combat.AttackStep == CombatStep.Defence) && (Combat.Defender.PilotName == PilotName))
                {
                    Combat.Attacker.OnTryAddAvailableActionEffect -= UseDarkCurseRestriction;
                    Combat.Attacker.RemoveToken(typeof(Conditions.DarkCurseCondition));
                }
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