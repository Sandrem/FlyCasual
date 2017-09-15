using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace BWing
    {
        public class DaggerSquadronPilot : BWing
        {
            public DaggerSquadronPilot() : base()
            {
                PilotName = "Dagger Squadron Pilot";
                ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/6/65/Dagger_Squadron.png";
                PilotSkill = 4;
                Cost = 24;

                SkinName = "Red";
            }
        }
    }
}
