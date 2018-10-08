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
using RuleSets;

namespace Ship
{
    namespace M12LKimogila
    {
        public class DalanOberos : M12LKimogila, ISecondEditionPilot
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

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 48;

                UsesCharges = true;
                MaxCharges = 2;

                PilotAbilities.RemoveAll(a => a is PilotAbilitiesNamespace.DalanOberosAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.DalanOberosKimogilaAbility());

                SEImageNumber = 208;
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
            if (HostShip.Charges > 0 && IsTargetAvailable())
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToSelectDalanOberosTarget);
            }
        }

        private bool IsTargetAvailable()
        {
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                if (ship.Shields > 0)
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
                HostShip.ImageUrl
            );
        }

        private void TargetIsSelected()
        {
            Messages.ShowInfo("Dalan Oberos: " + TargetShip.PilotName + " is selected");

            HostShip.SpendCharge(delegate { }); // Empty delegate is safe - Sandrem
            TargetShip.LoseShield();
            HostShip.TryRegenShields();

            SelectShipSubPhase.FinishSelection();
        }

        private bool FilterTargets(GenericShip ship)
        {
            if (ship.Shields == 0) return false;

            if (!FilterTargetsByRange(ship, 0, 3)) return false;

            ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapon);
            return shotInfo.InArcByType(ArcTypes.Bullseye);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.Cost;
        }
    }
}