using Ship;
using SubPhases;
using System;

namespace Ship
{
    namespace LambdaShuttle
    {
        public class CaptainYorr : LambdaShuttle
        {
            public CaptainYorr() : base()
            {
                PilotName = "Captain Yorr";
                PilotSkill = 4;
                Cost = 24;
                IsUnique = true;

                PilotAbilities.Add(new PilotAbilitiesNamespace.CaptainYorrAbility());
            }
        }
    }
}



namespace PilotAbilitiesNamespace
{
    public class CaptainYorrAbility : GenericPilotAbility
    {

        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            GenericShip.BeforeTokenIsAssignedGlobal += CaptainYorrPilotAbility;

            host.OnDestroyed += RemoveCaptainYorrAbility;
        }

        private void CaptainYorrPilotAbility(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(Tokens.StressToken) && ship.Owner == Host.Owner && ship != Host && Host.TokenCount(typeof(Tokens.StressToken)) <= 2)
            {

                Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation(ship, Host);
                if (positionInfo.Range <= 2)
                {
                    TargetShip = ship;
                    RegisterAbilityTrigger(TriggerTypes.OnBeforeTokenIsAssigned, ShowDecision);
                }
            }
        }

        private void ShowDecision(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, UseCaptainYorrAbility);
        }

        private void UseCaptainYorrAbility(object sender, System.EventArgs e)
        {            
            Host.AssignToken(TargetShip.TokenToAssign, delegate {
                TargetShip.TokenToAssign = null;
                TargetShip = null;
                DecisionSubPhase.ConfirmDecision();
            });
        }

        private void RemoveCaptainYorrAbility(GenericShip ship)
        {
            GenericShip.BeforeTokenIsAssignedGlobal -= CaptainYorrPilotAbility;
            Host.OnDestroyed -= RemoveCaptainYorrAbility;
        }
    }
}
