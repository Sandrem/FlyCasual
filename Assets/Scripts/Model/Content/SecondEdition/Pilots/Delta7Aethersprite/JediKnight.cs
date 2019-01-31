using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class JediKnight : Delta7Aethersprite
    {
        public JediKnight()
        {
            PilotInfo = new PilotCardInfo(
                "Jedi Knight",
                3,
                50,
                force: 1,
                extraUpgradeIcon: UpgradeType.Force
            );

            ImageUrl = "https://files.rebel.pl/images/wydawnictwo/zapowiedzi/Star_Wars/SWZ32_jedi-knight_pl.png";
        }
    }
}