using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace Ship
{
    namespace M12LKimogila
    {
        public class DalanOberos : M12LKimogila
        {
            public DalanOberos() : base()
            {
                PilotName = "Dalan Oberos";
                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/19/b3/19b3a603-f973-4064-ab69-fdf72f6d79e8/swx70-dalan-oberos.png";
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
    public class DalanOberosAbility : GenericPilotAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            Phases.OnCombatPhaseStart += RegisterAbilityTrigger;
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
            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Host, TargetShip, Host.PrimaryWeapon);
            if (shotInfo.InBullseyeArc)
            {
                Messages.ShowErrorToHuman("Target Lock is aquired");
                Actions.AssignTargetLockToPair(Host, TargetShip, SuccessfullSelection, UnSuccessfullSelection);
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
