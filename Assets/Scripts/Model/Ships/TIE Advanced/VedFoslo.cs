using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Movement;
using GameModes;
using RuleSets;

namespace Ship
{
    namespace TIEAdvanced
    {
        public class VedFoslo : TIEAdvanced, ISecondEditionPilot
        {
            public VedFoslo() : base()
            {
                PilotName = "Ved Foslo";
                PilotSkill = 4;
                Cost = 47;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new Abilities.JunoEclipseAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                //Not required
            }
        }
    }
}