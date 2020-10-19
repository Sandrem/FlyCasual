using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class JediGeneral : Eta2Actis
    {
        public JediGeneral()
        {
            PilotInfo = new PilotCardInfo(
                "Jedi General",
                4,
                40,
                force: 2,
                extraUpgradeIcon: UpgradeType.ForcePower
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b4/8d/b48d787d-96db-4955-9a58-5c8aa3ab9035/swz79_jedi-general.png";
        }
    }
}