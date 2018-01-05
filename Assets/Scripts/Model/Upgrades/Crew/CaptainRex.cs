using Upgrade;
using Ship;
using GameModes;
using Abilities;
using Tokens;
using System;

namespace UpgradesList
{
    public class CaptainRex : GenericUpgrade
    {
        public CaptainRex() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Captain Rex";
            Cost = 2;
            isUnique = true;
                                              
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
            // give user the option to use ability
            AskToUseAbility(AlwaysUseByDefault, UseAbility);
        }
 
        private void UseAbility(object sender, System.EventArgs e)
        {
			Messages.ShowInfoToHuman("Attacker gained focus from Captain Rex");
            HostShip.AssignToken(new Tokens.FocusToken(), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}