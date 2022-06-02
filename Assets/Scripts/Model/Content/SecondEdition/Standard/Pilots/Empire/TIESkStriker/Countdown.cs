using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
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
                PilotInfo = new PilotCardInfo25
                (
                    "\"Countdown\"",
                    "Death Defier",
                    Faction.Imperial,
                    4,
                    4,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CountdownAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
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
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                delegate { HostShip.Tokens.AssignToken(typeof(StressToken), BlankDamage); },
                descriptionLong: "Do you want to suffer 1 damage and gain 1 stress token to cancel all dice results?",
                imageHolder: HostShip
            );
        }

        private void BlankDamage()
        {
            Messages.ShowInfo("Countdown cancels all the attack's damage at the cost of 1 Hit and 1 stress token");

            DamageSourceEventArgs countdownDamage = new DamageSourceEventArgs
            {
                Source = HostShip,
                DamageType = DamageTypes.CardAbility
            };

            HostShip.AssignedDamageDiceroll.RemoveAll();
            HostShip.Damage.TryResolveDamage(1, countdownDamage, DecisionSubPhase.ConfirmDecision);
        }
    }
}
