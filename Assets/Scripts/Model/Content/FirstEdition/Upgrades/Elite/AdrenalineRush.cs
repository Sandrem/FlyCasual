using Ship;
using Upgrade;
using UnityEngine;
using Tokens;
using Movement;
using System;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class AdrenalineRush : GenericUpgrade
    {
        public AdrenalineRush() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Adrenaline Rush",
                UpgradeType.Elite,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.AdrenalineRushAbility)
            );

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(37, 1));
        }        
    }
}

namespace Abilities.FirstEdition
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

            if (HostShip.AssignedManeuver.ColorComplexity == MovementComplexity.Complex)
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
            movement.ColorComplexity = MovementComplexity.Normal;

            HostShip.SetAssignedManeuver(movement);

            Messages.ShowInfoToHuman(string.Format("{0} changed maneuver, Adrenaline Rush is disabled on {0}", HostShip.PilotName));
            HostUpgrade.TryDiscard(callback);
        }
    }
}