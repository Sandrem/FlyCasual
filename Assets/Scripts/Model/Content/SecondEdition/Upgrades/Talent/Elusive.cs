using Upgrade;
using System.Collections.Generic;
using System.Linq;
using System;
using Ship;
using ActionsList;

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
                charges: 1,
                seImageNumber: 4
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class Elusive : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddDiceModification;
            HostShip.OnMovementFinish += CheckRestoreCharge;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddDiceModification;
            HostShip.OnMovementFinish -= CheckRestoreCharge;
        }

        private void AddDiceModification(GenericShip host)
        {
            GenericAction newAction = new ElusiveDiceModification
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host,
                Source = HostUpgrade
            };
            host.AddAvailableDiceModification(newAction);
        }

        private void CheckRestoreCharge(GenericShip host)
        {
            if (HostShip.IsBumped) return;

            if (HostShip.AssignedManeuver.ColorComplexity != Movement.MovementComplexity.Complex) return;

            if (HostUpgrade.State.Charges < HostUpgrade.State.MaxCharges)
            {
                HostUpgrade.State.RestoreCharge();
                Messages.ShowInfo("Elusive: Charge is restored");
            }
        }
    }
}

namespace ActionsList
{
    public class ElusiveDiceModification : GenericAction
    {
        public ElusiveDiceModification()
        {
            Name = DiceModificationName = "Elusive";

            IsReroll = true;
        }

        public override void ActionEffect(Action callBack)
        {

            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 1,
                CallBack = callBack
            };

            Source.State.SpendCharge();
            diceRerollManager.Start();
        }

        public override bool IsDiceModificationAvailable()
        {
            if (Combat.AttackStep != CombatStep.Defence) return false;
            if (Source.State.Charges == 0) return false;

            return true;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes) result = 85;

            return result;
        }
    }
}