using System.Collections.Generic;
using Ship;
using SubPhases;
using Tokens;
using BoardTools;
using RuleSets;

namespace Ship
{
    namespace ProtectorateStarfighter
    {
        public class OldTeroch : ProtectorateStarfighter, ISecondEditionPilot
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

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 56;
                
                PilotAbilities.RemoveAll(ability => ability is Abilities.OldTerochAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.OldTerochAbility());
            }
        }
	}
}

namespace Abilities
{
    // At the start of the combat phase, you may choose 1 enemy ship
    //  at range 1. If you're inside its firing arc, it discards all
    //  focus and evade tokens.
    public class OldTerochAbility : GenericAbility
	{
        protected string AbilityDescription = "Choose a ship. If you are inside its firing arc, it discards all focus and evade tokens.";
		public override void ActivateAbility()
		{
            Phases.Events.OnCombatPhaseStart_Triggers += CheckOldTerochAbility;
		}

		public override void DeactivateAbility()
		{
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckOldTerochAbility;
		}

		private void CheckOldTerochAbility()
		{
			// give user the option to use ability
			RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskSelectShip);
		}

		private void AskSelectShip(object sender, System.EventArgs e)
		{
			// first check if there is at least one enemy at range 1
			if (TargetsForAbilityExist(FilterTargetsOfAbility)) {
                // Available selection are only within Range 1.
                // TODO : build the list if the enemy can fire to the ship
                SelectTargetForAbility(
                    ActivateOldTerochAbility,
                    FilterTargetsOfAbility,
                    GetAiPriorityOfTarget,
                    HostShip.Owner.PlayerNo,
                    true,
                    null,
                    HostShip.PilotName,
                    AbilityDescription,
                    HostShip.ImageUrl
                );
			} else {
				// no enemy in range
				Triggers.FinishTrigger();
			}
		}

        private bool FilterTargetsOfAbility(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) && FilterTargetsByRange(ship, 1, 1) && FilterTargetInEnemyArcWithTokens(ship);
        }

        private int GetAiPriorityOfTarget(GenericShip ship)
        {
            int priority = 50;

            priority += ship.Tokens.CountTokensByType(typeof(FocusToken)) * 10;
            priority += ship.Tokens.CountTokensByType(typeof(EvadeToken)) * 10;
            if (Actions.HasTargetLockOn(ship, HostShip)) priority += 10;
            priority += ship.Firepower*10;

            return priority;
        }

        private bool FilterTargetInEnemyArcWithTokens(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(ship, HostShip, ship.PrimaryWeapon);
            return shotInfo.InArc && (ship.Tokens.HasToken(typeof(FocusToken)) || ship.Tokens.HasToken(typeof(EvadeToken)));
        }

        private void ActivateOldTerochAbility()
		{
			ShotInfo shotInfo = new ShotInfo(TargetShip, HostShip, HostShip.PrimaryWeapon);
			// Range is already checked in "SelectTargetForAbility", only check if the Host is in the Target firing arc.
			// Do not use InPrimaryWeaponFireZone, reason :
			//		VCX-100 without docked Phantom cannot shoot using special rear arc,so InPrimaryWeaponFireZone
			//		will be false but this is still arc, so ability should be active - so just InArc is checked,
			//		even ship cannot shoot from it.
			if (shotInfo.InArc == true)
			{
                DiscardTokens();
            }
            else
            {
				Messages.ShowError(HostShip.PilotName + " is not within " + TargetShip.PilotName + " firing arc.");
			}
		}

        protected virtual void DiscardTokens()
        {
            Messages.ShowInfo(string.Format("{0} discarded all Focus and Evade tokens from {1}", HostShip.PilotName, TargetShip.PilotName));
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

    namespace SecondEdition
    {
        //At the start of the Engagement Phase, you may choose 1 enemy ship at range 1. If you do and you are in its (front arc), it removes all of its green tokens.
        public class OldTerochAbility : Abilities.OldTerochAbility
        {
            public OldTerochAbility()
            {                
                AbilityDescription = "Choose a ship. If you are inside its firing arc, it removes all of its green tokens.";
            }

            protected override void DiscardTokens()
            {
                Messages.ShowInfo(string.Format("{0} removed all green tokens from {1}", HostShip.PilotName, TargetShip.PilotName));
                TargetShip.Tokens.RemoveAllTokensByColor(TokenColors.Green, SelectShipSubPhase.FinishSelection);
            }
        }
    }
}