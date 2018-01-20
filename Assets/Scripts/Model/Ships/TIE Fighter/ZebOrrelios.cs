using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;

namespace Ship
{
    namespace TIEFighter
    {
        public class ZebOrrelios : TIEFighter
        {
            public ZebOrrelios() : base()
            {
                PilotName = "\"Zeb\" Orrelios";
                PilotSkill = 3;
                Cost = 13;

                faction = Faction.Rebel;

                PilotAbilities.Add(new ZebOrreliosPilotAbility());
            }
        }
    }
}
