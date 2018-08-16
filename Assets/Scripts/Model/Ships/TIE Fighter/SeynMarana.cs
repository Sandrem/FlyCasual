using ActionsList;
using ActionsList.SecondEdition;
using RuleSets;
using Ship;
using System;

namespace Ship
{
    namespace TIEFighter
    {
        public class SeynMarana : TIEFighter, ISecondEditionPilot
        {
            public SeynMarana() : base()
            {
                PilotName = "Seyn Marana";
                PilotSkill = 4;
                Cost = 30;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new Abilities.SecondEdition.SeynMaranaAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SeynMaranaAbilitySE : GenericAbility
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
                Source = Host,
                SourceDescription = "Seyn Marana's Ability",
                DamageType = DamageTypes.CardAbility
            };

            Combat.Defender.Damage.SufferFacedownDamageCard(SeynMaranaDamage, callBack);
        }
    }
}