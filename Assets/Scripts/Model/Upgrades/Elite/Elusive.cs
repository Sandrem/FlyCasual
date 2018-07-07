using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using SubPhases;
using System;
using Abilities;
using RuleSets;
using ActionsList;

namespace UpgradesList
{
    public class Elusive : GenericUpgrade
    {
        public Elusive() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Elusive";
            Cost = 1;

            MaxCharges = 1;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new Abilities.SecondEdition.Elusive());
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        public class Elusive : GenericAbility
        {
            public override void ActivateAbility()
            {
                HostShip.AfterGenerateAvailableActionEffectsList += AddDiceModification;
                HostShip.OnMovementFinish += CheckRestoreCharge;
            }

            public override void DeactivateAbility()
            {
                HostShip.AfterGenerateAvailableActionEffectsList -= AddDiceModification;
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
                host.AddAvailableActionEffect(newAction);
            }

            private void CheckRestoreCharge(GenericShip host)
            {
                if (HostShip.IsBumped) return;

                if (HostShip.AssignedManeuver.ColorComplexity != Movement.MovementComplexity.Complex) return;

                if (HostUpgrade.Charges < HostUpgrade.MaxCharges)
                {
                    HostUpgrade.RestoreCharge();
                    Messages.ShowInfo("Elusive: Charge is restored");
                }
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
            Name = EffectName = "Elusive";

            IsReroll = true;
        }

        public override void ActionEffect(Action callBack)
        {

            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 1,
                CallBack = callBack
            };

            Source.SpendCharge(diceRerollManager.Start);
        }

        public override bool IsActionEffectAvailable()
        {
            if (Combat.AttackStep != CombatStep.Defence) return false;
            if (Source.Charges == 0) return false;

            return true;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes) result = 85;

            return result;
        }
    }
}