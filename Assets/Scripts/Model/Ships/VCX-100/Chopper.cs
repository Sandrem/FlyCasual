using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Vcx100
    {
        public class Chopper : Vcx100
        {
            public Chopper() : base()
            {
                PilotName = "\"Chopper\"";
                PilotSkill = 4;
                Cost = 37;

                IsUnique = true;
            }
        }
    }
}
