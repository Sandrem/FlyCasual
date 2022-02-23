using ActionsList;
using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UT60DUWing
    {
        public class BenthicTwoTubes : UT60DUWing
        {
            public BenthicTwoTubes() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Benthic Two Tubes",
                    "Cavern Angels Marksman",
                    Faction.Rebel,
                    2,
                    5,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BenthicTwoTubesAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Partisan
                    },
                    seImageNumber: 58,
                    skinName: "Partisan"
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
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
                "Choose a ship to assign own Focus token to it",
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