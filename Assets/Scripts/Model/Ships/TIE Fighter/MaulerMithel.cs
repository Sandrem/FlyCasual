using System;
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
                PilotSkill = 7;
                Cost = 17;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.MaulerMithelAbility());
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
            BoardTools.ShipShotDistanceInformation shotInformation = new BoardTools.ShipShotDistanceInformation(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
            if (shotInformation.Range == 1)
            {
                Messages.ShowInfo("\"Mauler Mithel\": +1 attack die");
                result++;
            }
        }
    }
}
