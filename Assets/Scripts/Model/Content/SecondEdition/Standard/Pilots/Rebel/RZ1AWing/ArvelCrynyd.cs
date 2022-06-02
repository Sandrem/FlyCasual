using Actions;
using ActionsList;
using Content;
using Movement;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class ArvelCrynyd : RZ1AWing
        {
            public ArvelCrynyd() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Arvel Crynyd",
                    "Green Leader",
                    Faction.Rebel,
                    3,
                    3,
                    5,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ArvelCrynydAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.AWing
                    },
                    seImageNumber: 20
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ArvelCrynydAbility : GenericAbility
    {
        private GenericMovement SavedManeuver;
        private GenericAction ActionToRevert;

        private static readonly List<string> ChangedManeuversCodes = new List<string>() { "1.L.B", "1.F.S", "1.R.B" };
        private Dictionary<string, MovementComplexity> SavedManeuverColors;

        public override void ActivateAbility()
        {
            HostShip.PrimaryWeapons.ForEach(n => n.WeaponInfo.MinRange = 0);

            HostShip.OnActionIsReadyToBeFailed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.PrimaryWeapons.ForEach(n => n.WeaponInfo.MinRange = 1);

            HostShip.OnActionIsReadyToBeFailed += CheckAbility;
        }
        
        private void CheckAbility(GenericAction action, List<ActionFailReason> failReasons, ref bool isDefaultFailOverwritten)
        {
            // TODO: Real fail reasons
            if (action is BoostAction
                && failReasons.Count == 1
                && failReasons.First() == ActionFailReason.Bumped
            )
            {
                ActionToRevert = action;
                isDefaultFailOverwritten = true;

                RegisterAbilityTrigger(TriggerTypes.OnActionIsReadyToBeFailed, DoPseudoBoost);
            }
        }

        private void DoPseudoBoost(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " is resolving Boost as a maneuver");

            SavedManeuver = HostShip.AssignedManeuver;

            SavedManeuverColors = new Dictionary<string, MovementComplexity>();
            foreach (var changedManeuver in ChangedManeuversCodes)
            {
                KeyValuePair<ManeuverHolder, MovementComplexity> existingManeuver = (HostShip.DialInfo.PrintedDial.FirstOrDefault(n => n.Key.ToString() == changedManeuver));
                SavedManeuverColors.Add(changedManeuver, (existingManeuver.Equals(default(KeyValuePair<ManeuverHolder, MovementComplexity>))) ? MovementComplexity.None : existingManeuver.Value);
                HostShip.Maneuvers[changedManeuver] = MovementComplexity.Normal;
            }

            // Direction from action
            HostShip.SetAssignedManeuver(
                ShipMovementScript.MovementFromString(
                    ManeuverFromBoostTemplate(
                        (ActionToRevert as BoostAction).SelectedBoostTemplate
                    )
                )
            );

            HostShip.AssignedManeuver.IsRevealDial = false;
            HostShip.AssignedManeuver.GrantedBy = HostShip.PilotInfo.PilotName;
            ShipMovementScript.LaunchMovement(FinishAbility);
        }

        private void FinishAbility()
        {
            (ActionToRevert as BoostAction).SelectedBoostTemplate = null;

            HostShip.SetAssignedManeuver(SavedManeuver);

            foreach (var changedManeuver in ChangedManeuversCodes)
            {
                if (SavedManeuverColors[changedManeuver] == MovementComplexity.None)
                {
                    HostShip.Maneuvers.Remove(changedManeuver);
                }
                else
                {
                    HostShip.Maneuvers[changedManeuver] = SavedManeuverColors[changedManeuver];
                }
            }

            Triggers.FinishTrigger();
        }

        private string ManeuverFromBoostTemplate(string boostTemplateName)
        {
            switch (boostTemplateName)
            {
                case "Straight 1":
                    return "1.F.S";
                case "Bank 1 Left":
                    return "1.L.B";
                case "Bank 1 Right":
                    return "1.R.B";
                case "Turn 1 Right":
                    return "1.R.T";
                case "Turn 1 Left":
                    return "1.L.T";
            }

            return "1.F.S";
        }
    }
}