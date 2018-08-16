using System;

namespace DamageDeckCardFE
{

    public class MajorExplosion : GenericDamageCard
    {
        public MajorExplosion()
        {
            Name = "Major Explosion";
            Type = CriticalCardType.Ship;
            AiAvoids = true;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Selection.ActiveShip = Host;

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Roll for asteroid damage",
                TriggerOwner = Selection.ActiveShip.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnMajorExplosionCrit,
                EventHandler = RollForDamage
            });

            Triggers.ResolveTriggers(TriggerTypes.OnMajorExplosionCrit, delegate { Triggers.FinishTrigger(); });
        }

        private void RollForDamage(object sender, EventArgs e)
        {
            var phase = Phases.StartTemporarySubPhaseNew<SubPhases.MajorExplosionCheckSubPhase>(
                "Major Explosion",
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.MajorExplosionCheckSubPhase));
                    Triggers.FinishTrigger();
                });
            phase.DamageCard = this;
        }

    }

}

namespace SubPhases
{

    public class MajorExplosionCheckSubPhase : DiceRollCheckSubPhase
    {
        public GenericDamageCard DamageCard;

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 1;

            AfterRoll = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Success)
            {
                DealDamage();
            }
            else
            {
                NoDamage();
            }
        }

        private void DealDamage()
        {
            Messages.ShowInfo("Major Explosion: Suffer 1 additional critical damage");

            DamageSourceEventArgs majorexplosionDamage = new DamageSourceEventArgs()
            {
                Source = DamageCard,
                SourceDescription = "Critical hit card",
                DamageType = DamageTypes.CriticalHitCard
            };

            Selection.ActiveShip.Damage.TryResolveDamage(0, 1, majorexplosionDamage, CallBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfo("No damage");
            CallBack();
        }
    }

}