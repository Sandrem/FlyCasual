using Abilities.SecondEdition;
using RuleSets;
using Ship;
using SubPhases;
using Tokens;

namespace Ship
{
    namespace TIEStriker
    {
        public class Countdown : TIEStriker, ISecondEditionPilot
        {
            public Countdown() : base()
            {
                PilotName = "\"Countdown\"";
                PilotSkill = 4;
                Cost = 44;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new CountdownAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
                // not needed
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CountdownAbilitySE : GenericAbility
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