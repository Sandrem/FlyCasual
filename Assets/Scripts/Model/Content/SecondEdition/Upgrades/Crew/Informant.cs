using Ship;
using Upgrade;
using UnityEngine;
using SubPhases;
using Conditions;
using System.Linq;
using Tokens;
using BoardTools;
using ActionsList;
using System;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class Informant : GenericUpgrade
    {
        public Informant() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Informant",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.InformantAbility),
                seImageNumber: 44
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class InformantAbility : GenericAbility
    {       
        protected virtual string Prompt
        {
            get
            {
                return "Choose 1 enemy ship and assign the Listening device condition to it.";
            }
        }

        public override void ActivateAbility()
        {
            Phases.Events.OnSetupStart += RegisterInformantAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupStart -= RegisterInformantAbility;
        }

        private void RegisterInformantAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Informant target",
                TriggerType = TriggerTypes.OnSetupStart,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = SelectInformantTarget
            });
        }

        private void SelectInformantTarget(object Sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                  AssignListeningDevice,
                  CheckRequirements,
                  GetAiListeningDevicePriority,
                  HostShip.Owner.PlayerNo,
                  HostUpgrade.UpgradeInfo.Name,
                  Prompt,
                  HostUpgrade
              );
        }

        protected virtual void AssignListeningDevice()
        {
            TargetShip.Tokens.AssignCondition(typeof(ListeningDevice));
            
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Informant target",
                TriggerType = TriggerTypes.OnSystemsPhaseStart,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = InformantRevealDial,
                Skippable = true
            });
            
            SelectShipSubPhase.FinishSelection();
        }

        protected virtual bool CheckRequirements(GenericShip ship)
        {
            var match = ship.Owner.PlayerNo != HostShip.Owner.PlayerNo;
            return match;
        }

        private int GetAiListeningDevicePriority(GenericShip ship)
        {
            int result = 0;
            result = ship.State.Initiative;
            return result;
        }
        
        private void InformantRevealDial(object Sender, System.EventArgs e)
        {
            // Listening Device: During the System Phase, if an enemy ship with the 
            // Informant upgrade is at range 0-2, flip your dial faceup.
            if (new BoardTools.DistanceInfo(HostShip, TargetShip).Range < 3)
            {
                Messages.ShowInfo("Listening Device: " + TargetShip.PilotInfo.PilotName + " flip your dial faceup");
                Roster.ToggleManeuverVisibility(TargetShip, true);
                TargetShip.AlwaysShowAssignedManeuver = true;
            }
        }
    }
}

namespace Conditions
{
    // Listening Device:
    // During the System Phase, if an enemy ship with the Informant upgrade is at range 0-2, flip your dial faceup.
    public class ListeningDevice : GenericToken
    {
        public ListeningDevice(GenericShip host) : base(host)
        {
            Name = "Informant Condition";
            Temporary = false;

            Tooltip = "https://github.com/belk/xwing-data2-test/raw/listening-device/images/conditions/listening-device.png";
        }
    }
}
