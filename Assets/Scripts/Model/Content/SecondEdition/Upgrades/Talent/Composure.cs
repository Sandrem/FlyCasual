using Upgrade;
using System.Collections.Generic;
using System.Linq;
using Actions;
using ActionsList;
using Tokens;

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
    }
}