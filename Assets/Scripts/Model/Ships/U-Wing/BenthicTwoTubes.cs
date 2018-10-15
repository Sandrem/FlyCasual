using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using ActionsList;
using Tokens;
using Ship;
using SubPhases;
using RuleSets;

namespace Ship
{
    namespace UWing
    {
        public class BenthicTwoTubes : UWing, ISecondEditionPilot
        {
            public BenthicTwoTubes() : base()
            {
                PilotName = "Benthic Two Tubes";
                PilotSkill = 4;
                Cost = 24;

                IsUnique = true;

                SkinName = "Partisan";

                PilotAbilities.Add(new BenthicTwoTubesAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 47;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                SEImageNumber = 58;
            }
        }
    }
}

namespace Abilities
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
                HostShip.PilotName,
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

            result += (10 * ship.Agility);
            result += (5 - shipFocusTokens);

            return result;
        }
    }
}
