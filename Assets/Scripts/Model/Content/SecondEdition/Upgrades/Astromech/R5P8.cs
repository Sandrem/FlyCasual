using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class R5P8 : GenericUpgrade
    {
        public R5P8() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R5-P8",
                UpgradeType.Astromech,
                cost: 4,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.R5P8Ability),
                restriction: new FactionRestriction(Faction.Scum),
                charges: 3,
                seImageNumber: 144
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform an attack against a defender in your forward firing arc, you may spend 1 charge to reroll 1 attack die. 
    //If the rerolled result is a critical hit result, suffer 1 critical hit damage.
    public class R5P8Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                1,
                payAbilityCost: PayAbilityCost);
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack && Combat.Attacker == HostShip && Combat.ShotInfo.InArcByType(Arcs.ArcType.Front) && HostUpgrade.State.Charges > 0);
        }

        private int GetDiceModificationAiPriority()
        {
            return 75;
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                HostUpgrade.State.SpendCharge(); 
                HostShip.OnImmediatelyAfterReRolling += CheckForCrit;
                callback(true);
            }
            else callback(false);
        }

        private void CheckForCrit(DiceRoll diceroll)
        {
            HostShip.OnImmediatelyAfterReRolling -= CheckForCrit;

            if (diceroll.HasResult(DieSide.Crit))
            {
                RegisterAbilityTrigger(TriggerTypes.OnImmediatelyAfterReRolling, DealDamage);
            }
        }

        private void DealDamage(object sender, EventArgs e)
        {
            DealDamageToShip(HostShip, 1, true, Triggers.FinishTrigger);
        }
    }
}