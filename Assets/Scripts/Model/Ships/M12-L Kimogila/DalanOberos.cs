using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;
using Abilities;
using System;

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
            Phases.OnCombatPhaseStart += RegisterAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart -= RegisterAbilityTrigger;
        }


        private void RegisterAbilityTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, SelectShipForAbility);
        }

        private void SelectShipForAbility(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                CheckIsTargetInBullseyeArc,
                new List<TargetTypes>() { TargetTypes.Enemy },
                new Vector2(1, 3)
            );
        }

        private void CheckIsTargetInBullseyeArc()
        {
            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(HostShip, TargetShip, HostShip.PrimaryWeapon);
            if (shotInfo.InBullseyeArc)
            {
                Messages.ShowErrorToHuman("Target Lock is aquired");
                Actions.AssignTargetLockToPair(HostShip, TargetShip, SuccessfullSelection, UnSuccessfullSelection);
            }
            else
            {
                Messages.ShowErrorToHuman("Selected ship is not in bullseye arc");
            }
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
