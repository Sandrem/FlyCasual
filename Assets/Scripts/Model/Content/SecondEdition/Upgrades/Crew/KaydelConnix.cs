using Ship;
using Upgrade;
using System.Linq;
using Tokens;
using System;
using GameModes;
using Movement;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class KaydelConnix : GenericUpgrade
    {
        public KaydelConnix() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Kaydel Connix",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.KaydelConnixCrewAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/29a0eb418850a3821c38874daf0a6b0d.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class KaydelConnixCrewAbility : GenericAbility
    {
        private Dictionary<string, MovementComplexity> OriginalManeuvers = new Dictionary<string, MovementComplexity>();

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
            if (!HostShip.AssignedManeuver.IsIonManeuver)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);
            }
        }

        protected void AskChangeManeuver(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                "Kaydel Connix",
                NeverUseByDefault,
                ShowChangeManeuver,
                descriptionLong: "Do you want to change maneuver to basic maneuver of the next higher speed? (Difficulty will be increased)",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void ShowChangeManeuver(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            ModifyManeuversComplexity();

            HostShip.Owner.ChangeManeuver(
                (maneuverCode) => { 
                    Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": Maneuver was changed");
                    ShipMovementScript.SendAssignManeuverCommand(maneuverCode);
                    HostShip.OnMovementFinish += RestoreManuvers;
                },
                Triggers.FinishTrigger,
                IsNextHigherSpeedBasic
            );

        }

        private void RestoreManuvers(GenericShip ship)
        {
            HostShip.OnMovementFinish -= RestoreManuvers;

            foreach (var maneuverHolder in OriginalManeuvers)
            {
                HostShip.Maneuvers[maneuverHolder.Key] = maneuverHolder.Value;
            }

            OriginalManeuvers.Clear();
        }

        private void ModifyManeuversComplexity()
        {
            OriginalManeuvers.Clear();

            foreach (var maneuverHolder in HostShip.GetManeuvers())
            {
                if (IsNextHigherSpeedBasic(maneuverHolder.Key))
                {
                    OriginalManeuvers.Add(maneuverHolder.Key, maneuverHolder.Value);
                    HostShip.Maneuvers[maneuverHolder.Key] = GenericMovement.IncreaseComplexity(maneuverHolder.Value);
                }
            }
        }

        private bool IsNextHigherSpeedBasic(string maneuverString)
        {
            bool result = false;

            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);

            if (movementStruct.SpeedIntUnsigned == Selection.ThisShip.AssignedManeuver.Speed + 1
                && (movementStruct.Bearing == ManeuverBearing.Straight
                    || movementStruct.Bearing == ManeuverBearing.Bank
                    || movementStruct.Bearing == ManeuverBearing.Turn
                )
            )
            {
                result = true;
            }

            return result;
        }
    }
}