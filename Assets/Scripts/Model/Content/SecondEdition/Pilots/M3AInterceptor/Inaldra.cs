using ActionsList;
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
                    32,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.SerissuAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 187;
            }
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
            ship.AddAvailableDiceModification(new InaldraAction() { Host = HostShip });
        }
    }
}

namespace Abilities.SecondEdition
{
    public class InaldraAbilitySE : Abilities.FirstEdition.InaldraAbility
    {
        protected override void AddInaldraAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new InaldraActionSE() { Host = HostShip });
        }
    }
}

namespace ActionsList
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
            if (Host.State.ShieldsCurrent > 0)
            {
                Messages.ShowInfoToHuman(Name + ": losing one shield to reroll any number of dice");
                Host.LoseShield();
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
                Messages.ShowError(Name + ": no shield available, unable to use the ability.");
                callBack();
            }
        }

        public override bool IsDiceModificationAvailable()
        {
            // check if ship has shield to activate ability
            return Host.State.ShieldsCurrent > 0;
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

            Host.Damage.TryResolveDamage(1, inaldraRerollDmg, diceRerollManager.Start);

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