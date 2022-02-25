using Obstacles;
using Ship;
using SubPhases;
using System;
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
            if (Editions.Edition.Current.RuleSet.GetType() == typeof(Editions.RuleSets.RuleSet25))
            {
                BreakAllLocks(ship, ()=> StartToRoll(ship));
            }
            else
            {
                StartToRoll(ship);
            }
        }

        private void BreakAllLocks(GenericShip ship, Action callback)
        {
            ship.Tokens.RemoveAllTokensByType(typeof(Tokens.BlueTargetLockToken), () => GetStrain(ship, callback));
        }

        private void GetStrain(GenericShip ship, Action callback)
        {
            Messages.ShowErrorToHuman($"{ship.PilotInfo.PilotName} hit a gas cloud during movement and gained a Strain token");
            ship.Tokens.AssignToken(typeof(StrainToken), callback);
        }

        private void StartToRoll(GenericShip ship)
        {
            Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " hit a gas cloud during movement, rolling for effect");

            GasCloudHitCheckSubPhase newPhase = (GasCloudHitCheckSubPhase)Phases.StartTemporarySubPhaseNew(
                "Strain from gas cloud collision",
                typeof(GasCloudHitCheckSubPhase),
                delegate
                {
                    Phases.FinishSubPhase(typeof(GasCloudHitCheckSubPhase));
                    Triggers.FinishTrigger();
                });
            newPhase.TheShip = ship;
            newPhase.TheObstacle = this;
            newPhase.Start();
        }

        public override void AfterObstacleRoll(GenericShip ship, DieSide side, Action callback)
        {
            if (side == DieSide.Crit)
            {
                Messages.ShowErrorToHuman($"{ship.PilotInfo.PilotName} gains 3 Ion tokens");
                ship.Tokens.AssignTokens(() => CreateIonToken(ship), 3, callback);
            }
            else if (side == DieSide.Success)
            {
                Messages.ShowErrorToHuman($"{ship.PilotInfo.PilotName} gains 1 Ion token");
                ship.Tokens.AssignToken(typeof(Tokens.IonToken), callback);
            }
            else
            {
                Messages.ShowInfoToHuman("No effect");
                callback();
            }
        }

        private GenericToken CreateIonToken(GenericShip ship)
        {
            return new IonToken(ship);
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
        public GenericObstacle TheObstacle { get; set; }

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

            TheObstacle.AfterObstacleRoll(TheShip, CurrentDiceRoll.DiceList[0].Side, CallBack);
        }
    }
}
