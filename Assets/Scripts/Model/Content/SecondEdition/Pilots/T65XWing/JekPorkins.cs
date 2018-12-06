using Abilities.SecondEdition;
using System;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class JekPorkins : T65XWing
        {
            public JekPorkins() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Jek Porkins",
                    4,
                    46,
                    isLimited: true,
                    abilityType: typeof(JekPorkinsAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 5
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JekPorkinsAbility : Abilities.FirstEdition.JekPorkinsAbility
    {
        protected override void SufferNegativeEffect(Action callback)
        {
            DamageSourceEventArgs damageArgs = new DamageSourceEventArgs()
            {
                Source = HostShip,
                DamageType = DamageTypes.CardAbility
            };

            HostShip.Damage.TryResolveDamage(1, damageArgs, callback);
        }
    }
}