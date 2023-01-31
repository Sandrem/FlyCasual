using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.LancerClassPursuitCraft
    {
        public class ShadowportHunter : LancerClassPursuitCraft
        {
            public ShadowportHunter() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Shadowport Hunter",
                    "",
                    Faction.Scum,
                    2,
                    6,
                    6,
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Illicit,
                        UpgradeType.Illicit
                    },
                    seImageNumber: 221,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}