using Upgrade;
using ActionsList;
using Tokens;
using Ship;

namespace UpgradesList.SecondEdition
{
    public class Composure : GenericUpgrade
    {
        public Composure() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Composure",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.SecondEdition.ComposureAbility),
                restriction: new ActionBarRestriction(typeof(FocusAction)),
                seImageNumber: 156
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    //After you fail an action, if you have no green tokens, you may perform a focus action.
    //January 2020 errata: If you do, you cannot perform additional actions this round.
    public class ComposureAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsReallyFailed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsReallyFailed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (!HostShip.Tokens.HasTokenByColor(TokenColors.Green))
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsReallyFailed, ActivateComposure);
            }
        }

        private void ActivateComposure(object sender, System.EventArgs e)
        {
            if (!HostShip.Tokens.HasTokenByColor(TokenColors.Green))
            {
                HostShip.OnActionIsPerformed += SkipActionsUntilEndOfRound;
                HostShip.OnActionIsSkipped += SkipAbility;

                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": You may perform a Focus action");
                HostShip.AskPerformFreeAction(
                    new FocusAction(),
                    Triggers.FinishTrigger,
                    HostUpgrade.UpgradeInfo.Name,
                    "After you fail an action, if you have no green tokens, you may perform a Focus action.",
                    HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SkipAbility(GenericShip ship)
        {
            Messages.ShowInfoToHuman(HostUpgrade.UpgradeInfo.Name + ": Action is skipped");

            HostShip.OnActionIsPerformed -= SkipActionsUntilEndOfRound;
            HostShip.OnActionIsSkipped -= SkipAbility;
        }

        private void SkipActionsUntilEndOfRound(GenericAction action)
        {
            Messages.ShowInfoToHuman(HostUpgrade.UpgradeInfo.Name + ": You cannot perform another action during this round");

            HostShip.OnActionIsPerformed -= SkipActionsUntilEndOfRound;
            HostShip.OnActionIsSkipped -= SkipAbility;

            HostShip.OnTryAddAction += DisallowAction;
            HostShip.OnRoundEnd += ClearRestriction;
        }

        private void DisallowAction(GenericShip ship, GenericAction action, ref bool isAllowed)
        {
            isAllowed = false;
        }

        private void ClearRestriction(GenericShip ship)
        {
            Messages.ShowInfoToHuman(HostUpgrade.UpgradeInfo.Name + ": You can perform actions as usual");

            HostShip.OnRoundEnd -= ClearRestriction;
            HostShip.OnTryAddAction -= DisallowAction;
        }

    }
}