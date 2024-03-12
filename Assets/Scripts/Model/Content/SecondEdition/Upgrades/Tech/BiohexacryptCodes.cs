using ActionsList;
using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;
namespace UpgradesList.SecondEdition
{
    public class BiohexacryptCodes : GenericUpgrade
    {
        public BiohexacryptCodes() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Biohexacrypt Codes",
                UpgradeType.Tech,
                cost: 1,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.FirstOrder),
                    new ActionBarRestriction(typeof(TargetLockAction))
                ),
                abilityType: typeof(Abilities.SecondEdition.BiohexacryptCodesAbility)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BiohexacryptCodesAbility : GenericAbility
    {
        private GenericShip WrongTarget;

        public override void ActivateAbility()
        {
            HostShip.OnActionTargetIsWrong += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionTargetIsWrong -= CheckAbility;
        }

        private void CheckAbility(GenericAction action, GenericShip wrongTarget)
        {
            if (CanBeCoordinatedIgnoringRange(action, wrongTarget) || CanBeJammedIgnoringRange(action, wrongTarget))
            {
                if (ActionsHolder.HasTargetLockOn(HostShip, wrongTarget))
                {
                    WrongTarget = wrongTarget;
                    RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, AskToSpendTL);
                }
            }
        }

        private bool CanBeCoordinatedIgnoringRange(GenericAction action, GenericShip target)
        {
            return action is CoordinateAction
                && target.Owner.PlayerNo == HostShip.Owner.PlayerNo
                && target.CanBeCoordinated;
        }

        private bool CanBeJammedIgnoringRange(GenericAction action, GenericShip target)
        {
            return action is JamAction
                && target.Owner.PlayerNo != HostShip.Owner.PlayerNo;
        }

        private void AskToSpendTL(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                SpendTLtoAllow,
                descriptionLong: "Do you want to spend your lock to ignore range restriction?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void SpendTLtoAllow(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            char letter = ActionsHolder.GetTargetLocksLetterPairs(HostShip, WrongTarget).First();
            HostShip.Tokens.SpendToken(typeof(BlueTargetLockToken), TargetIsAllowed, letter);
        }

        private void TargetIsAllowed()
        {
            Triggers.FinishTrigger();

            SubPhases.SelectShipSubPhase.SendSelectShipCommand(WrongTarget);
        }
    }
}