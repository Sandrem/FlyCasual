using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.YV666LightFreighter
    {
        public class LattsRazzi : YV666LightFreighter
        {
            public LattsRazzi() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Latts Razzi",
                    "Martial Artist",
                    Faction.Scum,
                    3,
                    6,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LattsRazziPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.BountyHunter
                    },
                    seImageNumber: 212
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LattsRazziPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostShip.Tokens.HasToken(typeof(BlueTargetLockToken), '*') && IsTargetAvailable())
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToSelectLattsRazziTarget);
            }
        }

        private bool IsTargetAvailable()
        {
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                if (ActionsHolder.HasTargetLockOn(HostShip, ship))
                {
                    DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                    if (distInfo.Range == 1) return true;
                }
            }

            return false;
        }

        private void AskToSelectLattsRazziTarget(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                TargetIsSelected,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a ship at range 1 and spend a lock you have on that ship - that ship gains 1 Tractor Beam token",
                HostShip
            );
        }

        private void TargetIsSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " causes " + TargetShip.PilotInfo.PilotName + " to gain a Tractor Beam token");

            List<char> tlLetter = ActionsHolder.GetTargetLocksLetterPairs(HostShip, TargetShip);
            HostShip.Tokens.SpendToken(typeof(BlueTargetLockToken), ApplyAbility, tlLetter.First());
        }

        private void ApplyAbility()
        {
            TargetShip.Tokens.AssignToken(
                new TractorBeamToken(TargetShip, HostShip.Owner),
                Triggers.FinishTrigger
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            if (!ActionsHolder.HasTargetLockOn(HostShip, ship)) return false;

            if (HostShip.ShipsBumped.Contains(ship)) return false;

            if (!FilterTargetsByRange(ship, 1, 1)) return false;

            return true;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }
    }
}
