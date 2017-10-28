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
            }

            public override void InitializePilot()
            {
                base.InitializePilot();

                GenericShip.BeforeTokenIsAssignedGlobal += CaptainYorrAbility;

                OnDestroyed += RemoveCaptainYorrAbility;
            }

            private void CaptainYorrAbility(GenericShip ship, System.Type tokenType)
            {
                if (tokenType == typeof(Tokens.StressToken) && ship.Owner == this.Owner && ship != this && this.TokenCount(typeof(Tokens.StressToken)) <= 2)
                {

                    Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation(ship, this);
                    if (positionInfo.Range <= 2)
                    {
                        Selection.ShipReferences.Add("CaptainYorrAbility_YorrsShip", this);
                        Selection.ShipReferences.Add("CaptainYorrAbility_TargetShip", ship);
                        Triggers.RegisterTrigger(
                            new Trigger()
                            {
                                Name = "Captain Yorr's ability",
                                TriggerType = TriggerTypes.OnBeforeTokenIsAssigned,
                                TriggerOwner = this.Owner.PlayerNo,
                                EventHandler = ShowDecision
                            }
                        );
                    }
                }
            }

            private void ShowDecision(object sender, EventArgs e)
            {
                Phases.StartTemporarySubPhase(
                    "Captain Yorr's ability",
                    typeof(SubPhases.CaptainYorrDecisionSubPhase),
                    Triggers.FinishTrigger
                );
            }

            private void RemoveCaptainYorrAbility(GenericShip ship)
            {
                GenericShip.BeforeTokenIsAssignedGlobal -= CaptainYorrAbility;
                OnDestroyed -= RemoveCaptainYorrAbility;
            }
        }
    }
}


namespace SubPhases
{
    public class CaptainYorrDecisionSubPhase : DecisionSubPhase
    {
        public override void Prepare()
        {
            infoText = "Use Captain Yorr's ability?";

            AddDecision("Yes", UseCaptainYorrAbility);
            AddDecision("No", DontUseCaptainYorrAbility);            

            defaultDecision = "No";
        }

        private void UseCaptainYorrAbility(object sender, System.EventArgs e)
        {
            var yorrsShip = Selection.ShipReferences["CaptainYorrAbility_YorrsShip"];
            var targetShip = Selection.ShipReferences["CaptainYorrAbility_TargetShip"];

            yorrsShip.AssignToken(targetShip.TokenToAssign, delegate {
                targetShip.TokenToAssign = null;
                ConfirmDecision();
            });
        }

        private void DontUseCaptainYorrAbility(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

        private void ConfirmDecision()
        {
            Selection.ShipReferences.Remove("CaptainYorrAbility_YorrsShip");
            Selection.ShipReferences.Remove("CaptainYorrAbility_TargetShip");
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }
    }
}
