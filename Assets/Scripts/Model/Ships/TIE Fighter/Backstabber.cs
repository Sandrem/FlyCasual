using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class Backstabber : TIEFighter
        {
            public Backstabber() : base()
            {
                PilotName = "\"Backstabber\"";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/5/52/Backstabber.png";
                IsUnique = true;
                PilotSkill = 6;
                Cost = 16;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                AfterGotNumberOfAttackDices += BackstabberPilotAbility;
            }

            private void BackstabberPilotAbility(ref int diceNumber)
            {
                if (!Actions.InArcCheck(Selection.AnotherShip, Selection.ThisShip))
                {
                    Messages.ShowInfo("Backstabber: Additional dice");
                    diceNumber++;
                }
            }
        }
    }
}
