using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using SubPhases;
using Tokens;
using RuleSets;

namespace Ship
{
	namespace ProtectorateStarfighter
	{
		public class JoyRekkoff : ProtectorateStarfighter, ISecondEditionPilot
		{
			public JoyRekkoff() : base()
			{
				PilotName = "Joy Rekkoff";
				PilotSkill = 4;
				Cost = 57;

                ImageUrl = "https://i.imgur.com/O4xI9p6.png";

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
			}

            public void AdaptPilotToSecondEdition()
            {
                // No adaptation is required
            }
        }
	}
}