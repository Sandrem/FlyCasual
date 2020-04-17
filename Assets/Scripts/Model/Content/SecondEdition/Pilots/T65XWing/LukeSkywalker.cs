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
                    extraUpgradeIcon: UpgradeType.ForcePower,
                    seImageNumber: 2
                );

                ModelInfo.SkinName = "Luke Skywalker";
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
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " recovered 1 Force");
            }
        }
    }
}