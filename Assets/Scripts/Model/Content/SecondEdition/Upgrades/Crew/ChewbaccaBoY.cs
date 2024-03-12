using Ship;
using Upgrade;
using System;
using Tokens;
using System.Linq;
using UnityEngine;
using ActionsList;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class ChewbaccaBoY : GenericUpgrade
    {
        public ChewbaccaBoY() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo
            (
                "Chewbacca",
                UpgradeType.Crew,
                cost: 0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.ChewbaccaBoYAbility)
            );

            NameCanonical = "chewbacca-battleofyavin";

            ImageUrl = "https://i.imgur.com/prWwkwj.jpg";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ChewbaccaBoYAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (action is EvadeAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToGetFocusToken);
            }
        }

        private void AskToGetFocusToken(object sender, EventArgs e)
        {
            if (alwaysUseAbility)
            {
                Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: {HostShip.PilotInfo.PilotName} gains Focus token");
                HostShip.Tokens.AssignToken(typeof(FocusToken), Triggers.FinishTrigger);
            }
            else
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    AlwaysUseByDefault,
                    GainFocusToken,
                    showAlwaysUseOption: true,
                    descriptionLong: "Do you want to gain 1 Focus token?",
                    requiredPlayer: HostShip.Owner.PlayerNo
                );
            }
        }

        private void GainFocusToken(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: {HostShip.PilotInfo.PilotName} gains Focus token");
            HostShip.Tokens.AssignToken(typeof(FocusToken), Triggers.FinishTrigger);
        }
    }
}