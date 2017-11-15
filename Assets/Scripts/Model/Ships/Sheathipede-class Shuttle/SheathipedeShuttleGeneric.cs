using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace SheathipedeShuttle
    {
        public class SheathipedeShuttleGeneric : SheathipedeShuttle
        {
            public SheathipedeShuttleGeneric() : base()
            {
                PilotName = "Sheathipede Shuttle Generic";
                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/12/92/129299da-fdef-4c3b-96d8-06c23270de7e/swx72-ap-5.png";
                PilotSkill = 1;
                Cost = 15;
            }
        }
    }
}
