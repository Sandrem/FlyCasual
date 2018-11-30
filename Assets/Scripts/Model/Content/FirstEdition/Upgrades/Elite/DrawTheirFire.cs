using Ship;
using Upgrade;
using System.Collections.Generic;
using System;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class DrawTheirFire : GenericUpgrade
    {
        public DrawTheirFire() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Draw Their Fire",
                UpgradeType.Elite,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.DrawTheirFireAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class DrawTheirFireAbility : GenericAbility
    {
        private GenericShip curToDamage;
        private DamageSourceEventArgs curDamageInfo;

        public override void ActivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal += CheckDrawTheirFireAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal -= CheckDrawTheirFireAbility;
        }

        private void CheckDrawTheirFireAbility(GenericShip ship, DamageSourceEventArgs e)
        {
            curToDamage = ship;
            curDamageInfo = e;

            if (AbilityCanBeUsed())
                RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, UseDrawTheirFireAbility);
        }

        protected virtual bool AbilityCanBeUsed()
        {
            // Is the damage type a ship attack?
            if (curDamageInfo.DamageType != DamageTypes.ShipAttack)
                return false;

            // Is the defender on our team and not us? If not return.
            if (curToDamage.Owner.PlayerNo != HostShip.Owner.PlayerNo || curToDamage.ShipId == HostShip.ShipId)
                return false;

            // Is the defender at range 1 and is there a crit result?
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(curToDamage, HostShip);
            if (distanceInfo.Range > 1 || curToDamage.AssignedDamageDiceroll.CriticalSuccesses < 1)
                return false;

            return true;
        }

        private void UseDrawTheirFireAbility(object sender, System.EventArgs e)
        {
            if (curToDamage.AssignedDamageDiceroll.CriticalSuccesses > 0)
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
            // Find a crit and remove it from the ship we're protecting's assigned damage.
            Die criticalHitDice = curToDamage.AssignedDamageDiceroll.DiceList.Find(n => n.Side == DieSide.Crit);
            curToDamage.AssignedDamageDiceroll.DiceList.Remove(criticalHitDice);

            DamageSourceEventArgs drawtheirfireDamage = new DamageSourceEventArgs()
            {
                Source = "Draw Their Fire",
                DamageType = DamageTypes.CardAbility
            };

            HostShip.Damage.TryResolveDamage(0, 1, drawtheirfireDamage, DecisionSubPhase.ConfirmDecision);
        }

    }
}