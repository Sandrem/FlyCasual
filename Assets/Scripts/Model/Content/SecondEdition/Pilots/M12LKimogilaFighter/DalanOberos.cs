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
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.ToraniKuldaAbility),
                    charges: 2,
                    extraUpgradeIcon: UpgradeType.Elite,
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
                    ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapon);
                    if (shotInfo.InArcByType(ArcTypes.Bullseye) && shotInfo.Range < 4) return true;
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
                HostShip.PilotName,
                "Choose a shielded ship in your bullseye arc and spend a charge - that ship loses 1 shield and you recover 1 shield",
                HostShip
            );
        }

        private void TargetIsSelected()
        {
            Messages.ShowInfo("Dalan Oberos: " + TargetShip.PilotName + " is selected");

            HostShip.SpendCharge();
            TargetShip.LoseShield();
            HostShip.TryRegenShields();

            SelectShipSubPhase.FinishSelection();
        }

        private bool FilterTargets(GenericShip ship)
        {
            if (ship.State.ShieldsCurrent == 0) return false;

            if (!FilterTargetsByRange(ship, 0, 3)) return false;

            ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapon);
            return shotInfo.InArcByType(ArcTypes.Bullseye);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }
    }
}