using Ship;
using System;

namespace Ship
{
    namespace T70XWing
    {
        public class RedAce : T70XWing
        {
            public RedAce() : base()
            {
                PilotName = "\"Red Ace\"";
                PilotSkill = 6;
                Cost = 29;

                SkinName = "Red";

                IsUnique = true;

                PilotAbilities.Add(new Abilities.RedAceAbility());
            }
        }
    }
}

namespace Abilities
{
    public class RedAceAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            //Resets ability on round end.
            Phases.OnRoundEnd += ClearAbilityUsed;

            HostShip.OnShieldLost += RegisterCanGetEvadeToken;
        }

        public override void DeactivateAbility()
        {
            Phases.OnRoundEnd -= ClearAbilityUsed;

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
            HostShip.Tokens.AssignToken(new Tokens.EvadeToken(HostShip), Triggers.FinishTrigger);
        }

        private void ClearAbilityUsed()
        {
            IsAbilityUsed = false;
        }
    }
}