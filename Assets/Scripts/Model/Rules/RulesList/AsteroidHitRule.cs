using System.Collections;
using UnityEngine;
using Ship;

namespace RulesList
{
    public class AsteroidHitRule
    {

        public AsteroidHitRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GenericShip.OnMovementFinishGlobal += CheckDamage;
            Phases.BeforeActionSubPhaseStart += CheckSkipPerformAction;
        }

        private void CheckSkipPerformAction()
        {
            if (Selection.ThisShip.IsHitObstacles)
            {
                Messages.ShowErrorToHuman("Hit asteroid during movement - action subphase is skipped");
                Selection.ThisShip.IsSkipsActionSubPhase = true;
            }
        }

        private void CheckDamage(GenericShip ship)
        {
            if (Selection.ThisShip.IsHitObstacles)
            {
                foreach (var asteroid in Selection.ThisShip.ObstaclesHit)
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Roll for asteroid damage",
                        TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnShipMovementFinish,
                        EventHandler = RollForDamage
                    });
                }
            }
        }

        private void RollForDamage(object sender, System.EventArgs e)
        {
            Messages.ShowErrorToHuman("Hit asteroid during movement - rolling for damage");

            Selection.ActiveShip = Selection.ThisShip;
            Phases.StartTemporarySubPhaseOld(
                "Damage from asteroid collision",
                typeof(SubPhases.AsteroidHitCheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.AsteroidHitCheckSubPhase));
                    Triggers.FinishTrigger();
                });
        }
    }
}

namespace SubPhases
{

    public class AsteroidHitCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            diceType = DiceKind.Attack;
            diceCount = 1;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();
            Selection.ActiveShip = Selection.ThisShip;

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
                Selection.ActiveShip.AssignedDamageDiceroll.DiceList.Add(dice);

                Triggers.RegisterTrigger(new Trigger() {
                    Name = "Suffer asteroid damage",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = Selection.ActiveShip.Owner.PlayerNo,
                    EventHandler = Selection.ActiveShip.SufferDamage,
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
