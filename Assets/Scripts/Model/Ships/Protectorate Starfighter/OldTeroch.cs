using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using SubPhases;

namespace Ship
{
	namespace ProtectorateStarfighter
	{
		public class OldTeroch : ProtectorateStarfighter
		{
			public OldTeroch() : base()
			{
				PilotName = "Old Teroch";
				PilotSkill = 7;
				Cost = 26;
				IsUnique = false; // for debug only ;)
				ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/Protectorate%20Starfighter/old-teroch.png";

				PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
				PilotAbilities.Add(new Abilities.OldTerochAbility());
			}
		}
	}
}

namespace Abilities
{
	// At the start of the combat phase, you may choose 1 ennemy ship
	//  at range 1. If you're inside its firing arc, it discards all
	//  focus and evade tokens.
	public class OldTerochAbility : GenericAbility
	{
		public override void ActivateAbility()
		{
			HostShip.OnCombatPhaseStart += CheckOldTerochAbility;
		}

		public override void DeactivateAbility()
		{
			HostShip.OnCombatPhaseStart -= CheckOldTerochAbility;
		}

		private void CheckOldTerochAbility(Ship.GenericShip host)
		{
			// give user the option to use ability
			RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskSelectShip);
		}

		private void AskSelectShip(object sender, System.EventArgs e)
		{
			// Available selection are only within Range 1.
			// TODO : build the list if the enemy can fire to the ship
			SelectTargetForAbility(ActivateOldTerochAbility, new List<TargetTypes>() { TargetTypes.Enemy }, new Vector2(1, 1), null, true);
		}
			
		private void ActivateOldTerochAbility()
		{
			Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(HostShip, TargetShip);
			// Range is already checked in "SelectTargetForAbility", only check if the Target can fire to Host
			if (TargetShip.InPrimaryWeaponFireZone(HostShip))
			{
				Messages.ShowInfo(HostShip.PilotName + " removed focus and evade token\nto " + TargetShip.PilotName);
				TargetShip.RemoveToken(typeof(Tokens.FocusToken), '*', true);
				TargetShip.RemoveToken(typeof(Tokens.EvadeToken), '*', true);
				SelectShipSubPhase.FinishSelection ();
			}
			else {
				Messages.ShowError(HostShip.PilotName + " is not within " + TargetShip.PilotName + " firing arc.");
			}
		}
	}
}
