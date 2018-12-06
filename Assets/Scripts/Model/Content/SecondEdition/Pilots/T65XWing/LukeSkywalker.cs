using Abilities.SecondEdition;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class LukeSkywalker : T65XWing
        {
            public LukeSkywalker() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Luke Skywalker",
                    5,
                    62,
                    isLimited: true,
                    abilityType: typeof(LukeSkywalkerAbility),
                    force: 2,
                    extraUpgradeIcon: UpgradeType.Force,
                    seImageNumber: 2
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LukeSkywalkerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsDefender += RecoverForce;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsDefender -= RecoverForce;
        }

        private void RecoverForce()
        {
            if (HostShip.State.Force < HostShip.State.MaxForce)
            {
                HostShip.State.Force++;
                Messages.ShowInfo("Luke Skywalker recovered 1 Force");
            }
        }
    }
}