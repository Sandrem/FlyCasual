using Obstacles;
using Ship;
using SubPhases;
using System;

namespace Obstacles
{
    public class Debris: GenericObstacle
    {
        public Debris(string name, string shortName) : base(name, shortName)
        {
            
        }

        public override string GetTypeName => "Debris";

        public override void OnHit(GenericShip ship)
        {
            Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " hit debris during movement, Stress token is assigned");
            ship.Tokens.AssignToken(
                typeof(Tokens.StressToken), 
                delegate { StartToRoll(ship); }
            );
        }

        private void StartToRoll(GenericShip ship)
        {
            Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " hit debris during movement, rolling for effect");

            DebrisHitCheckSubPhase newPhase = (DebrisHitCheckSubPhase)Phases.StartTemporarySubPhaseNew(
                "Damage from debris collision",
                typeof(DebrisHitCheckSubPhase),
                delegate
                {
                    Phases.FinishSubPhase(typeof(DebrisHitCheckSubPhase));
                    Triggers.FinishTrigger();
                });
            newPhase.TheShip = ship;
            newPhase.TheObstacle = this;
            newPhase.Start();
        }

        public override void OnShotObstructedExtra(GenericShip attacker, GenericShip defender)
        {
            // Only default effect
        }

        public override void AfterObstacleRoll(GenericShip ship, DieSide side, Action callback)
        {
            if (side == DieSide.Crit
                || (side == DieSide.Success && Editions.Edition.Current.RuleSet.GetType() == typeof(Editions.RuleSets.RuleSet25))
            )
            {
                DealDebrisDamage(ship, side, callback);
            }
            else
            {
                NoEffect(callback);
            }
        }

        private void DealDebrisDamage(GenericShip ship, DieSide side, Action callback)
        {
            Messages.ShowErrorToHuman($"{ship.PilotInfo.PilotName} suffered damage after damage roll");

            ship.Damage.TryResolveDamage(1, 0, new DamageSourceEventArgs() { DamageType = DamageTypes.ObstacleCollision, Source = this }, callback);
        }

        private void NoEffect(Action callback)
        {
            Messages.ShowInfoToHuman("No damage");
            callback();
        }
    }
}

namespace SubPhases
{

    public class DebrisHitCheckSubPhase : DiceRollCheckSubPhase
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


