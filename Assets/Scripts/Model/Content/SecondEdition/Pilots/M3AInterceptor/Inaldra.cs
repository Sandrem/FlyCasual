﻿using ActionsList.SecondEdition;
using Ship;
using System;

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
                    30,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.InaldraAbilitySE),
                    seImageNumber: 187
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you defend or perform an attack, you may suffer 1 damage to reroll any number of your dice.
    public class InaldraAbilitySE : Abilities.FirstEdition.InaldraAbility
    {
        protected override void AddInaldraAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModificationOwn(new InaldraActionSE() { ImageUrl = HostShip.ImageUrl });
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class InaldraActionSE : GenericAction
    {
        public InaldraActionSE()
        {
            Name = DiceModificationName = "Inaldra";
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
                Source = "Inaldra Reroll",
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