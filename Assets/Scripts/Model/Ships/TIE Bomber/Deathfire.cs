using System;
using System.Linq;
using ActionsList;
using Ship;

namespace Ship
{
    namespace TIEBomber
    {
        public class Deathfire : TIEBomber
        {
            public Deathfire() : base()
            {
                PilotName = "\"Deathfire\"";
                PilotSkill = 3;
                Cost = 17;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.DeathfireAbility());
            }
        }
    }
}

namespace Abilities
{
    public class DeathfireAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += CheckDeathfireAbility;
            HostShip.OnActionIsPerformed += CheckDeathfireAbility;            
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= CheckDeathfireAbility;
            HostShip.OnActionIsPerformed -= CheckDeathfireAbility;
        }

        private void CheckDeathfireAbility(GenericAction action)
        {
            if (!IsAbilityUsed && action != null)
            {
                SetIsAbilityIsUsed(HostShip);
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, DeathfireEffect);
            }
        }

        private void CheckDeathfireAbility(GenericShip ship)
        {
            if (!IsAbilityUsed)
            {
                SetIsAbilityIsUsed(HostShip);
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, DeathfireEffect);
            }
        }

        private void DeathfireEffect(object sender, EventArgs e)
        {
            HostShip.GenerateAvailableActionsList();
            var actions = HostShip.GetAvailableActionsList()
                .Where(action => action is BombDropAction)
                .ToList();

            HostShip.AskPerformFreeAction(actions, () =>
            {
                ClearIsAbilityUsedFlag();
                Triggers.FinishTrigger();                
            });
        }        
    }
}
