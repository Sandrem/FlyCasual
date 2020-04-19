using System;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEPhantom
    {
        public class Whisper : TIEPhantom
        {
            public Whisper() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Whisper\"",
                    7,
                    32,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.WhisperAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class WhisperAbility : GenericAbility
    {
        protected virtual string DescriptionLong => "Do you want to gain 1 Focus Token?";
        protected virtual string TokenIsAssignedMessage => " gains Focus token";

        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsAttacker += RegisterWhisperAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsAttacker -= RegisterWhisperAbility;
        }

        public void RegisterWhisperAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackHit, AskAssignFocus);
        }

        protected virtual Type GetTokenType()
        {
            return typeof(FocusToken);
        }

        private void AskAssignFocus(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    AssignToken,
                    descriptionLong: DescriptionLong,
                    imageHolder: HostShip,
                    showAlwaysUseOption: true
                );
            }
            else
            {
                HostShip.Tokens.AssignToken(GetTokenType(), Triggers.FinishTrigger);
            }
        }

        private void AssignToken(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + TokenIsAssignedMessage);
            HostShip.Tokens.AssignToken(GetTokenType(), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }

}

