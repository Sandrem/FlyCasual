using System;
using System.Collections;
using System.Collections.Generic;
using ActionsList;
using ActionsList.SecondEdition;
using RuleSets;
using Ship;
using UnityEngine;
 
namespace Ship
{
    namespace M3AScyk
    {
        public class Inaldra : M3AScyk, ISecondEditionPilot
        {
            public Inaldra() : base()
            {
                PilotName = "Inaldra";
                PilotSkill = 3;
                Cost = 15;

                IsUnique = true;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.InaldraAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 32;

                PilotAbilities.RemoveAll(ability => ability is Abilities.InaldraAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.InaldraAbilitySE());
            }
        }
   }
}
 
 
namespace Abilities
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
            ship.AddAvailableDiceModification(new InaldraAction() {Host = HostShip});
        }
    }
}

namespace Abilities.SecondEdition
{
    public class InaldraAbilitySE : InaldraAbility
    {
        protected override void AddInaldraAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new InaldraActionSE() { Host = HostShip });
        }
    }
}

namespace ActionsList
{
    public class InaldraAction : ActionsList.GenericAction
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
            if (Host.Shields > 0)
            {
                Messages.ShowInfoToHuman(Name + ": losing one shield to reroll any number of dice");
                Host.LoseShield();
                // reroll dice
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    SidesCanBeRerolled = null,      // all the sides can be reroll
                    NumberOfDiceCanBeRerolled = 0,
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
            return Host.Shields > 0;
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
    public class InaldraActionSE : ActionsList.GenericAction
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
                NumberOfDiceCanBeRerolled = 0,
                CallBack = callBack// all the dices can be reroll
            };

            DamageSourceEventArgs inaldraRerollDmg = new DamageSourceEventArgs()
            {
                Source = Host,
                SourceDescription = "Inaldra Reroll Damage",
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