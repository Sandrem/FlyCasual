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
    public class Os1ArsenalLoadout : GenericUpgradeSlotUpgrade, ISecondEditionUpgrade
    {
        public Os1ArsenalLoadout() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Os-1 Arsenal Loadout";
            Cost = 2;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Torpedo),
                new UpgradeSlot(UpgradeType.Missile)
            };
            UpgradeAbilities.Add(new Os1ArsenalLoadoutAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Types.RemoveAll(t => t == UpgradeType.Title);
            Types.Add(UpgradeType.Configuration);

            Cost = 0;

            SEImageNumber = 125;

            UpgradeAbilities.RemoveAll(a => a is Abilities.Os1ArsenalLoadoutAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.Os1ArsenalLoadoutAbilitySE());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is AlphaClassStarWing;
        }
    }
}

namespace Abilities
{
    public class Os1ArsenalLoadoutAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck += AllowLaunchesByTargetLock;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck -= AllowLaunchesByTargetLock;
        }

        private void AllowLaunchesByTargetLock(ref bool result)
        {
            GenericSecondaryWeapon secondaryWeapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
            if (secondaryWeapon != null)
            {
                if ((secondaryWeapon.HasType(UpgradeType.Torpedo) || secondaryWeapon.HasType(UpgradeType.Missile)) && Actions.HasTargetLockOn(Selection.ThisShip, Selection.AnotherShip))
                {
                    result = false;
                }
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Os1ArsenalLoadoutAbilitySE : GenericAbility
    {
        private List<char> targetLockLetters = new List<char>();

        public override void ActivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck += AllowLaunchesByTargetLock;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnWeaponsDisabledCheck -= AllowLaunchesByTargetLock;
        }

        private void AllowLaunchesByTargetLock(ref bool result)
        {
            if (HostShip.Tokens.CountTokensByType(typeof(WeaponsDisabledToken)) != 1) return;

            if (!IsMissilesOrTorpedoesAttack()) return;

            if (!Actions.HasTargetLockOn(HostShip, Selection.AnotherShip)) return;

            Messages.ShowInfo(HostUpgrade.Name + ": Attack is allowed");
            result = false;
            LockTargetLocks();
        }

        private void LockTargetLocks()
        {
            targetLockLetters = Actions.GetTargetLocksLetterPairs(HostShip, Selection.AnotherShip);
            foreach (char targetLockLetter in targetLockLetters)
            {
                HostShip.Tokens.GetToken(typeof(BlueTargetLockToken), targetLockLetter).CanBeUsed = false;
            }

            HostShip.OnAttackFinish += SetTargetLockCanBeUsed;
        }

        private void SetTargetLockCanBeUsed(GenericShip ship)
        {
            foreach (char targetLockLetter in targetLockLetters)
            {
                BlueTargetLockToken ownTargetLockToken = (BlueTargetLockToken)HostShip.Tokens.GetToken(typeof(BlueTargetLockToken), targetLockLetter);
                if (ownTargetLockToken != null) ownTargetLockToken.CanBeUsed = true;
            }

            HostShip.OnAttackFinish -= SetTargetLockCanBeUsed;
        }

        private bool IsMissilesOrTorpedoesAttack()
        {
            bool result = false;

            GenericSecondaryWeapon secondaryWeapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
            if (secondaryWeapon != null)
            {
                if ((secondaryWeapon.HasType(UpgradeType.Torpedo) || secondaryWeapon.HasType(UpgradeType.Missile)) && Actions.HasTargetLockOn(Selection.ThisShip, Selection.AnotherShip))
                {
                    result = true;
                }
            }

            return result;
        }
    }
}
