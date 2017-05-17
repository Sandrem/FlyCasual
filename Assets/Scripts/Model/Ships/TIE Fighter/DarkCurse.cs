using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class DarkCurse : TIEFighter
        {
            public DarkCurse(Players.PlayerNo playerNo, int shipId, Vector3 position) : base(playerNo, shipId, position)
            {
                PilotName = "\"Dark Curse\"";
                isUnique = true;
                PilotSkill = 6;
                OnAttack += AddDarkCursePilotAbility;
                OnDefence += RemoveDarkCursePilotAbility;
            }

            public void AddDarkCursePilotAbility()
            {
                if ((Game.Combat.AttackStep == CombatStep.Attack) && (Game.Combat.Defender.PilotName == PilotName)) {
                    //Game.UI.ShowError("Dark Curse: Debuf On");
                    Game.Combat.Attacker.OnTrySpendFocus += UseDarkCurseFocusRestriction;
                    Game.Combat.Attacker.OnTryReroll += UseDarkCurseRerollRestriction;
                }
            }

            private void UseDarkCurseFocusRestriction(ref bool result)
            {
                Game.UI.ShowError("Dark Curse: Cannot spend focus");
                result = false;
            }

            private void UseDarkCurseRerollRestriction(ref bool result)
            {
                Game.UI.ShowError("Dark Curse: Cannot reroll");
                result = false;
            }

            public void RemoveDarkCursePilotAbility()
            {
                if ((Game.Combat.AttackStep == CombatStep.Defence) && (Game.Combat.Defender.PilotName == PilotName))
                {
                    //Game.UI.ShowInfo("Dark Curse: Debuf Off");
                    Game.Combat.Attacker.OnTrySpendFocus -= UseDarkCurseFocusRestriction;
                    Game.Combat.Attacker.OnTryReroll -= UseDarkCurseRerollRestriction;
                }
            }

        }
    }
}
