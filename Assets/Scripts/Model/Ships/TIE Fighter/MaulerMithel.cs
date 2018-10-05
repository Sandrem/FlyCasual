using BoardTools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuleSets;

namespace Ship
{
    namespace TIEFighter
    {
        public class MaulerMithel : TIEFighter, ISecondEditionPilot
        {
            public MaulerMithel() : base()
            {
                PilotName = "\"Mauler Mithel\"";
                PilotSkill = 7;
                Cost = 17;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.MaulerMithelAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotName = "\"Mauler\" Mithel";
                PilotSkill = 5;
                Cost = 32;

                SEImageNumber = 80;
            }
        }
    }
}

namespace Abilities
{
    public class MaulerMithelAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += MaulerMithelPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= MaulerMithelPilotAbility;
        }

        private void MaulerMithelPilotAbility(ref int result)
        {
            ShotInfo shotInformation = new ShotInfo(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
            if (shotInformation.Range == 1)
            {
                Messages.ShowInfo("\"Mauler Mithel\": +1 attack die");
                result++;
            }
        }
    }
}
