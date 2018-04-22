using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using Tokens;

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
                PilotAbilities.Add(new Abilities.EpsilonLeader());
            }
		}
	}
}

namespace Abilities
{
    public class EpsilonLeader : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers += RegisterEpsilonLeaderAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers -= RegisterEpsilonLeaderAbility;
        }

        private void RegisterEpsilonLeaderAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, UseEpsilonLeaderAbility);
        }

        private void UseEpsilonLeaderAbility(object sender, System.EventArgs e)
        {
            List<GenericToken> tokensToRemove = new List<GenericToken>();

            Vector2 range = new Vector2(1, 1);
            foreach(GenericShip friendlyShip in Board.BoardManager.GetShipsAtRange(HostShip, range, Team.Type.Friendly))
            {
                GenericToken stressToken = friendlyShip.Tokens.GetToken(typeof(StressToken));
                if (stressToken != null)
                {
                    tokensToRemove.Add(stressToken);
                }
            }

            Actions.RemoveTokens(tokensToRemove, Triggers.FinishTrigger);
        }
    }
}
