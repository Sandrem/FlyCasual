using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using SubPhases;
using System;

namespace UpgradesList
{
    public class Selflessness : GenericUpgrade
    {
        public Selflessness() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Selflessness";
            Cost = 1;

            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            bool result = true;

            if (ship.ShipBaseSize != BaseSize.Small) result = false;
            else if (ship.faction != Faction.Rebels) result = false;

            return result;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            GenericShip.OnTryDamagePreventionGlobal += CheckSelflessnessAbility;
            Host.OnDestroyed += RemoveSelflessnessAbility;
        }

        private void CheckSelflessnessAbility()
        {
            if (Combat.Defender.Owner.PlayerNo == Host.Owner.PlayerNo && Combat.Defender.ShipId != Host.ShipId)
            {
                Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Combat.Defender, Host);
                if (distanceInfo.Range == 1 && Combat.DiceRollAttack.RegularSuccesses > 0)
                {
                    RegisterDrawTheirFireAbility();
                }
            }
        }

        private void RegisterDrawTheirFireAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Selflessness",
                TriggerType = TriggerTypes.OnTryDamagePrevention,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = UseSelflessnessAbility
            });
        }

        private void UseSelflessnessAbility(object sender, System.EventArgs e)
        {
            if (Combat.DiceRollAttack.RegularSuccesses > 0)
            {
                SelflessnessDecisionSubPhase decisionSubphase = (SelflessnessDecisionSubPhase) Phases.StartTemporarySubPhaseNew(
                    "Selflessness",
                    typeof(SelflessnessDecisionSubPhase),
                    Triggers.FinishTrigger
                );
                decisionSubphase.SelflessnessUpgrade = this;
                decisionSubphase.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        public override void Discard(Action callBack)
        {
            RemoveSelflessnessAbility(this.Host);

            base.Discard(callBack);
        }

        private void RemoveSelflessnessAbility(GenericShip ship)
        {
            GenericShip.OnTryDamagePreventionGlobal -= CheckSelflessnessAbility;
        }
    }
}

namespace SubPhases
{

    public class SelflessnessDecisionSubPhase : DecisionSubPhase
    {
        public UpgradesList.Selflessness SelflessnessUpgrade;

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Use ability of Selflessness?";

            AddDecision("Yes", UseAbility);
            AddDecision("No", DontUseAbility);

            DefaultDecision = "Yes";

            callBack();
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            DiscardUpgrade(SufferRegularHits);
        }

        private void DiscardUpgrade(System.Action callBack)
        {
            SelflessnessUpgrade.Discard(callBack);
        }

        private void SufferRegularHits()
        {
            List<Die> regularHitsDice = new List<Die>();

            List<Die> diceRollAttackCopy = new List<Die>(Combat.DiceRollAttack.DiceList);

            foreach (var die in diceRollAttackCopy)
            {
                if (die.Side == DieSide.Success)
                {
                    regularHitsDice.Add(die);
                    Combat.DiceRollAttack.DiceList.Remove(die);
                }
            }

            SelflessnessUpgrade.Host.AssignedDamageDiceroll.DiceList.AddRange(regularHitsDice);

            foreach (var die in SelflessnessUpgrade.Host.AssignedDamageDiceroll.DiceList)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Suffer damage from Selflessness",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = SelflessnessUpgrade.Host.Owner.PlayerNo,
                    EventHandler = SelflessnessUpgrade.Host.SufferDamage,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = "Selflessness",
                        DamageType = DamageTypes.CardAbility
                    },
                    Skippable = true
                });
            }

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, ConfirmDecision);
        }

        private void DontUseAbility(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

    }

}
