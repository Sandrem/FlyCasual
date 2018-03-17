using Upgrade;
using Ship;
using Abilities;
using Tokens;





namespace UpgradesList
{
	public class Rage: GenericUpgrade
	{

		public Rage () :base()
		{
			Types.Add(UpgradeType.Elite);
			Name = "Rage";
			Cost = 1;
		}

		public override void AttachToShip(Ship.GenericShip host)
		{
			base.AttachToShip(host);
			host.AfterGenerateAvailableActionsList += AddRageAction;
		}

		private void AddRageAction(Ship.GenericShip host)
		{
			ActionsList.GenericAction newAction = new ActionsList.RageAction();
			newAction.ImageUrl = ImageUrl;
			host.AddAvailableAction(newAction);
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
			Phases.OnEndPhaseStart += RemoveRageCondition;

			//Mark as Rage has been used
			Host.Tokens.AssignCondition(new Conditions.RageCondition(Host));

			//These are tokens that applies to use Rage Action

			//Getting 1 Focus token
			Host.Tokens.AssignToken (new Tokens.FocusToken(Host), Phases.CurrentSubPhase.CallBack);

			//Getting 2 Stress tokens
			Host.Tokens.AssignToken (new Tokens.StressToken(Host), Phases.CurrentSubPhase.CallBack);
			Host.Tokens.AssignToken (new Tokens.StressToken(Host), Phases.CurrentSubPhase.CallBack);

		}


		private void AddRageCondition(Ship.GenericShip ship)
		{
			ship.AddAvailableActionEffect(this);
		}


		private void RemoveRageCondition()
		{
			Host.Tokens.RemoveCondition (typeof(Conditions.RageCondition));
			Host.AfterGenerateAvailableActionEffectsList -= AddRageCondition;

			Phases.OnEndPhaseStart -= RemoveRageCondition;
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