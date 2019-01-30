using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class RycerzJedi : Delta7Aethersprite
    {
        public RycerzJedi()
        {
            PilotInfo = new PilotCardInfo(
                "Rycerz Jedi",
                3,
                50,
                force: 1,
                extraUpgradeIcon: UpgradeType.Force
            );

            ImageUrl = "https://files.rebel.pl/images/wydawnictwo/zapowiedzi/Star_Wars/SWZ32_jedi-knight_pl.png";
        }
    }
}