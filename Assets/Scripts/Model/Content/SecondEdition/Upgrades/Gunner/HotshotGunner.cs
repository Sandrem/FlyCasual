using Ship;
using Upgrade;
using System.Collections.Generic;
using Tokens;
using System.Linq;
using ActionsList;
using System;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class HotshotGunner : GenericUpgrade
    {
        public HotshotGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hotshot Gunner",
                UpgradeType.Gunner,
                cost: 6,
                abilityType: typeof(Abilities.SecondEdition.HotshotGunnerAbility),
                seImageNumber: 49
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(355, 9),
                new Vector2(125, 125)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class HotshotGunnerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += StartAbilityDefender;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= StartAbilityDefender;
            if (Combat.Defender != null) Combat.Defender.OnAfterModifyDefenseDiceStep -= RegisterAbility;
        }

        private void StartAbilityDefender()
        {
            if (Combat.ArcForShot.ArcType == Arcs.ArcType.SingleTurret)
            {
                Messages.ShowInfo("Hotshot Gunner: After the Modify Defense Dice step, the defender removes 1 focus or calculate token");
                Combat.Defender.OnAfterModifyDefenseDiceStep += RegisterAbility;
            }
        }

        private void RegisterAbility(GenericShip ship)
        {
            Combat.Defender.OnAfterModifyDefenseDiceStep -= RegisterAbility;

            RegisterAbilityTrigger(TriggerTypes.OnAfterModifyDefenseDiceStep, CheckTokens);
        }

        private void CheckTokens(object sender, EventArgs e)
        {
            if (Combat.Defender.Tokens.HasToken<FocusToken>() && Combat.Defender.Tokens.HasToken<CalculateToken>())
            {
                AskWhatTokenToRemove();
            }
            else if (Combat.Defender.Tokens.HasToken<FocusToken>())
            {
                Messages.ShowInfo("Hotshot Gunner: Focus token is removed");
                RemoveTokenAndFinish(typeof(FocusToken));
            }
            else if (Combat.Defender.Tokens.HasToken<CalculateToken>())
            {
                Messages.ShowInfo("Hotshot Gunner: Calculate token is removed");
                RemoveTokenAndFinish(typeof(CalculateToken));
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void AskWhatTokenToRemove()
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                delegate {
                    SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();
                    RemoveTokenAndFinish(typeof(CalculateToken));
                },
                delegate {
                    SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();
                    RemoveTokenAndFinish(typeof(FocusToken));
                },
                descriptionLong: "Do you want to remove Calculate token and stay with Focus token?",
                imageHolder: HostUpgrade,
                showSkipButton: false,
                requiredPlayer: Combat.Defender.Owner.PlayerNo
            );
        }

        private void RemoveTokenAndFinish(Type tokenType)
        {
            Combat.Defender.Tokens.RemoveToken(tokenType, Triggers.FinishTrigger);
        }
    }
}