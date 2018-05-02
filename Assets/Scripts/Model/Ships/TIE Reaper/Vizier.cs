using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEReaper
    {
        public class Vizier : TIEReaper
        {
            public Vizier() : base()
            {
                PilotName = "\"Vizier\"";
                PilotSkill = 3;
                Cost = 23;

                IsUnique = true;
            }
        }
    }
}
