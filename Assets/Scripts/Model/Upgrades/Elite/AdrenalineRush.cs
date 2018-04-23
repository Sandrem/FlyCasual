using Abilities;
using Movement;
using Ship;
using SubPhases;
using System;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class AdrenalineRush : GenericUpgrade
    {
        public AdrenalineRush() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Adrenaline Rush";
            Cost = 1;

            AvatarOffset = new Vector2(37, 1);

            UpgradeAbilities.Add(new AdrenalineRushAbility());
        }
    }
}

namespace Abilities
{
    //When you reveal a red maneuver, you may discard this card to treat that maneuver as a white maneuver until the end of the Activation phase.
    public class AdrenalineRushAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += CheckTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= CheckTrigger;
        }

        private void CheckTrigger(GenericShip host)
        {
            //Adrenaline Rush cannot be used to treat the red L.T / R.T 
            //maneuver caused by a faceup Damaged Engine damage card as a white maneuver

            //X: Wing FAQ: Version 3.1.1 
            if (HostShip.Tokens.HasToken(typeof(DamagedEngineCritToken)))
            {
                return;
            }

            if (HostShip.AssignedManeuver.ColorComplexity == ManeuverColor.Red)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AskToUseAdrenalineRush);
            }
        }

        private void AskToUseAdrenalineRush(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, delegate { ChangeManeuverColorAbility(DecisionSubPhase.ConfirmDecision); });
        }

        private void ChangeManeuverColorAbility(Action callback)
        {
            GenericMovement movement = HostShip.AssignedManeuver;
            movement.ColorComplexity = ManeuverColor.White;

            HostShip.SetAssignedManeuver(movement);

            Messages.ShowInfoToHuman(string.Format("{0} changed maneuver, Adrenaline Rush is disabled on {0}", HostShip.PilotName));
            HostUpgrade.TryDiscard(callback);
        }
    }
}