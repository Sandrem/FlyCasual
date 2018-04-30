using Upgrade;
using Ship;
using Abilities;
using Tokens;
using UnityEngine;

namespace UpgradesList
{
	public class Rage: GenericUpgrade
	{

		public Rage () :base()
		{
			Types.Add(UpgradeType.Elite);
			Name = "Rage";
			Cost = 1;

			UpgradeAbilities.Add (new RageAbility ());

            AvatarOffset = new Vector2(53, 1);
        }
	}
}


namespace Abilities
{
	public class RageAbility: GenericAbility
	{
		public override void ActivateAbility()
		{
			HostShip.AfterGenerateAvailableActionsList += RageAddAction;
		}

		public override void DeactivateAbility()
		{
			HostShip.AfterGenerateAvailableActionsList -= RageAddAction;
		}

		private void RageAddAction(Ship.GenericShip host)
		{
			ActionsList.GenericAction action = new ActionsList.RageAction()
			{
				ImageUrl = HostUpgrade.ImageUrl,
				Host = HostShip
			};
			host.AddAvailableAction(action);
		}		
	}
}


namespace ActionsList
{
	public class RageAction: GenericAction
	{
		
		public RageAction()
		{
			Name = EffectName = "Rage"; 
		}
	

		public override void ActionTake()
		{
			Host = Selection.ThisShip;
			//Adding Rage reroll effect
			Host.AfterGenerateAvailableActionEffectsList += AddRageCondition; 
			Phases.OnEndPhaseStart_NoTriggers += RemoveRageCondition;

			//Rage Condition for reroll dices on each attach during this round
			Messages.ShowInfo("Rage: Condition assigned");
			Host.Tokens.AssignCondition(new Conditions.RageCondition(Host));

			//Assigns one focus and two stress tokens
			Messages.ShowInfo("Rage: Focus assigned");
			Host.Tokens.AssignToken (new Tokens.FocusToken (Host), delegate { assignStressTokensRecursively (2); });
		}


		//Assigns recursively stress tokens delegating its callback responsability on each call until last one
		private void assignStressTokensRecursively (int tokens){

			if (tokens > 0)
			{
				tokens--;
				string message = (tokens == 0) ? "Rage: Second Stress assigned" : "Rage: First Stress assigned";
				Messages.ShowInfo(message);
				Host.Tokens.AssignToken (new Tokens.StressToken (Host), delegate { assignStressTokensRecursively (tokens); });
			}
			else
			{
				Phases.CurrentSubPhase.CallBack();
			}
		}


		private void AddRageCondition(Ship.GenericShip ship)
		{
			ship.AddAvailableActionEffect(this);
		}


		private void RemoveRageCondition()
		{
			Host.Tokens.RemoveCondition(typeof(Conditions.RageCondition));
			Host.AfterGenerateAvailableActionEffectsList -= AddRageCondition;

			Phases.OnEndPhaseStart_NoTriggers -= RemoveRageCondition;
		}
			

		public override bool IsActionEffectAvailable ()
		{			
			return (Combat.AttackStep == CombatStep.Attack);
		}


		public override int GetActionEffectPriority()
		{
			//TODO: More Complex
			return 20;
		}

		//Reroll is the effect of Rage
		public override void ActionEffect(System.Action callBack)
		{
			IsReroll = true; //These effect is a reroll effect

			DiceRerollManager diceRerollManager = new DiceRerollManager
			{
				NumberOfDiceCanBeRerolled = 3,
				CallBack = callBack
			};
			diceRerollManager.Start();
		}

	}

}


namespace Conditions
{

	public class RageCondition : Tokens.GenericToken
	{
		public RageCondition(GenericShip host) : base(host)
		{
			Name = "Buff Token";
			Temporary = false;
			Tooltip = new UpgradesList.Rage().ImageUrl;
		}
	}

}