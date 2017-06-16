using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YWing
    {
        public class DutchVander : YWing
        {
            public DutchVander() : base()
            {
                PilotName = "Dutch Vander";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/b/bf/Dutch_Vander.png";
                IsUnique = true;
                PilotSkill = 6;
                Cost = 23;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                AfterTokenIsSpent += GarvenDreisPilotAbility;
            }

            private void GarvenDreisPilotAbility(System.Type type)
            {
                if (type == typeof(Tokens.BlueTargetLockToken))
                {
                    Debug.Log("Trigger!");
                    //Todo: Start Selection subPhase
                }
            }

        }
    }
}
