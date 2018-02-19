using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Ship;

namespace Ship
{
    namespace TIESilencer
    {
        public class TestPilotBlackout : TIESilencer
        {
            public TestPilotBlackout() : base()
            {
                PilotName = "Test Pilot \"Blackout\"";
                PilotSkill = 7;
                Cost = 31;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new TestPilotBlackoutAbility());
            }
        }
    }
}

namespace Abilities
{
    public class TestPilotBlackoutAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddTestPilotBlackoutAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= AddTestPilotBlackoutAbility;
        }

        private void AddTestPilotBlackoutAbility()
        {
            if (Selection.ThisShip.ShipId == HostShip.ShipId)
            {
                if (Combat.ShotInfo.IsObstructedByAsteroid)
                {
                    Combat.Defender.AfterGotNumberOfDefenceDice += DecreaseDiceResult;
                }
            }
        }

        private void DecreaseDiceResult(ref int count)
        {
            Messages.ShowInfo("Test Pilot \"Blackout\":\nDefender rolls 2 fewer dice");
            count -= 2;
            Combat.Defender.AfterGotNumberOfDefenceDice -= DecreaseDiceResult;
        }
    }
}
