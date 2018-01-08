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
        private int _originalShieldValue = 0;

        public override void ActivateAbility()
        {
            //Gets shield values at the start of the round.
            Phases.OnRoundStart += GetShipShields;
            //Resets ability on round end.
            Phases.OnRoundEnd += ClearAbilityUsed;

            HostShip.OnShieldLost += CanGetEvadeToken;
        }

        public override void DeactivateAbility()
        {
            Phases.OnRoundStart -= GetShipShields;
            Phases.OnRoundEnd -= ClearAbilityUsed;

            HostShip.OnShieldLost -= CanGetEvadeToken;
        }

        private void GetShipShields()
        {
            if (HostShip.Shields > 0)
            {
                _originalShieldValue = HostShip.Shields;
            }
        }

        private void CanGetEvadeToken()
        {
            if (IsAbilityUsed || HostShip.Shields == 0)
            {
                return;
            }

            IsAbilityUsed = true;
            HostShip.AssignToken(new Tokens.EvadeToken(), Phases.CurrentSubPhase.Resume);
        }

        private void ClearAbilityUsed()
        {
            IsAbilityUsed = false;
        }
    }
}