using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods.ModsList;

namespace Ship
{
    namespace UWing
    {
        public class PartisanRenegadeSmallBase : UWingSmallBase
        {
            public PartisanRenegadeSmallBase() : base()
            {
                RequiredMods.Add(typeof(UWingSmallBaseMod));

                PilotName = "Partisan Renegade (Small Base)";
                ImageUrl = "https://i.imgur.com/aDuffS2.png";
                PilotSkill = 1;
                Cost = 22;

                SkinName = "Partisan";
            }
        }
    }
}
