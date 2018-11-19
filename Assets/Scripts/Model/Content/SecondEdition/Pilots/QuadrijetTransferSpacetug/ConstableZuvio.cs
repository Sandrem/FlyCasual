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
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.ConstableZuvioAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 161;
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
