using System.Linq;

namespace Ship
{
    namespace LambdaShuttle
    {
        public class ColonelJendon : LambdaShuttle
        {
            public ColonelJendon() : base()
            {
                PilotName = "Colonel Jendon";
                PilotSkill = 6;
                Cost = 26;
                IsUnique = true;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();

                Phases.OnCombatPhaseStart += RegisterColonelJendonAbility;
            }

            private void RegisterColonelJendonAbility()
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Colonel Jendon' ability",
                    TriggerOwner = this.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnCombatPhaseStart,
                    EventHandler = StartSubphaseForColonelJendonAbility
                });
            }


            private void StartSubphaseForColonelJendonAbility(object sender, System.EventArgs e)
            {
                Selection.ThisShip = this;
                if (Owner.Ships.Count > 1 && this.HasToken(typeof(Tokens.BlueTargetLockToken), '*'))
                {
                    Phases.StartTemporarySubPhase(
                        "Colonel Jendon's ability",
                        typeof(SubPhases.ColonelJendonDecisionSubPhase),
                        delegate {
                            Phases.CurrentSubPhase.Resume();
                            Triggers.FinishTrigger();
                        }
                    );
                }
                else
                {
                    Triggers.FinishTrigger();
                }
            }
        }
    }
}


namespace SubPhases
{
    public class ColonelJendonDecisionSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "Use Colonel Jendon's ability?";

            var blueTargetLocks = Selection.ThisShip.GetAssignedTokens()
                .Where(t => t is Tokens.BlueTargetLockToken)
                .Select(x => (Tokens.BlueTargetLockToken)x).ToList();

            AddDecision("No", DontUseColonelJendonAbility);

            blueTargetLocks.ForEach(l => {
                var name = "Target Lock " + l.Letter;
                AddDecision(name, delegate { UseColonelJendonAbility(l.Letter); });
                AddTooltip(name, l.OtherTokenOwner.ImageUrl);
            });
            
            defaultDecision = "No";
        }

        private void UseColonelJendonAbility(char letter)
        {
            Phases.StartTemporarySubPhase(
                "Select target for Colonel Jendon's ability",
                typeof(ColonelJendonAbilityTargetSubPhase),
                ConfirmDecision
            );
        }

        private void DontUseColonelJendonAbility(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

        private void ConfirmDecision()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

    public class ColonelJendonAbilityTargetSubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            isFriendlyAllowed = true;
            maxRange = 1;
            finishAction = SelectColonelJendonAbilityTarget;

            UI.ShowSkipButton();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            if (ship.HasToken(typeof(Tokens.BlueTargetLockToken), '*'))
            {
                Messages.ShowErrorToHuman("Only ships without blue target lock tokens can be selected");
                RevertSubPhase();
                return false;
            }
            else
            {
                return base.ThisShipCanBeSelected(ship);
            }
        }

        private void SelectColonelJendonAbilityTarget()
        {
            MovementTemplates.ReturnRangeRuler();

            var token = Selection.ThisShip.GetToken(typeof(Tokens.BlueTargetLockToken), '*') as Tokens.BlueTargetLockToken;
            
            Selection.ThisShip.ReassignTargetLockToken(typeof(Tokens.BlueTargetLockToken), token.Letter, TargetShip, 
                delegate {
                    Phases.FinishSubPhase(typeof(ColonelJendonAbilityTargetSubPhase));
                    CallBack();
                });
        }

        protected override void RevertSubPhase() { }

        public override void SkipButton()
        {
            Phases.FinishSubPhase(typeof(ColonelJendonAbilityTargetSubPhase));
            CallBack();
        }

    }

}

