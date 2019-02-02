using ActionsList;
using Ship;
using System;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.M3AInterceptor
    {
        public class Inaldra : M3AInterceptor
        {
            public Inaldra() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Inaldra",
                    3,
                    15,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.InaldraAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
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
            ship.AddAvailableDiceModification(new ActionsList.FirstEdition.InaldraAction() { HostShip = HostShip });
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
                Messages.ShowInfoToHuman(Name + ": losing one shield to reroll any number of dice");
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
                Messages.ShowError(Name + ": no shield available, unable to use the ability.");
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