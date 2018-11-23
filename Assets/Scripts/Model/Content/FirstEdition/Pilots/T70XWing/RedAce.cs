using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace FirstEdition.T70XWing
    {
        public class RedAce : T70XWing
        {
            public RedAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Red Ace\"",
                    6,
                    29,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.RedAceAbility)
                );

                ModelInfo.SkinName = "Red";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class RedAceAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            //Resets ability on round end.
            Phases.Events.OnRoundEnd += ClearAbilityUsed;

            HostShip.OnShieldLost += RegisterCanGetEvadeToken;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnRoundEnd -= ClearAbilityUsed;

            HostShip.OnShieldLost -= RegisterCanGetEvadeToken;
        }

        private void RegisterCanGetEvadeToken()
        {
            if (IsAbilityUsed) return;

            IsAbilityUsed = true;
            RegisterAbilityTrigger(TriggerTypes.OnShieldIsLost, AssignEvadeToken);
        }

        private void AssignEvadeToken(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("\"Red Ace\": Evade token is assigned");
            HostShip.Tokens.AssignToken(typeof(EvadeToken), Triggers.FinishTrigger);
        }

        private void ClearAbilityUsed()
        {
            IsAbilityUsed = false;
        }
    }
}