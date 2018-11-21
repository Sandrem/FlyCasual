using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class RoarkGarnet : Hwk290LightFreighter
        {
            public RoarkGarnet() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Roark Garnet",
                    4,
                    38,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.RoarkGarnetAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 44;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RoarkGarnetAbility : Abilities.FirstEdition.RoarkGarnetAbility
    {
        protected override string GenerateAbilityMessage()
        {
            return "Choose another friendly ship.\nUntil the end of the phase, treat that ship's pilot skill value as \"7\".";
        }

        public override void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 7;
        }

        protected override bool FilterAbilityTarget(GenericShip ship)
        {
            return
                FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) &&
                FilterTargetsByRange(ship, 1, 3) &&
                Board.IsShipInArc(HostShip, ship);
        }
    }
}
