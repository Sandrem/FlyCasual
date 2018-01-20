﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;

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
            HostShip.OnCombatPhaseStart += RegisterEpsilonLeaderAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatPhaseStart -= RegisterEpsilonLeaderAbility;
        }

        private void RegisterEpsilonLeaderAbility(GenericShip genericShip)
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, UseEpsilonLeaderAbility);
        }

        private void UseEpsilonLeaderAbility(object sender, System.EventArgs e)
        {
            Vector2 range = new Vector2(1, 1);
            foreach(GenericShip friendlyShip in Board.BoardManager.GetShipsAtRange(HostShip, range, Team.Type.Friendly))
            {
                friendlyShip.RemoveToken(typeof(Tokens.StressToken));
            }
            Triggers.FinishTrigger();
        }
    }
}
