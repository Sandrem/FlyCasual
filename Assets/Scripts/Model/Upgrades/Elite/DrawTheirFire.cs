using Upgrade;
using UnityEngine;
using Ship;
using SubPhases;
using Abilities;
using System;

namespace UpgradesList
{
    public class DrawTheirFire : GenericUpgrade
    {
        public DrawTheirFire() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Draw Their Fire";
            Cost = 1;

            UpgradeAbilities.Add(new DrawTheirFireAbility());
        }
    }
}

namespace Abilities
{
    public class DrawTheirFireAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal += CheckDrawTheirFireAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal -= CheckDrawTheirFireAbility;
        }

        private void CheckDrawTheirFireAbility()
        {
            if (AbilityCanBeUsed())
            {
                RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, UseDrawTheirFireAbility);
            }
        }

        protected virtual bool AbilityCanBeUsed()
        {
            if (Combat.Defender.Owner.PlayerNo == HostShip.Owner.PlayerNo && Combat.Defender.ShipId != HostShip.ShipId)
            {
                BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(Combat.Defender, HostShip);
                if (distanceInfo.Range == 1 && Combat.DiceRollAttack.CriticalSuccesses > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void UseDrawTheirFireAbility(object sender, System.EventArgs e)
        {
            if (Combat.DiceRollAttack.CriticalSuccesses > 0)
            {
                AskToUseAbility(AlwaysUseByDefault, UseAbility, null, null, true);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            Die criticalHitDice = Combat.DiceRollAttack.DiceList.Find(n => n.Side == DieSide.Crit);

            Combat.DiceRollAttack.DiceList.Remove(criticalHitDice);
            HostShip.AssignedDamageDiceroll.DiceList.Add(criticalHitDice);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage from Draw Their Fire",
                TriggerType = TriggerTypes.OnTryDamagePrevention,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = HostShip.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = "Draw Their Fire",
                    DamageType = DamageTypes.CardAbility
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, DecisionSubPhase.ConfirmDecision);
        }

    }
}
