using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace SheathipedeShuttle
    {
        public class ZebOrrelios : SheathipedeShuttle
        {
            public ZebOrrelios() : base()
            {
                PilotName = "\"Zeb\" Orrelios";
                PilotSkill = 3;
                Cost = 16;

                IsUnique = true;

                PilotAbilities.Add(new PilotAbilitiesNamespace.ZebOrreliosPilotAbility());
            }
        }
    }
}
