using Ship;
using Upgrade;
using UnityEngine;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class CaptainRex : GenericUpgrade
    {
        public CaptainRex() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Captain Rex",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                restrictionFaction: Faction.Rebel,
                abilityType: typeof(Abilities.FirstEdition.CaptainRexAbility)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(84, 0));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class CaptainRexAbility : GenericAbility
    {
        // After you perform an attack that does not hit, you may assign
        // 1 focus token to your ship
        public override void ActivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker += RegisterCaptainRexAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= RegisterCaptainRexAbility;
        }

        private void RegisterCaptainRexAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackMissed, ShowDecision);
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                // give user the option to use ability
                AskToUseAbility(AlwaysUseByDefault, UseAbility, null, null, true);
            }
            else
            {
                Messages.ShowInfoToHuman(HostShip.PilotName + " gained focus from Captain Rex (auto)");
                HostShip.Tokens.AssignToken(typeof(FocusToken), Triggers.FinishTrigger);
            }
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman(HostShip.PilotName + " gained focus from Captain Rex");
            HostShip.Tokens.AssignToken(typeof(FocusToken), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}