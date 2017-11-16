using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Vcx100
    {
        public class HeraSyndulla : Vcx100
        {
            public HeraSyndulla() : base()
            {
                PilotName = "Hera Syndulla";
                PilotSkill = 7;
                Cost = 40;

                IsUnique = true;
            }
        }
    }
}
