using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Tokens;
using System;

namespace Ship
{
    namespace SheathipedeShuttle
    {
        public class AP5 : SheathipedeShuttle
        {
            public AP5() : base()
            {
                PilotName = "AP-5";
                PilotSkill = 1;
                Cost = 15;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.AP5PilotAbility());
            }
        }
    }
}

namespace Abilities
{
    public class AP5PilotAbility : GenericAbility
    {
        private class TwoShipsArguments: System.EventArgs
        {
            public GenericShip Host;
            public GenericShip Target;
        }

        public override void ActivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected += RegisterAP5PilotAbilty;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected -= RegisterAP5PilotAbilty;
        }

        private void RegisterAP5PilotAbilty(GenericShip targetShip)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "AP-5's effect",
                TriggerType = TriggerTypes.OnCoordinateTargetIsSelected,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = AskAP5PilotAbility,
                EventArgs = new TwoShipsArguments()
                {
                    Host = this.HostShip,
                    Target = targetShip
                }
            });
        }

        private void AskAP5PilotAbility(object sender, System.EventArgs e)
        {
            TwoShipsArguments twoShipsArguments = (TwoShipsArguments) e;

            AskToUseAbility(
                delegate { return IsUseAP5PilotAbility(twoShipsArguments.Target); },
                delegate { UsePilotAbility(twoShipsArguments); }
            );
            
        }

        private bool IsUseAP5PilotAbility(GenericShip targetShip)
        {
            bool result = false;
            StressToken stressTokens = (StressToken)targetShip.GetToken(typeof(StressToken));

            if (stressTokens != null && stressTokens.Count == 1)
            {
                result = true;
            }

            return result;
        }

        private void UsePilotAbility(TwoShipsArguments twoShipsArguments)
        {
            twoShipsArguments.Host.AssignToken(new StressToken(), delegate { AssignSecondStressToken(twoShipsArguments); });
        }

        private void AssignSecondStressToken(TwoShipsArguments twoShipsArguments)
        {
            twoShipsArguments.Host.AssignToken(new StressToken(), delegate { RemoveStressTokenFromTarget(twoShipsArguments); });
        }

        private void RemoveStressTokenFromTarget(TwoShipsArguments twoShipsArguments)
        {
            twoShipsArguments.Target.RemoveToken(typeof(StressToken));
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }
    }
}
