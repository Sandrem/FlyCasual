using Abilities.SecondEdition;
using ActionsList.SecondEdition;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class SeynMarana : TIELnFighter
        {
            public SeynMarana() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Seyn Marana",
                    "Inferno Four",
                    Faction.Imperial,
                    4,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(SeynMaranaAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 86,
                    skinName: "Inferno"
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SeynMaranaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddSeynMaranaAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddSeynMaranaAbility;
        }

        protected virtual void AddSeynMaranaAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModificationOwn(new SeynMaranaDiceModificationSE()
            {
                ImageUrl = HostShip.ImageUrl
            }); ;
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class SeynMaranaDiceModificationSE : GenericAction
    {
        public SeynMaranaDiceModificationSE()
        {
            Name = DiceModificationName = "Seyn Marana's ability";
        }

        public override bool IsDiceModificationAvailable()
        {
            if (Combat.AttackStep != CombatStep.Attack) return false;
            if (Combat.DiceRollAttack.CriticalSuccesses == 0) return false;

            return true;
        }

        public override void ActionEffect(Action callBack)
        {
            Combat.CurrentDiceRoll.RemoveAll();

            DamageSourceEventArgs SeynMaranaDamage = new DamageSourceEventArgs()
            {
                Source = HostShip,
                DamageType = DamageTypes.CardAbility
            };

            Combat.Defender.Damage.SufferFacedownDamageCard(SeynMaranaDamage, callBack);
        }
    }
}