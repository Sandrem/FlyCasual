using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tokens;

namespace Ship
{
    namespace AlphaClassStarWing
    {
        public class MajorVynder : AlphaClassStarWing
        {
            public MajorVynder() : base()
            {
                PilotName = "Major Vynder";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/Alpha-class%20Star%20Wing/lieutenant-karsabi.png";
                PilotSkill = 7;
                Cost = 26;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();

                AfterGotNumberOfDefenceDice += MajorVynderAbility;
            }

            private void MajorVynderAbility(ref int diceNumber)
            {
                if (HasToken(typeof(WeaponsDisabledToken))) diceNumber++;
            }
        }
    }
}
