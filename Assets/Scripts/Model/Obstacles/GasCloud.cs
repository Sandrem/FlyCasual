using Obstacles;
using Ship;
using SubPhases;
using Tokens;

namespace Obstacles
{
    public class GasCloud : GenericObstacle
    {
        public GasCloud(string name, string shortName) : base(name, shortName)
        {
            
        }

        public override string GetTypeName => "Gas Cloud";

        public override void OnHit(GenericShip ship)
        {
            if (!Selection.ThisShip.CanPerformActionsWhenOverlapping)
            {
                Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " hit a gas cloud during movement, their action subphase is skipped");
                Selection.ThisShip.IsSkipsActionSubPhase = true;
            }

            Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " hit a gas cloud during movement, rolling for strain");

            GasCloudHitCheckSubPhase newPhase = (GasCloudHitCheckSubPhase)Phases.StartTemporarySubPhaseNew(
                "Strain from gas cloud collision",
                typeof(GasCloudHitCheckSubPhase),
                delegate
                {
                    Phases.FinishSubPhase(typeof(GasCloudHitCheckSubPhase));
                    Triggers.FinishTrigger();
                });
            newPhase.TheShip = ship;
            newPhase.Start();
        }

        public override void OnLanded(GenericShip ship)
        {
            // Nothing
        }

        public override void OnShotObstructedExtra(GenericShip attacker, GenericShip defender)
        {
            defender.OnGenerateDiceModifications += TryToAddDiceModification;
        }

        private void TryToAddDiceModification(GenericShip ship)
        {
            ActionsList.GasCloudDiceModification newAction = new ActionsList.GasCloudDiceModification()
            {
                HostShip = ship,
                HostObstacle = this
            };
            ship.AddAvailableDiceModificationOwn(newAction);
        }
    }
}

namespace ActionsList
{
    public class GasCloudDiceModification : GenericAction
    {
        public GenericObstacle HostObstacle;

        public GasCloudDiceModification()
        {
            Name = DiceModificationName = "Gas Cloud";
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);
            callBack();
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence
                && Combat.CurrentDiceRoll.Blanks > 0
                && Combat.ShotInfo.ObstructedByObstacles.Contains(HostObstacle)
            )
            {
                result = true;
            }

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                int attackSuccessesCancelable = Combat.DiceRollAttack.SuccessesCancelable;
                int defenceSuccesses = Combat.CurrentDiceRoll.Successes;
                if (attackSuccessesCancelable > defenceSuccesses)
                {
                    return 100;
                }
            }

            return result;
        }

    }
}


namespace SubPhases
{

    public class GasCloudHitCheckSubPhase : DiceRollCheckSubPhase
    {
        private GenericShip prevActiveShip = Selection.ActiveShip;

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 1;

            AfterRoll = FinishAction;
            Selection.ActiveShip = TheShip;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();
            Selection.ActiveShip = prevActiveShip;

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Focus || CurrentDiceRoll.DiceList[0].Side == DieSide.Success)
            {
                Messages.ShowErrorToHuman("The ship gains a strain token!");
                TheShip.Tokens.AssignToken(typeof(StrainToken), CallBack);
            }
            else
            {
                Messages.ShowInfoToHuman("No strain");
                CallBack();
            }
        }
    }
}
