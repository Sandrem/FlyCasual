using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods.ModsList;

namespace Ship
{
    namespace UWing
    {
        public class BlueSquadronPathfinderSmallBase : UWingSmallBase
        {
            public BlueSquadronPathfinderSmallBase() : base()
            {
                RequiredMods.Add(typeof(UWingSmallBaseMod));

                PilotName = "Blue Squadron Pathfinder (Small Base)";
                ImageUrl = "https://i.imgur.com/ks8jRBa.png";
                PilotSkill = 2;
                Cost = 23;
            }
        }
    }
}
