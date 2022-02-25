using Upgrade;
using Ship;
using SubPhases;
using Movement;
using UnityEngine;
using Content;

namespace UpgradesList.SecondEdition
{
    public class PrecisionIonEngines : GenericUpgrade
    {
        public PrecisionIonEngines() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Precision Ion Engines",
                UpgradeType.Modification,
                cost: 2,
                charges: 2,
                restriction: new TagRestriction(Tags.Tie),
                abilityType: typeof(Abilities.SecondEdition.PrecisionIonEnginesAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/bb/fb/bbfb4727-e2f5-4f23-be9a-3341ea4de7b5/swz80_upgrade_precison-ion-engines.png";
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipInfo.Agility == 3;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PrecisionIonEnginesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterAskChangeManeuver;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterAskChangeManeuver;
        }

        private void RegisterAskChangeManeuver(GenericShip ship)
        {
            if (HostShip.RevealedManeuver != null
                && HostShip.RevealedManeuver.Bearing == ManeuverBearing.KoiogranTurn
                && HostShip.RevealedManeuver.Speed >= 1
                && HostShip.RevealedManeuver.Speed <= 3
                && HostUpgrade.State.Charges > 0
            )
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);
            }
        }

        private void AskChangeManeuver(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                "Precision Ion Engines",
                NeverUseByDefault,
                ChangeManeuver,
                descriptionLong: "Do you want to spend 1 charge to execute Segnor's Loop instead?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void ChangeManeuver(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharge();

            HostShip.Maneuvers.Add($"{HostShip.RevealedManeuver.Speed}.L.R", MovementComplexity.Complex);
            HostShip.Maneuvers.Add($"{HostShip.RevealedManeuver.Speed}.R.R", MovementComplexity.Complex);

            HostShip.OnMovementFinish += RemoveAddedManeuvers;

            HostShip.Owner.ChangeManeuver(
                ShipMovementScript.SendAssignManeuverCommand,
                Triggers.FinishTrigger,
                IsSameSegnorsLoop
            );
        }

        private void RemoveAddedManeuvers(GenericShip ship)
        {
            HostShip.OnMovementFinish -= RemoveAddedManeuvers;

            HostShip.Maneuvers.Remove($"{HostShip.RevealedManeuver.Speed}.L.R");
            HostShip.Maneuvers.Remove($"{HostShip.RevealedManeuver.Speed}.R.R");
        }

        private bool IsSameSegnorsLoop(string maneuverString)
        {
            bool result = false;
            
            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);
            
            if (movementStruct.Speed == HostShip.RevealedManeuver.ManeuverSpeed
                && movementStruct.Bearing == ManeuverBearing.SegnorsLoop
            )
            {
                result = true;
            }

            return result;
        }
    }
}