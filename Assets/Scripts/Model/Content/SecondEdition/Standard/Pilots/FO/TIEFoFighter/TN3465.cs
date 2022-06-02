using BoardTools;
using System;
using System.Collections.Generic;
using Upgrade;
using Content;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class TN3465 : TIEFoFighter
        {
            public TN3465() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "TN-3465",
                    "Loose End",
                    Faction.FirstOrder,
                    2,
                    3,
                    5,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TN3465Ability),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Tech,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/333cbf0da8849edb38c4e93944d8fe57.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TN3465Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Change,
                1,
                sideCanBeChangedTo: DieSide.Crit,
                isGlobal: true,
                payAbilityCost: SufferCriticalDamage
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsDiceModificationAvailable()
        {
            if (Combat.AttackStep != CombatStep.Attack) return false;

            if (Combat.Attacker.ShipId == HostShip.ShipId) return false;

            if (Combat.Attacker.Owner.PlayerNo != HostShip.Owner.PlayerNo) return false;

            DistanceInfo distInfo = new DistanceInfo(HostShip, Combat.Defender);
            if (distInfo.Range > 1) return false;

            return true;
        }

        private int GetDiceModificationAiPriority()
        {
            return (HostShip.State.ShieldsCurrent > 0) ? 100 : 0;
        }

        private void SufferCriticalDamage(Action<bool> callback)
        {
            DamageSourceEventArgs damageArgs = new DamageSourceEventArgs()
            {
                DamageType = DamageTypes.CardAbility,
                Source = HostShip
            };

            HostShip.Damage.TryResolveDamage(0, 1, damageArgs, delegate { callback(true); });
        }
    }
}
