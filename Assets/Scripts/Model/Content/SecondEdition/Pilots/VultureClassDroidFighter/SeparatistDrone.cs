using System;
using System.Collections.Generic;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class SeparatistDrone : VultureClassDroidFighter
    {
        public SeparatistDrone()
        {
            PilotInfo = new PilotCardInfo(
                "Separatist Drone",
                3,
                23
            );

            RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/89/cb/89cb527c-4578-410c-9e5b-4ac78815a679/swz31_separatist-drone.png";
        }
    }
}