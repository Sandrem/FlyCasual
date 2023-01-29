using Ship;
using Upgrade;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Tokens;
using System.Linq;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class BurnoutThrusters : GenericUpgrade
    {
        public BurnoutThrusters() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Burnout Thrusters",
                UpgradeType.Modification,
                cost: 6,
                restrictions: new UpgradeCardRestrictions(
                    new BaseSizeRestriction(BaseSize.Small, BaseSize.Medium),
                    new FactionRestriction(Faction.Scum)
                ),
                charges: 1,
                addAction: new ActionInfo(typeof(SlamAction)),
                abilityType: typeof(Abilities.SecondEdition.BurnoutThrustersAbility)
            );

            ImageUrl = "https://infinitearenas.com/xw2/images/upgrades/burnoutthrusters.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class BurnoutThrustersAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTryAddAction += RestrictSlam;
            HostShip.OnSlam += LoseCharge;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTryAddAction -= RestrictSlam;
            HostShip.OnSlam -= LoseCharge;
        }

        private void RestrictSlam(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (action is SlamAction)
            {
                if (canBeUsed) canBeUsed = HostUpgrade.State.Charges > 0;
            }
        }

        private void LoseCharge()
        {
            if (HostUpgrade.State.Charges > 0)
            {
                HostUpgrade.State.LoseCharge();
                RegisterAbilityTrigger(TriggerTypes.OnSlam, AskToReplaceToken);
            }
        }

        private void AskToReplaceToken(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                ReplaceToken,
                descriptionLong: "Do you want to gain 1 Deplete token to remove Disarm token?",
                imageHolder: HostUpgrade
            );
        }

        private void ReplaceToken(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.AssignToken(typeof(DepleteToken), RemoveDisarmToken);
        }

        private void RemoveDisarmToken()
        {
            HostShip.Tokens.RemoveToken(typeof(WeaponsDisabledToken), Triggers.FinishTrigger);
        }
    }
}
