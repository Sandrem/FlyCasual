using Ship;
using SubPhases;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.QuadrijetTransferSpacetug
    {
        public class ConstableZuvio : QuadrijetTransferSpacetug
        {
            public ConstableZuvio() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Constable Zuvio",
                    4,
                    33,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ConstableZuvioAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 161
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ConstableZuvioAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.CanLaunchBombsWithTemplate = 1;
        }

        public override void DeactivateAbility()
        {
            HostShip.CanLaunchBombsWithTemplate = 0;
        }
    }
}
