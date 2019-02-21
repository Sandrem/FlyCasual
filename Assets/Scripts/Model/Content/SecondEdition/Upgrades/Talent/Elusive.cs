using Upgrade;
using System;
using Ship;

namespace UpgradesList.SecondEdition
{
    public class Elusive : GenericUpgrade
    {
        public Elusive() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Elusive",
                UpgradeType.Talent,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.Elusive),
                restriction: new BaseSizeRestriction(BaseSize.Small, BaseSize.Medium),
                charges: 1,
                seImageNumber: 4
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    //While you defend, you may spend 1 charge to reroll 1 defense die.
    //After you fully execute a red maneuver, recover 1 charge.
    public class Elusive : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                1,
                payAbilityCost: PayAbilityCost
            );
            HostShip.OnMovementFinishSuccessfully += CheckRestoreCharge;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
            HostShip.OnMovementFinishSuccessfully -= CheckRestoreCharge;
        }

        private bool IsDiceModificationAvailable()
        {
            if (Combat.AttackStep != CombatStep.Defence) return false;
            if (HostUpgrade.State.Charges == 0) return false;

            return true;
        }

        private int GetDiceModificationAiPriority()
        {
            int result = 0;

            if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes) result = 85;

            return result;
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                HostUpgrade.State.SpendCharge();
                callback(true);
            }
            else
            {
                callback(false);
            }
        }

        private void CheckRestoreCharge(GenericShip host)
        {
            if (HostShip.AssignedManeuver.ColorComplexity != Movement.MovementComplexity.Complex) return;

            if (HostUpgrade.State.Charges < HostUpgrade.State.MaxCharges)
            {
                HostUpgrade.State.RestoreCharge();
                Messages.ShowInfo("Elusive: Charge is restored");
            }
        }
    }
}
