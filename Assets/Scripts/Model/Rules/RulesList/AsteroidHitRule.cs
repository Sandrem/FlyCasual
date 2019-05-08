using System.Collections;
using UnityEngine;
using Ship;
using SubPhases;

namespace RulesList
{
    public class AsteroidHitRule
    {
        static bool RuleIsInitialized = false;

        public AsteroidHitRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (!RuleIsInitialized)
            {
                GenericShip.OnMovementFinishGlobal += CheckDamage;
                RuleIsInitialized = true;
            }
        }

        public void CheckSkipPerformAction()
        {
            if (Selection.ThisShip.IsHitObstacles)
            {
                Messages.ShowErrorToHuman(Selection.ThisShip.PilotInfo.PilotName + " hit an asteroid during movement, their action subphase is skipped");
                Selection.ThisShip.IsSkipsActionSubPhase = true;
            }
        }

        public void CheckDamage(GenericShip ship)
        {
            if (ship.IsHitObstacles)
            {
                foreach (var asteroid in ship.ObstaclesHit)
                {
                    if (ship.IgnoreObstaclesList.Contains(asteroid)) continue;

                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Roll for asteroid damage.",
                        TriggerOwner = ship.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnMovementFinish,
                        EventHandler = RollForDamage(ship)
                    });
                }
            }
        }

        private System.EventHandler RollForDamage(GenericShip ship)
        {
            return delegate {
                Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " hit an asteroid during movement, rolling for damage");

                AsteroidHitCheckSubPhase newPhase = (AsteroidHitCheckSubPhase) Phases.StartTemporarySubPhaseNew(
                    "Damage from asteroid collision",
                    typeof(AsteroidHitCheckSubPhase),
                    delegate {
                        Phases.FinishSubPhase(typeof(AsteroidHitCheckSubPhase));
                        Triggers.FinishTrigger();
                    });
                newPhase.TheShip = ship;
                newPhase.Start();
            };
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
