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

                PilotAbilities.Add(new Abilities.CaptainYorrAbility());
            }
        }
    }
}



namespace Abilities
{
    public class CaptainYorrAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            GenericShip.BeforeTokenIsAssignedGlobal += CaptainYorrPilotAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.BeforeTokenIsAssignedGlobal -= CaptainYorrPilotAbility;
        }

        private void CaptainYorrPilotAbility(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(Tokens.StressToken) && ship.Owner == HostShip.Owner && ship != HostShip && HostShip.TokenCount(typeof(Tokens.StressToken)) <= 2)
            {

                Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation(ship, HostShip);
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
            HostShip.AssignToken(TargetShip.TokenToAssign, delegate {
                TargetShip.TokenToAssign = null;
                TargetShip = null;
                DecisionSubPhase.ConfirmDecision();
            });
        }

    }
}
