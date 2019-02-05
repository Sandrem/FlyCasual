using ActionsList.SecondEdition;
using Ship;
using System;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.M3AInterceptor
    {
        public class Inaldra : M3AInterceptor
        {
            public Inaldra() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Inaldra",
                    2,
                    31,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.SerissuAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 187
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class InaldraAbilitySE : Abilities.FirstEdition.InaldraAbility
    {
        protected override void AddInaldraAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new InaldraActionSE() { HostShip = HostShip });
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class InaldraActionSE : GenericAction
    {
        public InaldraActionSE()
        {
            Name = DiceModificationName = "Inaldra's ability";
        }

        public override void ActionEffect(Action callBack)
        {
            Messages.ShowInfoToHuman(Name + ": suffer one hit to reroll any number of dice");

            // reroll dice
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                SidesCanBeRerolled = null,      // all the sides can be reroll
                CallBack = callBack// all the dices can be reroll
            };

            DamageSourceEventArgs inaldraRerollDmg = new DamageSourceEventArgs()
            {
                Source = "Inaldra Reroll Damage",
                DamageType = DamageTypes.CardAbility
            };

            HostShip.Damage.TryResolveDamage(1, inaldraRerollDmg, diceRerollManager.Start);

        }

        public override bool IsDiceModificationAvailable()
        {
            return true;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                // While defending, use the ability only if the attack success is greater than
                // the defence AND it's possible to cancel more dice with the defence
                if ((Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes)
                    && (Combat.DiceRollAttack.Count <= Combat.DiceRollDefence.Count))
                {
                    result = 90;
                }
            }

            if (Combat.AttackStep == CombatStep.Attack)
            {
                // While attacking, use the ability only if the attack success is lower than
                // the defence AND it's possible to damage more the the defence
                if ((Combat.DiceRollAttack.Successes < Combat.DiceRollDefence.Successes)
                    && (Combat.DiceRollAttack.Count >= Combat.DiceRollDefence.Count))
                {
                    result = 90;
                }
            }
            return result;
        }
    }
}