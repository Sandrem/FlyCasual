using System.Collections.Generic;
using Arcs;
using Upgrade;
using System;

namespace UpgradesList.SecondEdition
{
    public class ProtonCannons : GenericSpecialWeapon
    {
        public ProtonCannons() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Proton Cannons",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Cannon,
                    UpgradeType.Cannon
                },
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 4,
                    minRange: 2,
                    maxRange: 3,
                    charges: 2,
                    regensCharges: true,
                    arc: ArcType.Bullseye
                ),
                abilityType: typeof(Abilities.SecondEdition.ProtonCannonsAbility)

            );


            ImageUrl = "https://infinitearenas.com/xw2/images/upgrades/protoncannons.png";
        }
        public override void PayAttackCost(Action callBack) { callBack(); }
    }
}

namespace Abilities.SecondEdition
{
    public class ProtonCannonsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostUpgrade.UpgradeInfo.Name,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Change,
                1,
                new List<DieSide>() { DieSide.Focus, DieSide.Success  },
                DieSide.Crit,
                payAbilityCost: payCharges
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private void payCharges(Action<bool> callback)
        {
            if (HostUpgrade.State.Charges > 1)
            {
                HostUpgrade.State.SpendCharge();
                HostUpgrade.State.SpendCharge();
                callback(true);
            }
            else
            {
                callback(false);
            }
        }

        private bool IsDiceModificationAvailable()
        {
            bool result = true;

            if (Combat.AttackStep != CombatStep.Attack) result = false;

            if (Combat.ChosenWeapon != HostUpgrade) result = false;

            if (HostUpgrade.State.Charges < 2) result = false;

            return result;
        }

        private int GetDiceModificationAiPriority()
        {
            int result = 0;
            if (Combat.DiceRollAttack.RegularSuccesses + Combat.DiceRollAttack.Focuses > 0) result = 100;
            return result;
        }
    }
}
