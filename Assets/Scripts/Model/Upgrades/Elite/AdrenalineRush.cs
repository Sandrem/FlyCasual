using Abilities;
using Movement;
using Ship;
using SubPhases;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList
{
    public class AdrenalineRush : GenericUpgrade
    {
        public AdrenalineRush() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Adrenaline Rush";
            Cost = 1;

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
            Phases.OnRoundStart += CanUseRedmaneuversWhenStressed;

            HostShip.OnManeuverIsRevealed += CheckTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.OnRoundStart -= CanUseRedmaneuversWhenStressed;

            HostShip.OnManeuverIsRevealed -= CheckTrigger;
        }

        //The card allows pilots to be able to reveal red maneuvers while stressed and convert them into white maneuvers.
        private void CanUseRedmaneuversWhenStressed()
        {
            if (!HostUpgrade.isDiscarded)
            {
                HostShip.CanPerformRedManeuversWhileStressed = true;
            }
        }

        private void CheckTrigger(GenericShip host)
        {
            //Adrenaline Rush cannot be used to treat the red L.T / R.T 
            //maneuver caused by a faceup Damaged Engine damage card as a white maneuver

            //X: Wing FAQ: Version 3.1.1 
            if (HostShip.HasToken(typeof(DamagedEngineCritToken)))
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
            if (HostShip.HasToken(typeof(StressToken)))
            {
                Messages.ShowInfoToHuman(string.Format("Adrenaline Rush: {0} is stressed, must use Adrenaline Rush", HostShip.PilotName));
                ChangeManeuverColorAbility(Triggers.FinishTrigger);

                //This should only run when Adrenaline Rush is being forced on the user.
                HostShip.CanPerformRedManeuversWhileStressed = false;
            }
            else
            {
                AskToUseAbility(NeverUseByDefault, delegate { ChangeManeuverColorAbility(DecisionSubPhase.ConfirmDecision); });
            }
        }

        private void ChangeManeuverColorAbility(Action callback)
        {
            GenericMovement movement = HostShip.AssignedManeuver;
            movement.ColorComplexity = ManeuverColor.White;

            HostShip.SetAssignedManeuver(movement);

            HostUpgrade.TryDiscard(callback);
            Messages.ShowInfoToHuman("Adrenaline Rush: ability used, card discarded");
        }
    }
}