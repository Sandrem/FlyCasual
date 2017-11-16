using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Vcx100
    {
        public class KananJarrus : Vcx100
        {
            public KananJarrus() : base()
            {
                PilotName = "Kanan Jarrus";
                PilotSkill = 5;
                Cost = 38;

                IsUnique = true;
            }
        }
    }
}
