using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Z95
    {
        public class BanditSquadronPilot : Z95, ISecondEditionPilot
        {
            public BanditSquadronPilot() : base()
            {
                PilotName = "Bandit Squadron Pilot";
                PilotSkill = 2;
                Cost = 12;

                faction = Faction.Rebel;

                //SkinName = "Bandit Squadron";
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 1;
            }
        }
    }
}
