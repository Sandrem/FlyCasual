using Ship;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace Ship
{
    namespace FirstEdition.TIEFoFighter
    {
        public class EpsilonLeader : TIEFoFighter
        {
            public EpsilonLeader() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Epsilon Leader\"",
                    6,
                    19,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.EpsilonLeader)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class EpsilonLeader : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterEpsilonLeaderAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterEpsilonLeaderAbility;
        }

        private void RegisterEpsilonLeaderAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, UseEpsilonLeaderAbility);
        }

        private void UseEpsilonLeaderAbility(object sender, System.EventArgs e)
        {
            List<GenericToken> tokensToRemove = new List<GenericToken>();

            Vector2 range = new Vector2(1, 1);
            foreach (GenericShip friendlyShip in BoardTools.Board.GetShipsAtRange(HostShip, range, Team.Type.Friendly))
            {
                GenericToken stressToken = friendlyShip.Tokens.GetToken(typeof(StressToken));
                if (stressToken != null)
                {
                    tokensToRemove.Add(stressToken);
                }
            }

            ActionsHolder.RemoveTokens(tokensToRemove, Triggers.FinishTrigger);
        }
    }
}