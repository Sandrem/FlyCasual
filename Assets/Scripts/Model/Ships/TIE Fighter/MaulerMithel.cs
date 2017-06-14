using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class MaulerMithel : TIEFighter
        {
            public MaulerMithel() : base()
            {
                PilotName = "\"Mauler Mithel\"";
                ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/e/e8/Mauler-mithel.png";
                IsUnique = true;
                PilotSkill = 7;
                Cost = 17;
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                AfterGotNumberOfPrimaryWeaponAttackDices += MaulerMithelPilotAbility;
            }

            private void MaulerMithelPilotAbility(ref int result)
            {
                if (Actions.GetRange(Combat.Attacker, Combat.Defender) == 1)
                {
                    Game.UI.ShowInfo("\"Mauler Mithel\": +1 attack die");
                    result++;
                }
            }

        }
    }
}
