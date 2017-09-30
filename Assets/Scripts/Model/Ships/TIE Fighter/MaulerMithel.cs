﻿using System.Collections;
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
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                AfterGotNumberOfPrimaryWeaponAttackDice += MaulerMithelPilotAbility;
            }

            private void MaulerMithelPilotAbility(ref int result)
            {
                Board.ShipShotDistanceInformation shotInformation = new Board.ShipShotDistanceInformation(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
                if (shotInformation.Range == 1)
                {
                    Messages.ShowInfo("\"Mauler Mithel\": +1 attack die");
                    result++;
                }
            }

        }
    }
}
