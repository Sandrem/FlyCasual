using Upgrade;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class R3Astromech : GenericUpgrade
    {
        public R3Astromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R3 Astromech",
                UpgradeType.Astromech,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.R3AstromechAbility),
                restrictionFaction: Faction.Rebel,
                seImageNumber: 54
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class R3AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.TwoTargetLocksOnDifferentTargetsAreAllowed.Add(HostUpgrade);
            HostShip.OnActionIsPerformed += CheckAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.TwoTargetLocksOnDifferentTargetsAreAllowed.Remove(HostUpgrade);
            HostShip.OnActionIsPerformed -= CheckAction;
        }

        private void CheckAction(GenericAction action)
        {
            if (action is TargetLockAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AcquireSecondTargetLock);
            }
        }

        private void AcquireSecondTargetLock(object sender, System.EventArgs e)
        {
            HostShip.ChooseTargetToAcquireTargetLock(
                Triggers.FinishTrigger,
                "You may acquire a lock",
                HostUpgrade
            );
        }
    }
}