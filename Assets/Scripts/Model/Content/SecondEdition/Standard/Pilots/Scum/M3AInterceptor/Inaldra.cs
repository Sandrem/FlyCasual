using ActionsList.SecondEdition;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Inaldra",
                    "Tansarii Point Boss",
                    Faction.Scum,
                    2,
                    3,
                    6,
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

namespace Abilities.FirstEdition
{
    // When attacking or defending, you may spend 1 shield to reroll any number of your dice
    public class InaldraAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddInaldraAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddInaldraAbility;
        }

        protected virtual void AddInaldraAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModificationOwn(new ActionsList.FirstEdition.InaldraAction());
        }
    }
}

namespace ActionsList.FirstEdition
{
    public class InaldraAction : GenericAction
    {
        public InaldraAction()
        {
            Name = DiceModificationName = "Inaldra's ability";
        }

        public override void ActionEffect(Action callBack)
        {
            // remove one shield
            // It should not be possible to get there without having at least one shield (IsActionEffectAvailable),
            //   but just in case ...
            if (HostShip.State.ShieldsCurrent > 0)
            {
                Messages.ShowInfoToHuman("Inaldra is losing one shield to reroll any number of dice");
                HostShip.LoseShield();
                // reroll dice
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    SidesCanBeRerolled = null,      // all the sides can be reroll
                    CallBack = callBack// all the dices can be reroll
                };
                diceRerollManager.Start();
            }
            else
            {
                // should never happens, thanks to IsActionEffectAvailable...
                Messages.ShowError("Inaldra has no shields available and is unable to use her ability");
                callBack();
            }
        }

        public override bool IsDiceModificationAvailable()
        {
            // check if ship has shield to activate ability
            return HostShip.State.ShieldsCurrent > 0;
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