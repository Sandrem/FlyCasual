using System;

namespace Ship
{
	namespace TIEInterceptor
	{
		public class FelsWrath : TIEInterceptor
		{
			protected bool IsDestructionIsDelayed;

			public FelsWrath()
			{
				PilotName = "\"Fel's Wrath\"";
				ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/1/12/Fel%27s_Wrath.png";
				PilotSkill = 5;
				Cost = 23;

				IsUnique = true;
			}

			public override void InitializePilot ()
			{
				base.InitializePilot ();
				Phases.OnCombatPhaseEnd += ProcessFelsWrath;
			}

			public override void IsHullDestroyedCheck(Action callBack)
			{
				if (Hull == 0 && !IsDestroyed && !IsDestructionIsDelayed)
				{
					IsDestructionIsDelayed = true;
				}

				callBack();
			}

			public void ProcessFelsWrath()
			{
				Triggers.RegisterTrigger(
					new Trigger()
					{
						Name = "Fel's Wrath Ability",
						TriggerOwner = Owner.PlayerNo,
						TriggerType = TriggerTypes.OnEndPhaseStart,
						EventHandler = CleanUpFelsWrath
					}
				);
			}

			private void CleanUpFelsWrath(object sender, EventArgs e){

				if (IsDestructionIsDelayed) {
					Selection.ThisShip = this;
					DestroyShip (Triggers.FinishTrigger, true);
				} else {
					Triggers.FinishTrigger ();
				}
			}
		}
	}
}
