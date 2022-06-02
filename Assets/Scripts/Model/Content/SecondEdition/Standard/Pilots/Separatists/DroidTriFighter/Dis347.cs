using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class Dis347 : DroidTriFighter
    {
        public Dis347()
        {
            PilotInfo = new PilotCardInfo25
            (
                "DIS-347",
                "Target Acquired",
                Faction.Separatists,
                3,
                4,
                14,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.Dis347Ability),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Missile,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/8f/96/8f96a822-921a-4c77-ae90-2fe15e196171/swz81_dis-347_cutout.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Dis347Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToSelectTarget);
        }

        private void AskToSelectTarget(object sender, EventArgs e)
        {
            if (HasTargetsForAbility())
            {
                SelectTargetForAbility(
                    GetLock,
                    FilterTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    name: "DIS-347",
                    description: "You may acquire a lock on an object at range 1-3 that has a friendly lock",
                    imageSource: HostShip,
                    showSkipButton: true
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool HasTargetsForAbility()
        {
            bool result = false;

            foreach (GenericShip unit in Roster.AllUnits.Values)
            {
                if (FilterTargets(unit))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private void GetLock()
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            ActionsHolder.AcquireTargetLock(
                HostShip,
                TargetShip,
                Triggers.FinishTrigger,
                Triggers.FinishTrigger
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            bool result = false;

            foreach (GenericShip friendlyShip in HostShip.Owner.Ships.Values)
            {
                if (ActionsHolder.HasTargetLockOn(friendlyShip, ship))
                {
                    DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                    if (distInfo.Range >= 1 && distInfo.Range <= 3)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        private int GetAiPriority(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapons);
            return ship.PilotInfo.Cost + ((shotInfo.IsShotAvailable) ? 100 : 0);
        }
    }
}
