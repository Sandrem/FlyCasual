using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class GarvenDreis : XWing
        {
            public GarvenDreis() : base()
            {
                PilotName = "Garven Dreis";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/f/f8/Garven-dreis.png";
                IsUnique = true;
                PilotSkill = 6;
                Cost = 26;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                AfterTokenIsSpent += GarvenDreisPilotAbility;
            }

            private void GarvenDreisPilotAbility(GenericShip ship)
            {
                Debug.Log("Trigger!");
                //Todo: Start Selection subPhase
            }

        }
    }
}
