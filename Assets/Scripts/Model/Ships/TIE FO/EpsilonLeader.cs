using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

namespace Ship
{
	namespace TIEFO
	{
		public class EpsilonLeader : TIEFO
		{
			public EpsilonLeader () : base ()
			{
				PilotName = "\"Epsilon Leader\"";
				PilotSkill = 6;
				Cost = 19;

                IsUnique = true;
                PilotAbilities.Add(new PilotAbilitiesNamespace.EpsilonLeader());
            }
		}
	}
}

namespace PilotAbilitiesNamespace
{
    public class EpsilonLeader : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Host.OnCombatPhaseStart += RegisterEpsilonLeaderAbility;
        }

        private void RegisterEpsilonLeaderAbility(GenericShip genericShip)
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, UseEpsilonLeaderAbility);
        }

        private void UseEpsilonLeaderAbility(object sender, System.EventArgs e)
        {
            // remove a stress from friendly ships at range 1
            foreach (var friendlyShip in Host.Owner.Ships)
            {
                GenericShip friendlyShipDefined = friendlyShip.Value;
                Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation(Host, friendlyShipDefined);
                if (positionInfo.Range == 1)
                {
                    // remove 1 stress token if it exists
                    friendlyShipDefined.RemoveToken(typeof(Tokens.StressToken));
                }
            }
            Triggers.FinishTrigger();
        }
    }
}
