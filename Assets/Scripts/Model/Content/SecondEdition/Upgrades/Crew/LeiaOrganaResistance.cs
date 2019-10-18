using Ship;
using Upgrade;
using SubPhases;
using System.Linq;
using Movement;
using System.Collections.Generic;
using ActionsList;
using Actions;
using System;

namespace UpgradesList.SecondEdition
{
    public class LeiaOrganaResistance : GenericUpgrade
    {
        public LeiaOrganaResistance() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Leia Organa (Resistance)",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Crew,
                    UpgradeType.Crew
                },
                cost: 19,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                addAction: new ActionInfo(typeof(CoordinateAction), ActionColor.Purple),
                addForce: 1,
                abilityType: typeof(Abilities.SecondEdition.LeiaOrganaResistanceAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/0a194c8c529278b471e64edc597b06fc.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class LeiaOrganaResistanceAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnManeuverIsRevealedGlobal += TryRegisterAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnManeuverIsRevealedGlobal -= TryRegisterAbility;
        }

        private void TryRegisterAbility(GenericShip ship)
        {
            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo
                && HostShip.State.Force > 0
                && Selection.ThisShip.RevealedManeuver.ColorComplexity != MovementComplexity.Easy
            )
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                "Leia Organa",
                UseIfRedOnly,
                DecreaseComplexityOfManeuver,
                descriptionLong: "Do you want to spen 1 Force to reduce difficuly of revealed maneuver?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void DecreaseComplexityOfManeuver(object sender, EventArgs e)
        {
            HostShip.State.Force--;

            Selection.ThisShip.AssignedManeuver.ColorComplexity = GenericMovement.ReduceComplexity(Selection.ThisShip.AssignedManeuver.ColorComplexity);

            // Update revealed dial in UI
            Roster.UpdateAssignedManeuverDial(Selection.ThisShip, Selection.ThisShip.AssignedManeuver);

            Messages.ShowInfo("Leia Organa: Difficulty of maneuver is reduced");

            DecisionSubPhase.ConfirmDecision();
        }

        private bool UseIfRedOnly()
        {
            return Selection.ThisShip.RevealedManeuver.ColorComplexity == MovementComplexity.Complex;
        }
    }
}