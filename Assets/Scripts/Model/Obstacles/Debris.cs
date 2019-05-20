using Players;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
                delegate { RollForDamage(ship); }
            );
        }

        private void RollForDamage(GenericShip ship)
        {
            Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " hit debris during movement, rolling for critical damage");

            DebrisHitCheckSubPhase newPhase = (DebrisHitCheckSubPhase)Phases.StartTemporarySubPhaseNew(
                "Damage from asteroid collision",
                typeof(DebrisHitCheckSubPhase),
                delegate
                {
                    Phases.FinishSubPhase(typeof(DebrisHitCheckSubPhase));
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
            // Only default effect
        }
    }
}

namespace SubPhases
{

    public class DebrisHitCheckSubPhase : DiceRollCheckSubPhase
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

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Crit)
            {
                Messages.ShowErrorToHuman("The ship takes a critical hit!");
                SufferDamage();
            }
            else
            {
                NoDamage();
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


