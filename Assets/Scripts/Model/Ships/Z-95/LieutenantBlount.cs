using UnityEngine;
using Ship;
using System.Collections;
using System.Collections.Generic;
using SubPhases;
using Abilities;
using Board;
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

				SkinName = "Red";
			}
		}
	}
}

namespace Abilities
{
	public class LtBlountAbiliity : GenericAbility
	{
		// Attack is considered a hit, even if there
		// are no uncancelled hit/crit results
		public override void ActivateAbility()
		{
			HostShip.OnAttackMissedAsAttacker += RegisterLtBlountAbility;
		}

		public override void DeactivateAbility()
		{
			HostShip.OnAttackMissedAsAttacker -= RegisterLtBlountAbility;
		}

		private void RegisterLtBlountAbility()
		{
			RegisterAbilityTrigger (TriggerTypes.OnAttackMissed, MakeAttackHit);
		}

		private void MakeAttackHit(object sender, System.EventArgs e)
		{
			Messages.ShowInfo("Lieutenant Blount's attacks\nare always considered a hit");
			Combat.Attacker.CallShotHitAsAttacker();
			Combat.Attacker.CallShotHitAsDefender();
			Combat.Defender.CallOnAttackHitAsDefender();
			Combat.Attacker.CallOnAttackHitAsAttacker();
			Triggers.ResolveTriggers(TriggerTypes.OnAttackMissed, Triggers.FinishTrigger);
		}
	}
}
