using Abilities.SecondEdition;
using System;

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
                    limited: 1,
                    abilityText: "When you receive a stress token, you may remove it and roll 1 attack die. On a hit result, deal 1 facedown Damage card to this ship.",
                    abilityType: typeof(JekPorkinsAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 5;
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