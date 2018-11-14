using System;
using Tokens;

namespace Ship
{
    namespace SecondEdition.TIEPhPhantom
    {
        public class Whisper : TIEPhPhantom
        {
            public Whisper() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Whisper\"",
                    5,
                    52,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.WhisperAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 131;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class WhisperAbility : GenericAbility
    {
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
                AskToUseAbility(AlwaysUseByDefault, AssignToken, null, null, true);
            }
            else
            {
                HostShip.Tokens.AssignToken(GetTokenType(), Triggers.FinishTrigger);
            }
        }

        private void AssignToken(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(GetTokenType(), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }

}

namespace Abilities.SecondEdition
{
    public class WhisperAbility : Abilities.FirstEdition.WhisperAbility
    {
        protected override Type GetTokenType()
        {
            return typeof(EvadeToken);
        }
    }
}
