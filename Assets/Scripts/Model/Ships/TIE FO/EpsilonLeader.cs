using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
	namespace TIEFO
	{
		public class EpsilonLeader : TIEFO
		{
			public EpsilonLeader () : base ()
			{
				PilotName = "Epsilon Leader";
				ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/First%20Order/TIE-fo%20Fighter/epsilon-leader.png";
				IsUnique = true;
				PilotSkill = 6;
				Cost = 19;
			}

			public override void InitializePilot ()
			{
				base.InitializePilot ();

				OnCombatPhaseStart += RegisterEpsilonLeaderAbility;
			}

			private void RegisterEpsilonLeaderAbility (GenericShip genericShip)
			{
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Epsilon Leader Ability",
                    TriggerOwner = this.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnCombatPhaseStart,
                    EventHandler = UseEpsilonLeaderAbility
                });
			}

            private void UseEpsilonLeaderAbility(object sender, System.EventArgs e)
            {
                Selection.ThisShip = this;
                Ship.GenericShip EpsilonLeader = Selection.ThisShip;
                // remove a stress from friendly ships at range 1
                foreach (var friendlyShip in EpsilonLeader.Owner.Ships) {
                    Ship.GenericShip friendlyShipDefined = friendlyShip.Value;
                    Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation (EpsilonLeader, friendlyShipDefined);
                    if (positionInfo.Range == 1) {
                        // remove 1 stress token if it exists
                        friendlyShipDefined.RemoveToken (typeof(Tokens.StressToken));
                    }
                }
                Triggers.FinishTrigger ();
            }
		}
	}
}
