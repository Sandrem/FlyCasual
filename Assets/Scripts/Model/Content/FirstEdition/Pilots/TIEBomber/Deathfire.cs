using ActionsList;
using Ship;
using System;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEBomber
    {
        public class Deathfire : TIEBomber
        {
            public Deathfire() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Deathfire\"",
                    3,
                    17,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.DeathfireAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
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
            if (!IsAbilityUsed)
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
            var actions = HostShip.GetAvailableActions()
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
