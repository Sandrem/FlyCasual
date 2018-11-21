using Ship;
using Tokens;

namespace Ship
{
    namespace FirstEdition.SheathipedeClassShuttle
    {
        public class AP5 : SheathipedeClassShuttle
        {
            public AP5() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "AP-5",
                    1,
                    15,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.AP5PilotAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class AP5PilotAbility : GenericAbility
    {
        private class TwoShipsArguments : System.EventArgs
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
            TwoShipsArguments twoShipsArguments = (TwoShipsArguments)e;

            AskToUseAbility(
                delegate { return IsUseAP5PilotAbility(twoShipsArguments.Target); },
                delegate { UsePilotAbility(twoShipsArguments); }
            );

        }

        private bool IsUseAP5PilotAbility(GenericShip targetShip)
        {
            bool result = false;
            StressToken stressTokens = (StressToken)targetShip.Tokens.GetToken(typeof(StressToken));

            if (stressTokens != null)
            {
                result = true;
            }

            return result;
        }

        private void UsePilotAbility(TwoShipsArguments twoShipsArguments)
        {
            twoShipsArguments.Host.Tokens.AssignToken(typeof(StressToken), delegate { AssignSecondStressToken(twoShipsArguments); });
        }

        private void AssignSecondStressToken(TwoShipsArguments twoShipsArguments)
        {
            twoShipsArguments.Host.Tokens.AssignToken(typeof(StressToken), delegate { RemoveStressTokenFromTarget(twoShipsArguments); });
        }

        private void RemoveStressTokenFromTarget(TwoShipsArguments twoShipsArguments)
        {
            twoShipsArguments.Target.Tokens.RemoveToken(
                typeof(StressToken),
                SubPhases.DecisionSubPhase.ConfirmDecision
            );
        }
    }
}