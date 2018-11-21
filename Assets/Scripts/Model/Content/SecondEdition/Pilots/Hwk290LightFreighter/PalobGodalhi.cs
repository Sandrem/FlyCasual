using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class PalobGodalhi : Hwk290LightFreighter
        {
            public PalobGodalhi() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Palob Godalhi",
                    3,
                    38,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.PalobGodalhi)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ShipInfo.Faction = Faction.Scum;

                SEImageNumber = 175;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PalobGodalhi : Abilities.FirstEdition.PalobGodalhi
    {
        protected override bool FilterAbilityTarget(GenericShip ship)
        {
            return
                FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) &&
                FilterTargetsByRange(ship, 0, 2) &&
                Board.IsShipInArc(HostShip, ship) &&
                FilterTargetWithTokens(ship);
        }
    }
}