using Upgrade;
using System.Collections.Generic;
using Ship;
using BoardTools;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class Selfless : GenericUpgrade
    {
        public Selfless() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Selfless",
                UpgradeType.Talent,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.SelflessAbility),
                restriction: new FactionRestriction(Faction.Rebel),
                seImageNumber: 15
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class SelflessAbility : Abilities.FirstEdition.DrawTheirFireAbility
    {
        protected override bool AbilityCanBeUsed()
        {
            bool result = base.AbilityCanBeUsed();

            if (result)
            {
                ShotInfo shotInfo = new ShotInfo(Combat.Attacker, HostShip, Combat.Attacker.PrimaryWeapons);
                if (!shotInfo.InArc) result = false;
            }

            return result;
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
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    AlwaysUseByDefault,
                    UseAbility,
                    descriptionLong: "Do you want to suffer 1 of the uncanceled critical results instead of the target ship?",
                    imageHolder: HostUpgrade,
                    showAlwaysUseOption: true
                );
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