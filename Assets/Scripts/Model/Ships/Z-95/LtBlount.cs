using UnityEngine;
using Ship;
using System.Collections;
using System.Collections.Generic;
using SubPhases;
using Abilities;
using BoardTools;
using System.Linq;

namespace Ship
{
	namespace Z95
	{
		public class LtBlount : Z95
		{
			public LtBlount() : base()
			{
				PilotName = "Lieutenant Blount";
				PilotSkill = 6;
				Cost = 17;
				IsUnique = true;
				PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

				PilotAbilities.Add(new LtBlountAbiliity());
				faction = Faction.Rebel;

				SkinName = "Blount";
			}
		}
	}
}

namespace Abilities
{
	public class LtBlountAbiliity : GenericAbility
	{
		public override void ActivateAbility()
		{
			HostShip.AttackIsAlwaysConsideredHit = true;
		}

		public override void DeactivateAbility()
		{
			HostShip.AttackIsAlwaysConsideredHit = false;
		}
	}
}