using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFO
    {
        public class EpsilonLeader : TIEFO
        {
            public EpsilonLeader() : base()
            {
                PilotName = "Epsilon Leader";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/First%20Order/TIE-fo%20Fighter/epsilon-leader.png";
                IsUnique = true;
                PilotSkill = 6;
                Cost = 19;
				PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Tech);
            }
            public override void InitializePilot()
            {
                base.InitializePilot();

                OnCombatPhaseStart += EpsilonLeaderAbility;
                OnDestroyed += RemoveEpsilonLeaderAbility;
            }

			private void EpsilonLeaderAbility(GenericShip genericShip)
            {
                Ship.GenericShip EpsilonLeader = null;
				foreach (var friendlyShip in Combat.Attacker.Owner.Ships) {
					if (friendlyShip.Value.GetType () == typeof(Ship.TIEFO.EpsilonLeader)) {
						EpsilonLeader = friendlyShip.Value;
						break;
					}
				}
	            if (EpsilonLeader != null)
	            {
					// loop through again to check at range 1 and remove stress
					foreach (var friendlyShip in Combat.Attacker.Owner.Ships) {
						Ship.GenericShip friendlyShipDefined = friendlyShip.Value;
						Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation(EpsilonLeader, friendlyShipDefined);
		                if (positionInfo.Range == 1)
		                {
							// remove 1 stress token if it exists
							friendlyShipDefined.RemoveToken(typeof(Tokens.StressToken));
		                }
					}
	            }
            }

            private void RemoveEpsilonLeaderAbility(GenericShip ship)
            {
                OnDestroyed -= RemoveEpsilonLeaderAbility;
            }
        }
    }
}