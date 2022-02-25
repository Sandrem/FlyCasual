using System;
using System.Collections.Generic;
using System.Linq;
using Obstacles;
using Ship;
using SubPhases;

namespace Obstacles
{
    public class Asteroid : GenericObstacle
    {
        public Asteroid(string name, string shortName) : base(name, shortName)
        {

        }

        public override string GetTypeName => "Asteroid";

        public override void OnHit(GenericShip ship)
        {
            if (Selection.ThisShip.IgnoreObstacleTypes.Contains(typeof(Asteroid))) return;

            Messages.ShowErrorToHuman($"{ship.PilotInfo.PilotName} hit an asteroid during movement and suffered damage");
            DealAutoAsteroidDamage(ship, () => StartToRoll(ship));
        }

        private void StartToRoll(GenericShip ship)
        {
            Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " hit an asteroid during movement, rolling for damage");

            AsteroidHitCheckSubPhase newPhase = (AsteroidHitCheckSubPhase)Phases.StartTemporarySubPhaseNew(
                "Damage from asteroid collision",
                typeof(AsteroidHitCheckSubPhase),
                delegate
                {
                    Phases.FinishSubPhase(typeof(AsteroidHitCheckSubPhase));
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

        private void DealAutoAsteroidDamage(GenericShip ship, Action callback)
        {
            ship.Damage.TryResolveDamage(1, new DamageSourceEventArgs() { DamageType = DamageTypes.ObstacleCollision, Source = this }, callback);
        }

        public override void AfterObstacleRoll(GenericShip ship, DieSide side, Action callback)
        {
            if (side == DieSide.Crit || side == DieSide.Success)
            {
                DealAsteroidDamage(ship, side, callback);
            }
            else
            {
                NoEffect(callback);
            }
        }

        private void DealAsteroidDamage(GenericShip ship, DieSide side, Action callback)
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

    public class AsteroidHitCheckSubPhase : DiceRollCheckSubPhase
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
