using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class Dfs081 : VultureClassDroidFighter
    {
        public Dfs081()
        {
            PilotInfo = new PilotCardInfo(
                "DFS-081",
                3,
                29,
                true,
                abilityType: typeof(Abilities.SecondEdition.Dfs081Ability),
                pilotTitle: "Preservation Programming"
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b4/04/b4044057-fae9-4638-b758-14339c1ce98a/swz29_dfs-081.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Dfs081Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            // TODO
        }

        public override void DeactivateAbility()
        {
            // TODO
        }
    }
}
