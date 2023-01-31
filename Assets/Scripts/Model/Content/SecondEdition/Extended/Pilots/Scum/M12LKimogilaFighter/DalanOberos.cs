using Arcs;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.M12LKimogilaFighter
    {
        public class DalanOberos : M12LKimogilaFighter
        {
            public DalanOberos() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Dalan Oberos",
                    "Returned from the Grave",
                    Faction.Scum,
                    3,
                    5,
                    12,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DalanOberosKimogilaAbility),
                    charges: 2,
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Astromech,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    },
                    seImageNumber: 208,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ModelInfo.SkinName = "Dalan Oberos";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DalanOberosKimogilaAbility : GenericAbility
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
            if (HostShip.State.Charges > 0 && IsTargetAvailable())
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToSelectDalanOberosTarget);
            }
        }

        private bool IsTargetAvailable()
        {
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                if (ship.State.ShieldsCurrent > 0)
                {
                    if (HostShip.SectorsInfo.IsShipInSector(ship, ArcType.Bullseye) && HostShip.SectorsInfo.RangeToShipBySector(ship, ArcType.Bullseye) <= 3) return true;
                }
            }

            return false;
        }

        private void AskToSelectDalanOberosTarget(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                TargetIsSelected,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a shielded ship in your Bullseye arc and spend a charge - that ship loses 1 shield and you recover 1 shield",
                HostShip
            );
        }

        private void TargetIsSelected()
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": " + TargetShip.PilotInfo.PilotName + " is selected");

            HostShip.SpendCharge();
            TargetShip.LoseShield();
            HostShip.TryRegenShields();

            SelectShipSubPhase.FinishSelection();
        }

        private bool FilterTargets(GenericShip ship)
        {
            if (ship.State.ShieldsCurrent == 0) return false;

            if (!FilterTargetsByRange(ship, 0, 3)) return false;

            return HostShip.SectorsInfo.IsShipInSector(ship, ArcType.Bullseye);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }
    }
}