using System.Collections;
using UnityEngine;

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
            Phases.BeforeActionSubPhaseStart += CheckSkipPerformAction;
        }

        public void CheckSkipPerformAction()
        {
            if (Selection.ThisShip.ObstaclesHit.Count > 0)
            {
                Messages.ShowErrorToHuman("Hit asteroid during movement - action subphase is skipped");
                Selection.ThisShip.IsSkipsActionSubPhase = true;
            }
        }

        public void CheckDamage(Ship.GenericShip ship)
        {
            if (Selection.ThisShip.ObstaclesHit.Count > 0)
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
            Phases.StartTemporarySubPhase(
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
            dicesType = DiceKind.Attack;
            dicesCount = 1;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();
            Selection.ActiveShip = Selection.ThisShip;

            switch (CurrentDiceRoll.DiceList[0].Side)
            {
                case DiceSide.Blank:
                    NoDamage();
                    break;
                case DiceSide.Focus:
                    NoDamage();                    
                    break;
                case DiceSide.Success:
                    Messages.ShowErrorToHuman("Damage is dealt!");
                    SufferDamage();
                    Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, callBack);
                    break;
                case DiceSide.Crit:
                    Messages.ShowErrorToHuman("Critical damage is dealt!");
                    SufferDamage();
                    Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, callBack);
                    break;
                default:
                    break;
            }
        }

        private void NoDamage()
        {
            Messages.ShowInfoToHuman("No damage");
            callBack();
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
                    EventHandler = Selection.ActiveShip.SufferDamage
                });
            }
        }

    }

}
