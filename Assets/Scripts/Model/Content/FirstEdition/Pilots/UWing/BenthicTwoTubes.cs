using ActionsList;
using Ship;
using System.Collections;
using System.Collections.Generic;
using SubPhases;
using Tokens;

namespace Ship
{
    namespace FirstEdition.UWing
    {
        public class BenthicTwoTubes : UWing
        {
            public BenthicTwoTubes() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Benthic Two Tubes",
                    4,
                    24,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.BenthicTwoTubesAbility)
                );

                ModelInfo.SkinName = "Partisan";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class BenthicTwoTubesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckBenthicTwoTubesAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckBenthicTwoTubesAbility;
        }

        private void CheckBenthicTwoTubesAbility(GenericAction action)
        {
            if (action is FocusAction && HostShip.Tokens.HasToken(typeof(FocusToken)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, SelectTargetForBenthicTwoTubesAbility);
            }
        }

        private void SelectTargetForBenthicTwoTubesAbility(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                TargetIsSelected,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a ship to assign own Focus token to it.",
                HostShip
            );
        }

        private void TargetIsSelected()
        {
            HostShip.Tokens.RemoveToken(typeof(FocusToken), AssignFocusTokenToTarget);
        }

        private void AssignFocusTokenToTarget()
        {
            TargetShip.Tokens.AssignToken(typeof(FocusToken), SelectShipSubPhase.FinishSelection);
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.OtherFriendly, TargetTypes.This)
                && FilterTargetsByRange(ship, 1, 2);
        }

        private int GetAiPriority(GenericShip ship)
        {
            int result = 0;

            int shipFocusTokens = ship.Tokens.CountTokensByType(typeof(FocusToken));
            if (shipFocusTokens == 0) result += 100;

            result += (10 * ship.State.Agility);
            result += (5 - shipFocusTokens);

            return result;
        }
    }
}

