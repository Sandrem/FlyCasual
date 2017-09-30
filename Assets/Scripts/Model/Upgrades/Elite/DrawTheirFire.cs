using Upgrade;
using UnityEngine;
using Ship;

namespace UpgradesList
{
    public class DrawTheirFire : GenericUpgrade
    {
        public DrawTheirFire() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Draw Their Fire";
            Cost = 1;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            GenericShip.OnAttackHitAsDefenderGlobal += CheckDrawTheirFireAbility;
            Host.OnDestroyed += RemoveDrawTheirFireAbility;
        }

        private void CheckDrawTheirFireAbility()
        {
            if (Combat.Defender.Owner.PlayerNo == Host.Owner.PlayerNo && Combat.Defender.ShipId != Host.ShipId)
            {
                Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Combat.Defender, Host);
                if (distanceInfo.Range == 1 && Combat.DiceRollAttack.CriticalSuccesses > 0)
                {
                    RegisterDrawTheirFireAbility();
                }
            }
        }

        private void RegisterDrawTheirFireAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Draw Their Fire",
                TriggerType = TriggerTypes.OnAttackHit,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = UseDrawTheirFireAbility
            });
        }

        private void UseDrawTheirFireAbility(object sender, System.EventArgs e)
        {
            if (Combat.DiceRollAttack.CriticalSuccesses > 0)
            {
                Selection.ActiveShip = Host;
                Phases.StartTemporarySubPhase(
                    "Draw Their Fire",
                    typeof(SubPhases.DrawTheirFireDecisionSubPhase),
                    Triggers.FinishTrigger
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void RemoveDrawTheirFireAbility(GenericShip ship)
        {
            GenericShip.OnAttackHitAsDefenderGlobal -= CheckDrawTheirFireAbility;
        }
    }
}

namespace SubPhases
{

    public class DrawTheirFireDecisionSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "Use ability of Draw Their Fire?";

            AddDecision("Yes", UseAbility);
            AddDecision("No", DontUseAbility);

            defaultDecision = "Yes";
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            Die criticalHitDice = Combat.DiceRollAttack.DiceList.Find(n => n.Side == DieSide.Crit);

            Combat.DiceRollAttack.DiceList.Remove(criticalHitDice);
            Selection.ActiveShip.AssignedDamageDiceroll.DiceList.Add(criticalHitDice);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage from Draw Their Fire",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Selection.ActiveShip.Owner.PlayerNo,
                EventHandler = Selection.ActiveShip.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = "Draw Their Fire",
                    DamageType = DamageTypes.CardAbility
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, ConfirmDecision);
        }

        private void DontUseAbility(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

        private void ConfirmDecision()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
