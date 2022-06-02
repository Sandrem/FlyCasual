using ActionsList;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESeBomber
    {
        public class Breach : TIESeBomber
        {
            public Breach() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Breach\"",
                    "Ordnance Expert",
                    Faction.FirstOrder,
                    5,
                    5,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BreachPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Torpedo,
                        UpgradeType.Gunner
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://i.imgur.com/Mj8XIc4.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BreachPilotAbility : GenericAbility
    {
        List<GenericShip> EnemyShipsInPath;

        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckAbilityAfterManeuver;
            HostShip.OnActionIsPerformed += CheckAbilityAfterBoost;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckAbilityAfterManeuver;
            HostShip.OnActionIsPerformed -= CheckAbilityAfterBoost;
        }

        private void CheckAbilityAfterManeuver(GenericShip ship)
        {
            EnemyShipsInPath = new List<GenericShip>(ship.ShipsMovedThrough.Where(n => !Tools.IsSameTeam(HostShip, n)).ToList());
            if (EnemyShipsInPath.Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToChooseTarget);
            }
        }

        private void CheckAbilityAfterBoost(GenericAction action)
        {
            if (action is BoostAction)
            {
                EnemyShipsInPath = new List<GenericShip>(HostShip.ShipsBoostedThrough.Where(n => !Tools.IsSameTeam(HostShip, n)).ToList());
                if (EnemyShipsInPath.Count > 0)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToChooseTarget);
                }
            }
        }
        private void AskToChooseTarget(object sender, EventArgs e)
        {
            SelectTargetForAbility
            (
                AquireLockOnTarget,
                OnlyShipsInPath,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "You moved through an enemy ship - you may acquire a lock on it",
                HostShip
            );
        }

        private void AquireLockOnTarget()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            ActionsHolder.AcquireTargetLock
            (
                HostShip,
                TargetShip,
                Triggers.FinishTrigger,
                Triggers.FinishTrigger
            );
        }

        private bool OnlyShipsInPath(GenericShip ship)
        {
            return EnemyShipsInPath.Contains(ship);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }
    }
}
