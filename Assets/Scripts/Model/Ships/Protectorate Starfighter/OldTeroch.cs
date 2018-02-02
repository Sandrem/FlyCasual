using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using SubPhases;
using Tokens;

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

                IsUnique = true;

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
			// first check if there is at least one enemy at range 1
			if (Board.BoardManager.GetShipsAtRange (HostShip, new Vector2 (1, 1), Team.Type.Enemy).Count >= 1) {
				// Available selection are only within Range 1.
				// TODO : build the list if the enemy can fire to the ship
				SelectTargetForAbilityOld (ActivateOldTerochAbility, new List<TargetTypes> () { TargetTypes.Enemy }, new Vector2 (1, 1), null, true);
			} else {
				// no enemy in range
				Triggers.FinishTrigger ();
			}
		}
			
		private void ActivateOldTerochAbility()
		{
			Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(TargetShip, HostShip);
			// Range is already checked in "SelectTargetForAbility", only check if the Host is in the Target firing arc.
			// Do not use InPrimaryWeaponFireZone, reason :
			//		VCX-100 without docked Phantom cannot shoot using special rear arc,so InPrimaryWeaponFireZone
			//		will be false but this is still arc, so ability should be active - so just InArc is checked,
			//		even ship cannot shoot from it.
			if (shotInfo.InArc == true)
			{
                DiscardFocusAndEvadeTokens();
            }
            else
            {
				Messages.ShowError(HostShip.PilotName + " is not within " + TargetShip.PilotName + " firing arc.");
			}
		}

        private void DiscardFocusAndEvadeTokens()
        {
            DiscardAllFocusTokens();
        }

        private void DiscardAllFocusTokens()
        {
            TargetShip.Tokens.RemoveAllTokensByType(
                typeof(FocusToken),
                DiscardAllEvadeTokens
            );
        }

        private void DiscardAllEvadeTokens()
        {
            TargetShip.Tokens.RemoveAllTokensByType(
                typeof(EvadeToken),
                SelectShipSubPhase.FinishSelection
            );
        }
    }
}