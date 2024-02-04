using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class JediGeneral : Eta2Actis
    {
        public JediGeneral()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Jedi General",
                "",
                Faction.Republic,
                4,
                5,
                4,
                force: 2,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.ForcePower,
                    UpgradeType.Cannon,
                    UpgradeType.Astromech,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Blue"
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b4/8d/b48d787d-96db-4955-9a58-5c8aa3ab9035/swz79_jedi-general.png";
        }
    }
}