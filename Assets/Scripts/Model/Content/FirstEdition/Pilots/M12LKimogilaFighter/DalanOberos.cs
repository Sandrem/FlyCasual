using Arcs;
using BoardTools;
using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.M12LKimogilaFighter
    {
        public class DalanOberos : M12LKimogilaFighter
        {
            public DalanOberos() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Dalan Oberos",
                    7,
                    25,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.DalanOberosAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                ModelInfo.SkinName = "Dalan Oberos";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class DalanOberosAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbilityTrigger;
        }


        private void RegisterAbilityTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, SelectShipForAbility);
        }

        private void SelectShipForAbility(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                AcquireTargetLock,
                FilterTargetInBullseyeArc,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotName,
                "Acqure a Target Lock on an enemy ship inside your bullseye firing arc.",
                HostShip
            );
        }

        private bool FilterTargetInBullseyeArc(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapon);
            return shotInfo.InArcByType(ArcType.Bullseye) && FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) && FilterTargetsByRange(ship, 1, 3);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;

            ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapon);
            result += (3 - shotInfo.Range) * 100;

            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);

            return result;
        }

        private void AcquireTargetLock()
        {
            ActionsHolder.AcquireTargetLock(HostShip, TargetShip, SuccessfullSelection, UnSuccessfullSelection);
        }

        private void SuccessfullSelection()
        {
            SelectShipSubPhase.FinishSelection();
        }

        private void UnSuccessfullSelection()
        {
            Messages.ShowErrorToHuman("Cannot aquire Target Lock");
        }
    }
}