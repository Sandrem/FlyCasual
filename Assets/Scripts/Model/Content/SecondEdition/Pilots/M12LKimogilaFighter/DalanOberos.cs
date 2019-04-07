using Arcs;
using BoardTools;
using Ship;
using SubPhases;
using System;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.M12LKimogilaFighter
    {
        public class DalanOberos : M12LKimogilaFighter
        {
            public DalanOberos() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Dalan Oberos",
                    3,
                    48,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DalanOberosKimogilaAbility),
                    charges: 2,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 208
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
                "Choose a shielded ship in your Bullseye arc and spend a charge - that ship loses 1 shield and you recover 1 shield.",
                HostShip
            );
        }

        private void TargetIsSelected()
        {
            Messages.ShowInfo("Dalan Oberos: " + TargetShip.PilotInfo.PilotName + " is selected.");

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