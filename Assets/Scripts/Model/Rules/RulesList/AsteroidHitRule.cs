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
            Phases.Events.BeforeActionSubPhaseStart += CheckSkipPerformAction;
        }

        private void CheckSkipPerformAction()
        {
            if (Selection.ThisShip.IsHitObstacles)
            {
                Messages.ShowErrorToHuman("Hit asteroid during movement - action subphase is skipped");
                Selection.ThisShip.IsSkipsActionSubPhase = true;
            }
        }

        public void CheckDamage(GenericShip ship)
        {
            if (ship.IsHitObstacles)
            {
                foreach (var asteroid in ship.ObstaclesHit)
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Roll for asteroid damage",
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
                Messages.ShowErrorToHuman("Hit asteroid during movement - rolling for damage");

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
            diceType = DiceKind.Attack;
            diceCount = 1;

            finishAction = FinishAction;
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
                    Messages.ShowErrorToHuman("Damage is dealt!");
                    SufferDamage();
                    break;
                case DieSide.Crit:
                    Messages.ShowErrorToHuman("Critical damage is dealt!");
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
            foreach (var dice in CurrentDiceRoll.DiceList)
            {
                TheShip.AssignedDamageDiceroll.DiceList.Add(dice);

                Triggers.RegisterTrigger(new Trigger() {
                    Name = "Suffer asteroid damage",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = TheShip.Owner.PlayerNo,
                    EventHandler = TheShip.SufferDamage,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = "Asteroid",
                        DamageType = DamageTypes.ObstacleCollision
                    }
                });
            }

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, CallBack);
        }

    }

}
