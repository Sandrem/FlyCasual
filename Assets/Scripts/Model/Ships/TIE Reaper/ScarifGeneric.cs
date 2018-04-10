using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEReaper
    {
        public class ScarifGeneric : TIEReaper
        {
            public ScarifGeneric() : base()
            {
                PilotName = "Scarif Generic";
                PilotSkill = 1;
                Cost = 21;

                ImageUrl = "https://i.imgur.com/ql7NYgF.jpg";
            }
        }
    }
}
