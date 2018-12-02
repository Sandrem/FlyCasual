using BoardTools;
using Ship;
using System.Collections;
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
                PilotInfo = new PilotCardInfo(
                    "Torkil Mux",
                    2,
                    36,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TorkilMuxAbility),
                    extraUpgradeIcon: UpgradeType.Illicit,
                    factionOverride: Faction.Scum,
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
