using Ship;
using Upgrade;
using System.Linq;
using System.Collections.Generic;
using Tokens;
using BoardTools;
using ActionsList;
using System;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class GeneralHux : GenericUpgrade
    {
        public GeneralHux() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "General Hux",
                UpgradeType.Crew,
                cost: 6,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.FirstOrder),
                    new ActionBarRestriction(typeof(CoordinateAction))
                ),
                abilityType: typeof(Abilities.SecondEdition.GeneralHuxAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/fa0b8492eff625bc66f00bd561015465.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class GeneralHuxAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.BeforeActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action, ref bool isFree)
        {
            if (action is CoordinateAction && action.Color == ActionColor.White)
            {
                RegisterAbilityTrigger(TriggerTypes.BeforeActionIsPerformed, AskToTreatAsRed);
            }
        }

        private void AskToTreatAsRed(object sender, EventArgs e)
        {
            AskToUseAbility(
                NeverUseByDefault,
                TreatActionAsRed,
                infoText: "Do you want to treat action as red to coordinate additional ships?"
            );
        }

        private void TreatActionAsRed(object sender, EventArgs e)
        {
            HostShip.OnCheckActionComplexity += TreatThisCoordinateActionAsRed;
            HostShip.OnCheckCoordinateModeModification += SetCustomCoordinateMode;

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void SetCustomCoordinateMode(ref CoordinateActionData coordinateActionData)
        {
            coordinateActionData.MaxTargets = 3;
            coordinateActionData.SameShipTypeLimit = true;
            coordinateActionData.SameActionLimit = true;
            coordinateActionData.TreatCoordinatedActionAsRed = true;

            HostShip.OnCheckCoordinateModeModification -= SetCustomCoordinateMode;
        }

        private void TreatThisCoordinateActionAsRed(GenericAction action, ref ActionColor color)
        {
            if (action is CoordinateAction && color == ActionColor.White)
            {
                color = ActionColor.Red;
                HostShip.OnCheckActionComplexity -= TreatThisCoordinateActionAsRed;
            }
        }
    }
}