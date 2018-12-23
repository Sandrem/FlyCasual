using System;
using System.Collections;
using System.Collections.Generic;
using Ship;
using SubPhases;
using Tokens;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class HobbbieKlivian : XWing
        {
            public HobbbieKlivian() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Hobbie\" Klivian",
                    5,
                    25,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.HobbieKlivianAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class HobbieKlivianAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsSpent += RegisterHobbieKlivianPilotAbility;
            HostShip.OnTargetLockIsAcquired += RegisterHobbieKlivianPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsSpent -= RegisterHobbieKlivianPilotAbility;
            HostShip.OnTargetLockIsAcquired -= RegisterHobbieKlivianPilotAbility;
        }

        private void RegisterHobbieKlivianPilotAbility(GenericShip ship, System.Type type)
        {
            if (type == typeof(BlueTargetLockToken) && HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsSpent, AskToRemoveStress);
            }
        }

        private void RegisterHobbieKlivianPilotAbility(GenericShip ship)
        {
            if (HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTargetLockIsAcquired, AskToRemoveStress);
            }
        }

        private void AskToRemoveStress(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(
                    AlwaysUseByDefault,
                    DecideRemoveStress,
                    DecideDontRemoveStress,
                    null,
                    true,
                    string.Format("Should {0} remove 1 stress token?", HostShip.PilotInfo.PilotName)
                );
            }
            else
            {
                RemoveStress(Triggers.FinishTrigger);
            }
        }

        private void RemoveStress(Action callback)
        {
            HostShip.Tokens.RemoveToken(typeof(StressToken), callback);
        }

        private void DecideRemoveStress(object sender, EventArgs e)
        {
            RemoveStress(DecisionSubPhase.ConfirmDecision);
        }

        private void DecideDontRemoveStress(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecision();
        }
    }
}
