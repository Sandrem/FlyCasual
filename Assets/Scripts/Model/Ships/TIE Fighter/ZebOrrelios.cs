using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using RuleSets;

namespace Ship
{
    namespace TIEFighter
    {
        public class ZebOrrelios : TIEFighter, ISecondEditionPilot
        {
            public ZebOrrelios() : base()
            {
                PilotName = "\"Zeb\" Orrelios";
                PilotSkill = 3;
                Cost = 13;

                faction = Faction.Rebel;

                PilotAbilities.Add(new ZebOrreliosPilotAbility());

                SpecialModel = "TIE Fighter Rebel";
                SkinName = "Rebel";
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 26;

                SEImageNumber = 49;
            }
        }
    }
}
