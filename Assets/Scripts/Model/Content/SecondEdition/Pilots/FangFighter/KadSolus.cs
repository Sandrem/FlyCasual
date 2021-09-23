using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class KadSolus : FangFighter
        {
            public KadSolus() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kad Solus",
                    4,
                    53,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KadSolusAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Modification },
                    seImageNumber: 158
                );

                ModelInfo.SkinName = "Skull Squadron";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a red maneuver, gain 2 focus tokens.
    public class KadSolusAbility : Abilities.FirstEdition.KadSolusAbility
    {
        protected override bool CheckAbility()
        {
            if (HostShip.IsBumped) return false;

            return base.CheckAbility();
        }
    }
}