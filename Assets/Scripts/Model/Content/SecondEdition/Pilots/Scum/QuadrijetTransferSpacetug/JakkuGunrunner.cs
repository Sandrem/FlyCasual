using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.QuadrijetTransferSpacetug
    {
        public class JakkuGunrunner : QuadrijetTransferSpacetug
        {
            public JakkuGunrunner() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Jakku Gunrunner",
                    "",
                    Faction.Scum,
                    1,
                    4,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                    },
                    seImageNumber: 164,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}
