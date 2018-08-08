using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;
using Abilities;
using RuleSets;

namespace Ship
{
	namespace ARC170
	{
		public class Ibtisam : ARC170, ISecondEditionPilot
		{
			public Ibtisam() : base()
			{
				PilotName = "Ibtisam";
				PilotSkill = 3;
				Cost = 50;

				IsUnique = true;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

				PilotAbilities.Add(new BraylenStrammPilotAbility());
			}

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
	}
}