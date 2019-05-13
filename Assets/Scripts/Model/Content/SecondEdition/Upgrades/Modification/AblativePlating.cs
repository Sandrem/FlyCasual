using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class AblativePlating : GenericUpgrade
    {
        public AblativePlating() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ablative Plating",
                UpgradeType.Modification,
                cost: 6,
                restriction: new BaseSizeRestriction(BaseSize.Medium, BaseSize.Large),
                abilityType: typeof(Abilities.SecondEdition.AblativePlatingAbility),
                charges: 2,
                seImageNumber: 68
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AblativePlatingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTryDamagePrevention += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTryDamagePrevention -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, DamageSourceEventArgs e)
        {
            if (HostUpgrade.State.Charges > 0 && DamageTypeIsSupported(e))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                AlwaysUseByDefault,
                UseOwnAbility,
                infoText: "Ablative Plating: You may spend 1 charge to prevent 1 damage"
            );
        }

        private void UseOwnAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo("Ablative Plating was used");

            HostUpgrade.State.SpendCharge();
            if (HostShip.AssignedDamageDiceroll.CriticalSuccesses > 0)
            {
                HostShip.AssignedDamageDiceroll.RemoveType(DieSide.Crit);
            }
            else
            {
                HostShip.AssignedDamageDiceroll.RemoveType(DieSide.Success);
            }

            Triggers.FinishTrigger();
        }

        private bool DamageTypeIsSupported(DamageSourceEventArgs e)
        {
            return (
                e.DamageType == DamageTypes.ObstacleCollision
                || (
                    e.DamageType == DamageTypes.BombDetonation
                    && (e.Source as GenericUpgrade).UpgradeInfo.SubType == UpgradeSubType.Bomb
                    && (e.Source as GenericUpgrade).HostShip.Owner.PlayerNo == HostShip.Owner.PlayerNo
                )
            );
        }
    }
}