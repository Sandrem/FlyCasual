using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class LukeSkywalker : XWing
        {
            public LukeSkywalker(Player playerNo, int shipId, Vector3 position) : base(playerNo, shipId, position)
            {
                PilotName = "Luke Skywalker";
                isUnique = true;
                PilotSkill = 8;
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);

                AfterGenerateDiceModifications += AddLukeSkywalkerPilotAbility;
            }

            public void AddLukeSkywalkerPilotAbility(ref Dictionary<string, DiceModification> dict)
            {
                if (Game.Combat.AttackStep == CombatStep.Defence)
                {
                    dict.Add("Luke Skywalker's ability", UseLukeSkywalkerPilotAbility);
                }
            }

            private void UseLukeSkywalkerPilotAbility()
            {
                Game.Combat.CurentDiceRoll.ChangeOne(DiceSide.Focus, DiceSide.Success);
            }
        }
    }
}
