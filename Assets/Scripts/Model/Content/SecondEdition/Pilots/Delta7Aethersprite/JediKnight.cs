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

            RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b0/b3/b0b3f463-a3ea-4fe6-be69-41afed1b4110/swz32_jedi-knight.png";
        }
    }
}