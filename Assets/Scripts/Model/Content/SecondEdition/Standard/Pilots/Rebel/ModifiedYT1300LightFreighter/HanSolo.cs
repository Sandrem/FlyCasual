using BoardTools;
using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ModifiedYT1300LightFreighter
    {
        public class HanSolo : ModifiedYT1300LightFreighter
        {
            public HanSolo() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Han Solo",
                    "Scoundrel for Hire",
                    Faction.Rebel,
                    6,
                    8,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HanSoloRebelPilotAbility),
                    charges: 1,
                    regensCharges: 1,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                        UpgradeType.Title,
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.YT1300
                    },
                    seImageNumber: 69
                );

                PilotNameCanonical = "hansolo-modifiedyt1300lightfreighter";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HanSoloRebelPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsDiceModificationAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                int.MaxValue,
                timing: DiceModificationTimingType.AfterRolled,
                isTrueReroll: false,
                isForcedFullReroll: true
            );
        }

        private bool IsDiceModificationAvailable()
        {
            foreach (var obstacle in Obstacles.ObstaclesManager.GetPlacedObstacles())
            {
                ShipObstacleDistance obstacleDistance = new ShipObstacleDistance(HostShip, obstacle);
                if (obstacleDistance.Range < 2) return true;
            }

            return false;
        }

        private int GetAiPriority()
        {
            if (HostShip.IsAttacking && Combat.DiceRollAttack.Failures > Combat.DiceRollAttack.Successes)
            {
                return 95;
            }
            else if (HostShip.IsDefending && Combat.DiceRollDefence.Failures > Combat.DiceRollDefence.Successes)
            {
                return 95;
            }
            else return 0;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            HostShip.Tokens.AssignToken(typeof(Tokens.StressToken), () => callback(true));
        }
    }
}
