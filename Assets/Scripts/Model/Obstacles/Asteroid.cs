using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Players;
using Ship;
using SubPhases;
using UnityEngine;

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
            if (Selection.ThisShip.IgnoreObstacleTypes.Contains(typeof(Asteroid))) {
                return;
            }

            if (!Selection.ThisShip.CanPerformActionsWhenOverlapping)
            {
                Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " hit an asteroid during movement, their action subphase is skipped");
                Selection.ThisShip.IsSkipsActionSubPhase = true;
            }

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
            newPhase.Start();
        }

        public override void OnLanded(GenericShip ship)
        {
            ship.OnTryPerformAttack += DenyAttack;
        }

        public void DenyAttack(ref bool result, List<string> stringList)
        {
            if (Selection.ThisShip.ObstaclesLanded.Contains(this) && !Selection.ThisShip.CanAttackWhileLandedOnObstacle())
            {
                stringList.Add(Selection.ThisShip.PilotInfo.PilotName + " landed on an asteroid and cannot attack");
                result = false;
            }
        }

        public override void OnShotObstructedExtra(GenericShip attacker, GenericShip defender)
        {
            // Only default effect
        }

    }
}

namespace SubPhases
{

    public class AsteroidHitCheckSubPhase : DiceRollCheckSubPhase
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

            switch (CurrentDiceRoll.DiceList[0].Side)
            {
                case DieSide.Blank:
                    NoDamage();
                    break;
                case DieSide.Focus:
                    NoDamage();
                    break;
                case DieSide.Success:
                    Messages.ShowErrorToHuman("The ship takes a hit!");
                    SufferDamage();
                    break;
                case DieSide.Crit:
                    Messages.ShowErrorToHuman("The ship takes a critical hit!");
                    SufferDamage();
                    break;
                default:
                    break;
            }
        }

        private void NoDamage()
        {
            Messages.ShowInfoToHuman("No damage");
            CallBack();
        }

        private void SufferDamage()
        {
            DamageSourceEventArgs asteroidDamage = new DamageSourceEventArgs()
            {
                Source = "Asteroid",
                DamageType = DamageTypes.ObstacleCollision
            };

            TheShip.Damage.TryResolveDamage(CurrentDiceRoll.DiceList, asteroidDamage, CallBack);
        }
    }
}
