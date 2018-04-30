using Upgrade;
using Ship;
using GameModes;
using Abilities;
using Tokens;
using System;
using UnityEngine;

namespace UpgradesList
{
    public class CaptainRex : GenericUpgrade
    {
        public CaptainRex() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Captain Rex";
            Cost = 2;
            isUnique = true;

            // AvatarOffset = new Vector2(84, 0);

            UpgradeAbilities.Add(new CaptainRexAbility());
        }
 
        public override bool IsAllowedForShip(Ship.GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
     }
}
 
namespace Abilities
{
	public class CaptainRexAbility : GenericAbility
    {
		// After you perform an attack that does not hit, you may assign
		// 1 focus token to your ship
        public override void ActivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker += RegisterCaptainRexAbility;
        }
 
        public override void DeactivateAbility()
        {
			HostShip.OnAttackMissedAsAttacker -= RegisterCaptainRexAbility;
        }
 
        private void RegisterCaptainRexAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackMissed, ShowDecision);
        }
 
        private void ShowDecision(object sender, System.EventArgs e)
        {
			if (!alwaysUseAbility) {
				// give user the option to use ability
				AskToUseAbility (AlwaysUseByDefault, UseAbility, null, null, true);
			} else {
				Messages.ShowInfoToHuman(HostShip.PilotName + " gained focus from Captain Rex (auto)");
				HostShip.Tokens.AssignToken(new FocusToken(HostShip), Triggers.FinishTrigger);
			}
        }
 
        private void UseAbility(object sender, System.EventArgs e)
        {
			Messages.ShowInfoToHuman(HostShip.PilotName + " gained focus from Captain Rex");
            HostShip.Tokens.AssignToken(new FocusToken(HostShip), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}