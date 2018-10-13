using System.Collections;
using System.Collections.Generic;
using Upgrade;
using RuleSets;
using Ship;
using Movement;
using ActionsList;

namespace UpgradesList
{

    public class R3Astromech : GenericUpgrade
    {
        public R3Astromech() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "R3 Astromech";
            Cost = 3;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new Abilities.SecondEdition.R3AstromechAbilitySE());

            SEImageNumber = 54;
        }
    }

}

namespace Abilities
{
    namespace SecondEdition
    {
        public class R3AstromechAbilitySE : GenericAbility
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
}
