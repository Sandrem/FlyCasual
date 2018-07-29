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

            // Is this ship the defender in combat?
            if (Combat.Defender == curToDamage)
                return;

            // Is the damage type a ship attack?
            if (curDamageInfo.DamageType != DamageTypes.ShipAttack)
                return;

            // Is the defender on our team and not us? If not return.
            if (Combat.Defender.Owner.PlayerNo != HostShip.Owner.PlayerNo || Combat.Defender.ShipId == HostShip.ShipId)
                return;

            // Is the defender at range 1 and is there a crit result?
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(Combat.Defender, HostShip);
            if (distanceInfo.Range > 1 || Combat.DiceRollAttack.CriticalSuccesses < 1)
                return;

            RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, UseDrawTheirFireAbility);
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
            // Find a crit and remove it from the ship we're protecting's assigned damage.
            Die criticalHitDice = Combat.DiceRollAttack.DiceList.Find(n => n.Side == DieSide.Crit);
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
