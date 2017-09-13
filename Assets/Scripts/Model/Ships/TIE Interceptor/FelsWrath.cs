using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;

namespace Ship
{
	namespace TIEInterceptor
	{
		public class FelsWrath : TIEInterceptor
		{
			protected bool isDestructionIsDelayed = false;

			public FelsWrath() : base()
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
			}

			public override void IsHullDestroyedCheck(Action callBack)
			{
				if (Hull == 0 && !IsDestroyed && !isDestructionIsDelayed)
				{
					isDestructionIsDelayed = true;

					Triggers.RegisterTrigger(
						new Trigger()
						{
							Name = "Fel's Wrath Ability",
							TriggerOwner = this.Owner.PlayerNo,
							TriggerType = TriggerTypes.OnEndPhaseStart,
							EventHandler = delegate{
								CleanUpFelsWrath(callBack);
							}
						}
					);
				}
				else
				{
					callBack();
				}
			}

			private void CleanUpFelsWrath(Action callBack){
				DestroyShip(callBack, true);
			}
		}
	}
}
