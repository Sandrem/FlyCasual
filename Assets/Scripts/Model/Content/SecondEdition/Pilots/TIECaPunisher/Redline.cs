using ActionsList;
using System;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIECaPunisher
    {
        public class Redline : TIECaPunisher
        {
            public Redline() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Redline\"",
                    5,
                    52,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.RedlineAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 139
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //You can maintain 2 locks. After you perform an action, you may acquire a lock.
    public class RedlineAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.TwoTargetLocksOnSameTargetsAreAllowed.Add(HostShip);
            HostShip.OnActionIsPerformed += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.TwoTargetLocksOnSameTargetsAreAllowed.Remove(HostShip);
            HostShip.OnActionIsPerformed -= RegisterAbility;
        }

        private void RegisterAbility(GenericAction action)
        {
            RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AcquireSecondTargetLock);
        }

        private void AcquireSecondTargetLock(object sender, EventArgs e)
        {
            HostShip.ChooseTargetToAcquireTargetLock(
                Triggers.FinishTrigger,
                "You may acquire a lock",
                HostShip
            );
        }
    }
}
