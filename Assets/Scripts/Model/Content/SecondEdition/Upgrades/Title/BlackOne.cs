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
    public class BlackOne : GenericUpgrade
    {
        public BlackOne() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Black One",
                UpgradeType.Title,
                cost: 2,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new ShipRestriction(typeof(Ship.SecondEdition.T70XWing.T70XWing)),
                    new FactionRestriction(Faction.Resistance)
                ),
                charges: 1,
                addAction: new ActionInfo(typeof(SlamAction)),
                abilityType: typeof(Abilities.SecondEdition.BlackOneAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/3e9870bff7f61acc12970c254eaeca89.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class BlackOneAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTryAddAction += RestrictSlam;
            HostShip.OnActionIsPerformed += LoseCharge;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTryAddAction -= RestrictSlam;
            HostShip.OnActionIsPerformed -= LoseCharge;
        }

        private void RestrictSlam(GenericAction action, ref bool canBeUsed)
        {
            if (action is SlamAction)
            {
                if (canBeUsed) canBeUsed = HostUpgrade.State.Charges > 0;
            }
        }

        private void LoseCharge(GenericAction action)
        {
            if (action is SlamAction && HostUpgrade.State.Charges > 0)
            {
                HostUpgrade.State.LoseCharge();
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToReplaceToken);
            };
        }

        private void AskToReplaceToken(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                AlwaysUseByDefault,
                ReplaceToken,
                infoText: "Do you want to gain Ion token to remove Disarm token?"
            );
        }

        private void ReplaceToken(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.AssignToken(typeof(IonToken), RemoveDisarmToken);
        }

        private void RemoveDisarmToken()
        {
            HostShip.Tokens.RemoveToken(typeof(WeaponsDisabledToken), Triggers.FinishTrigger);
        }
    }
}
