using Ship;
using Upgrade;
using System.Linq;
using System.Collections.Generic;
using Tokens;
using BoardTools;
using ActionsList;
using System;
using Actions;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class GeneralHux : GenericUpgrade
    {
        public GeneralHux() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "General Hux",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.FirstOrder),
                    new ActionBarRestriction(typeof(CoordinateAction))
                ),
                abilityType: typeof(Abilities.SecondEdition.GeneralHuxAbility)
            );

            Avatar = new AvatarInfo(
                Faction.FirstOrder,
                new Vector2(326, 1)
            );
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
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                TreatActionAsRed,
                descriptionLong: "Do you want to treat action as red? (If you do, you may coordinate up to 2 additional ships of the same ship type, and each ship you coordinate must perform the same action, treating that action as red)",
                imageHolder: HostUpgrade
            );
        }

        private void TreatActionAsRed(object sender, EventArgs e)
        {
            HostShip.OnCheckActionColor += TreatThisCoordinateActionAsRed;
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
                HostShip.OnCheckActionColor -= TreatThisCoordinateActionAsRed;
            }
        }
    }
}