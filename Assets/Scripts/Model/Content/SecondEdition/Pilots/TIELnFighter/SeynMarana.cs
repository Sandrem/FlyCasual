using Abilities.SecondEdition;
using ActionsList.SecondEdition;
using Ship;
using System;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class SeynMarana : TIELnFighter
        {
            public SeynMarana() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Seyn Marana",
                    4,
                    30,
                    isLimited: true,
                    abilityType: typeof(SeynMaranaAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 86
                );

                ModelInfo.SkinName = "Inferno";
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
            ship.AddAvailableDiceModification(new SeynMaranaDiceModificationSE() { Host = HostShip });
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
            Combat.CurrentDiceRoll.OrganizeDicePositions();

            DamageSourceEventArgs SeynMaranaDamage = new DamageSourceEventArgs()
            {
                Source = "Seyn Marana's Ability",
                DamageType = DamageTypes.CardAbility
            };

            Combat.Defender.Damage.SufferFacedownDamageCard(SeynMaranaDamage, callBack);
        }
    }
}