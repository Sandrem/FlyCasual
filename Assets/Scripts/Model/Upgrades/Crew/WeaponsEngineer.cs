using ActionsList;
using Ship;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class WeaponsEngineer : GenericUpgrade
    {
        public WeaponsEngineer() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Weapons Engineer";
            Cost = 3;

            AvatarOffset = new Vector2(60, 1);

            UpgradeAbilities.Add(new Abilities.WeaponsEngineerAbility());
        }
    }
}

namespace Abilities
{
    public class WeaponsEngineerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.TwoTargetLocksOnDifferentTargetsAreAllowed.Add(HostUpgrade);
            HostShip.OnTargetLockIsAcquired += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.TwoTargetLocksOnDifferentTargetsAreAllowed.Remove(HostUpgrade);
            HostShip.OnTargetLockIsAcquired -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (!IsAbilityUsed)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AcquireSecondTargetLock);
            }
        }

        private void AcquireSecondTargetLock(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            HostShip.ChooseTargetToAcquireTargetLock(
                delegate
                {
                    IsAbilityUsed = false;
                    Triggers.FinishTrigger();
                },
                "You may acquare a lock",
                HostUpgrade.ImageUrl
            );
        }
    }
}
