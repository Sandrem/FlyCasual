using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class Backstabber : TIEFighter
        {
            public Backstabber() : base()
            {
                PilotName = "\"Backstabber\"";
                PilotSkill = 6;
                Cost = 16;

                IsUnique = true;

                PilotAbilities.Add(new PilotAbilitiesNamespace.BackstabberAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class BackstabberAbility : GenericPilotAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            Host.AfterGotNumberOfAttackDice += BackstabberPilotAbility;
        }

        private void BackstabberPilotAbility(ref int diceNumber)
        {
            Board.ShipShotDistanceInformation shotInformation = new Board.ShipShotDistanceInformation(Combat.Defender, Combat.Attacker, Combat.ChosenWeapon);
            if (!shotInformation.InArc)
            {
                Messages.ShowInfo("Backstabber: Additional dice");
                diceNumber++;
            }
        }
    }
}
