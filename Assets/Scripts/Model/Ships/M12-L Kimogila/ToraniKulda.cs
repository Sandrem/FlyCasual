using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace M12LKimogila
    {
        public class ToraniKulda : M12LKimogila
        {
            public ToraniKulda() : base()
            {
                PilotName = "Torani Kulda";
                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/23/8d/238d3ebc-519d-4593-884b-3379d72e5f60/swx70-torani-kulda.png";
                PilotSkill = 8;
                Cost = 27;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new PilotAbilitiesNamespace.ToraniKuldaAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class ToraniKuldaAbility : GenericPilotAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);
        }
    }
}
