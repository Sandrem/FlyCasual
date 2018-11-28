using Ship;
using Upgrade;
using UnityEngine;
using System.Collections.Generic;

namespace UpgradesList.FirstEdition
{
    public class WeaponsEngineer : GenericUpgrade
    {
        public WeaponsEngineer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Weapons Engineer",
                UpgradeType.Crew,
                cost: 3,
                abilityType: typeof(Abilities.FirstEdition.WeaponsEngineerAbility)
            );

            Avatar = new AvatarInfo(Faction.None, new Vector2(60, 1));
        }        
    }
}

namespace Abilities.FirstEdition
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
                "You may acquire a lock",
                HostUpgrade
            );
        }
    }
}