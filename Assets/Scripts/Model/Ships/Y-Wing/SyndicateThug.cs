using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YWing
    {
        public class SyndicateThug : YWing
        {
            public SyndicateThug() : base()
            {
                PilotName = "Syndicate Thug";
                PilotSkill = 2;
                Cost = 18;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.SalvagedAstromech);

                faction = Faction.Scum;

                SkinName = "Brown";
            }
        }
    }
}
