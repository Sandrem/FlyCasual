using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;
using Abilities;
using System;
using Ship;
using System.Linq;
using BoardTools;
using Arcs;

namespace Ship
{
    namespace M12LKimogila
    {
        public class DalanOberos : M12LKimogila
        {
            public DalanOberos() : base()
            {
                PilotName = "Dalan Oberos";
                PilotSkill = 7;
                Cost = 25;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new PilotAbilitiesNamespace.DalanOberosAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
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
                true,
                null,
                HostShip.PilotName,
                "Acqure a Target Lock on an enemy ship inside your bullseye firing arc.",
                HostShip.ImageUrl
            );
        }

        private bool FilterTargetInBullseyeArc(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapon);
            return shotInfo.InArcByType(ArcTypes.Bullseye) && FilterByTargetType(ship, new List<TargetTypes>(){ TargetTypes.Enemy }) && FilterTargetsByRange(ship, 1, 3);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;

            ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapon);
            result += (3 - shotInfo.Range) * 100;

            result += ship.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.Cost);

            return result;
        }

        private void AcquireTargetLock()
        {
            Actions.AcquireTargetLock(HostShip, TargetShip, SuccessfullSelection, UnSuccessfullSelection);
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
