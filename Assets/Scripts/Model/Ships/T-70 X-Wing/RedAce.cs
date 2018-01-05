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
            Phases.OnPlanningPhaseStart += GetShipShields;

            //Determines if abilities / debris damaged the ship.
            Phases.OnActivationPhaseEnd += RegisterRedAceAbility;           
            HostShip.OnAttackHitAsDefender += RegisterRedAceAbility;            

            //Resets ability on round end.
            Phases.OnRoundEnd += ClearAbilityUsed;
        }

        public override void DeactivateAbility()
        {
            Phases.OnPlanningPhaseStart -= GetShipShields;

            HostShip.OnAttackHitAsDefender -= RegisterRedAceAbility;
            Phases.OnActivationPhaseEnd -= RegisterRedAceAbility;

            Phases.OnRoundEnd -= ClearAbilityUsed;
        }

        private void GetShipShields()
        {
            if (HostShip.Shields > 0)
            {
                _originalShieldValue = HostShip.Shields;
            }
        }

        private void CanGetEvadeToken(object sender, EventArgs e)
        {            
            if (IsAbilityUsed || _originalShieldValue == 0)
            {
                return;
            }

            IsAbilityUsed = true;
            HostShip.AssignToken(new Tokens.EvadeToken(), Triggers.FinishTrigger);            
        }

        private void ClearAbilityUsed()
        {
            IsAbilityUsed = false;
        }

        private void RegisterRedAceAbility()
        {
            if (HostShip.Shields < _originalShieldValue)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, CanGetEvadeToken);
            }            
        }
    }
}