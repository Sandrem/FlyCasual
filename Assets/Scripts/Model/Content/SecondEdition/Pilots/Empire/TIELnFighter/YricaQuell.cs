using Abilities.SecondEdition;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class YricaQuell : TIELnFighter
        {
            public YricaQuell() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Yrica Quell",
                    "Consumed by Duty",
                    Faction.Imperial,
                    3,
                    3,
                    9,
                    isLimited: true,
                    abilityType: typeof(YricaQuellAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://i.imgur.com/c9QpLZa.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class YricaQuellAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (IsAnyEnemyShipInBullseye())
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToSelectEnemyInBullseye);
            }
        }

        private bool IsAnyEnemyShipInBullseye()
        {
            foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values)
            {
                if (HostShip.SectorsInfo.IsShipInSector(enemyShip, Arcs.ArcType.Bullseye)) return true;
            }

            return false;
        }

        private void AskToSelectEnemyInBullseye(object sender, EventArgs e)
        {
            SelectTargetForAbility
            (
                AcquireLockOnTarget,
                FilterEnemyInBullseye,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "You may acquire a lock on an enemy ship in your bullseye arc",
                imageSource: HostShip
            );
        }

        private void AcquireLockOnTarget()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            ActionsHolder.AcquireTargetLock(HostShip, TargetShip, Triggers.FinishTrigger, Triggers.FinishTrigger);
        }

        private bool FilterEnemyInBullseye(GenericShip ship)
        {
            return Tools.IsAnotherTeam(HostShip, ship) && HostShip.SectorsInfo.IsShipInSector(ship, Arcs.ArcType.Bullseye);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }
    }
}