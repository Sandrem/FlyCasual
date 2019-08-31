﻿using BoardTools;
using System;
using System.Collections;
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
                PilotInfo = new PilotCardInfo(
                    "Han Solo",
                    6,
                    82,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HanSoloRebelPilotAbility),
                    charges: 1,
                    regensCharges: true,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 69
                );
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
