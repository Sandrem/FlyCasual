using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEVnSilencer
    {
        public class Blackout : TIEVnSilencer
        {
            public Blackout() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Blackout\"",
                    "Ill-Fated Test Pilot",
                    Faction.FirstOrder,
                    5,
                    6,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TestPilotBlackoutAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Torpedo
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/c5659b210e13b4e11fdd5f1396f2847c.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TestPilotBlackoutAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += TryAddTestPilotBlackoutAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= TryAddTestPilotBlackoutAbility;
        }

        private void TryAddTestPilotBlackoutAbility()
        {
            if (Combat.ShotInfo.IsObstructedByObstacle)
            {
                Combat.Defender.AfterGotNumberOfDefenceDice += DecreaseDiceResult;
            }
        }

        private void DecreaseDiceResult(ref int count)
        {
            Messages.ShowInfo("Test Pilot \"Blackout\"'s attack is obstructed,\n the defender rolls 2 fewer defense dice");
            count -= 2;
            Combat.Defender.AfterGotNumberOfDefenceDice -= DecreaseDiceResult;
        }
    }
}
