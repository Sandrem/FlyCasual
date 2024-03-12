using Actions;
using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Ghost : GenericUpgrade
    {
        public Ghost() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ghost",
                UpgradeType.Title,
                cost: 0,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Rebel),
                    new ShipRestriction(typeof(Ship.SecondEdition.VCX100LightFreighter.VCX100LightFreighter))
                ),
                abilityType: typeof(Abilities.SecondEdition.GhostAbility),
                seImageNumber: 102
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class GhostAbility : GenericAbility
    {
        private List<GenericShip> DockableShips;

        public override void ActivateAbility()
        {
            ActivateDocking(FilterShuttles, FilterOnlyRearDirection);
        }

        public override void DeactivateAbility()
        {
            DeactivateDocking();
        }

        private bool FilterOnlyRearDirection(Direction direction)
        {
            return direction == Direction.Bottom;
        }

        private bool FilterShuttles(GenericShip ship)
        {
            return ship is Ship.SecondEdition.AttackShuttle.AttackShuttle || ship is Ship.SecondEdition.SheathipedeClassShuttle.SheathipedeClassShuttle;
        }
    }
}