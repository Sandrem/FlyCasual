using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using RuleSets;

namespace Ship
{
    namespace EscapeCraft
    {
        public class LandoCalrissianEscapeCraft : EscapeCraft, ISecondEditionPilot
        {
            public LandoCalrissianEscapeCraft() : base()
            {
                PilotName = "Lando Calrissian";
                PilotSkill = 4;
                Cost = 26;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SecondEdition.LandoCalrissianScumPilotAbilitySE());

                SEImageNumber = 226;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}