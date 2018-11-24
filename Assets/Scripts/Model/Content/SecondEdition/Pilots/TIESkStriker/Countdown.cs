using Ship;
using SubPhases;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class Countdown : TIESkStriker
        {
            public Countdown() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Countdown\"",
                    4,
                    44,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.CountdownAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 118
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CountdownAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal += CheckCountdownAbilitySE;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal -= CheckCountdownAbilitySE;
        }

        private void CheckCountdownAbilitySE(GenericShip toDamage, DamageSourceEventArgs e)
        {
            if (Combat.Defender != HostShip)
                return;

            if (HostShip.Tokens.HasToken<StressToken>())
                return;

            RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, UseCountdownAbilitySE);
        }

        private void UseCountdownAbilitySE(object sender, System.EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, delegate { HostShip.Tokens.AssignToken(typeof(StressToken), BlankDamage); });
        }

        private void BlankDamage()
        {
            Messages.ShowInfo("Damage canceled by Countdown!");

            DamageSourceEventArgs countdownDamage = new DamageSourceEventArgs
            {
                Source = "Countdown's ability",
                DamageType = DamageTypes.CardAbility
            };

            HostShip.AssignedDamageDiceroll.RemoveAll();
            HostShip.Damage.TryResolveDamage(1, countdownDamage, DecisionSubPhase.ConfirmDecision);
        }
    }
}
