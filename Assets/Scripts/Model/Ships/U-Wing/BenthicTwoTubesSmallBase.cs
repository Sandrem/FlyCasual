using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods.ModsList;
using Abilities;

namespace Ship
{
    namespace UWing
    {
        public class BenthicTwoTubesSmallBase : UWingSmallBase
        {
            public BenthicTwoTubesSmallBase() : base()
            {
                RequiredMods.Add(typeof(UWingSmallBaseMod));

                PilotName = "Benthic Two Tubes (Small Base)";
                PilotSkill = 4;
                Cost = 24;

                IsUnique = true;

                SkinName = "Partisan";

                ImageUrl = "https://i.imgur.com/jaHvfbI.png";

                PilotAbilities.Add(new BenthicTwoTubesAbility());
            }
        }
    }
}
