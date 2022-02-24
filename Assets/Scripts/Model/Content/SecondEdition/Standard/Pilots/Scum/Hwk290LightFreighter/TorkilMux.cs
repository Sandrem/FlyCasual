using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class TorkilMux : Hwk290LightFreighter
        {
            public TorkilMux() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Torkil Mux",
                    "Mercenary Miner",
                    Faction.Scum,
                    2,
                    4,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TorkilMuxAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Crew,
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter
                    },
                    seImageNumber: 176
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TorkilMuxAbility : RoarkGarnetAbility
    {
        protected override string GenerateAbilityMessage()
        {
            return "Choose another ship in arc.\nUntil the end of the phase, treat that ship's pilot skill value as \"0\".";
        }

        public override void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 0;
        }

        protected override bool FilterAbilityTarget(GenericShip ship)
        {
            return Board.IsShipInArc(HostShip, ship);
        }
    }
}
