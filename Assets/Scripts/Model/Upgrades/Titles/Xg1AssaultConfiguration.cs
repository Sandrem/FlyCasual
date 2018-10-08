using Ship;
using Ship.AlphaClassStarWing;
using Upgrade;
using System.Collections.Generic;
using System;
using Abilities;
using RuleSets;
using Tokens;

namespace UpgradesList
{
    public class Xg1AssaultConfiguration : GenericUpgradeSlotUpgrade, ISecondEditionUpgrade
    {
        public Xg1AssaultConfiguration() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Xg-1 Assault Configuration";
            Cost = 1;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Cannon),
                new UpgradeSlot(UpgradeType.Cannon)
            };
            UpgradeAbilities.Add(new Xg1AssaultConfigurationAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Types.RemoveAll(t => t == UpgradeType.Title);
            Types.Add(UpgradeType.Configuration);

            Cost = 0;

            AddedSlots.Remove(AddedSlots.Find(u => u.Type == UpgradeType.Cannon));

            SEImageNumber = 126;

            UpgradeAbilities.RemoveAll(a => a is Abilities.Xg1AssaultConfigurationAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.Xg1AssaultConfigurationAbilitySE());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is AlphaClassStarWing;
        }
    }
}

namespace Abilities
{
    public class Xg1AssaultConfigurationAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck += AllowLowCostCannons;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck -= AllowLowCostCannons;
        }

        private void AllowLowCostCannons(ref bool result)
        {
            GenericSecondaryWeapon secondaryWeapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
            if (secondaryWeapon != null)
            {
                if (secondaryWeapon.HasType(UpgradeType.Cannon) && secondaryWeapon.Cost <= 2)
                {
                    result = false;
                }
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Xg1AssaultConfigurationAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck += AllowLowCostCannons;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck -= AllowLowCostCannons;
        }

        private void AllowLowCostCannons(ref bool result)
        {
            if (HostShip.Tokens.CountTokensByType(typeof(WeaponsDisabledToken)) != 1) return;

            if (!IsCannonAttack()) return;

            Messages.ShowInfo(HostUpgrade.Name + ": Attack is allowed");
            result = false;
            PrepareAttackDiceCap();
        }

        private bool IsCannonAttack()
        {
            bool result = false;

            GenericSecondaryWeapon secondaryWeapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
            if (secondaryWeapon != null)
            {
                if (secondaryWeapon.HasType(UpgradeType.Cannon))
                {
                    result = true;
                }
            }

            return result;
        }

        private void PrepareAttackDiceCap()
        {
            HostShip.AfterGotNumberOfAttackDiceCap += SetAttackDiceCap;

            HostShip.OnAttackFinish += RemoveAttackDiceCap;
        }

        private void SetAttackDiceCap(ref int count)
        {
            Messages.ShowInfo(HostUpgrade.Name + ": Only 3 dice can be rolled");
            if (count > 3) count = 3;
        }

        private void RemoveAttackDiceCap(GenericShip ship)
        {
            HostShip.AfterGotNumberOfAttackDiceCap -= SetAttackDiceCap;

            HostShip.OnAttackFinish -= RemoveAttackDiceCap;
        }
    }
}
