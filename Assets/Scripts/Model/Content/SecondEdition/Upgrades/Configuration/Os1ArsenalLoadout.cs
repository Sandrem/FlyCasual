using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Os1ArsenalLoadout : GenericUpgrade
    {
        public Os1ArsenalLoadout() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Os-1 Arsenal Loadout",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.AlphaClassStarWing.AlphaClassStarWing)),
                addSlots: new List<UpgradeSlot>
                {
                    new UpgradeSlot(UpgradeType.Torpedo),
                    new UpgradeSlot(UpgradeType.Missile)
                },
                abilityType: typeof(Abilities.SecondEdition.Os1ArsenalLoadoutAbility),
                seImageNumber: 125
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class Os1ArsenalLoadoutAbility : GenericAbility
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

            if (!ActionsHolder.HasTargetLockOn(HostShip, Selection.AnotherShip)) return;

            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": The attack is allowed");
            result = false;
            LockTargetLocks();
        }

        private void LockTargetLocks()
        {
            targetLockLetters = ActionsHolder.GetTargetLocksLetterPairs(HostShip, Selection.AnotherShip);
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

            GenericSpecialWeapon secondaryWeapon = Combat.ChosenWeapon as GenericSpecialWeapon;
            if (secondaryWeapon != null)
            {
                if (secondaryWeapon.HasType(UpgradeType.Torpedo) || secondaryWeapon.HasType(UpgradeType.Missile))
                {
                    result = true;
                }
            }

            return result;
        }
    }
}
