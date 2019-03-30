using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.KihraxzFighter
    {
        public class CaptainJostero : KihraxzFighter
        {
            public CaptainJostero() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Captain Jostero",
                    3,
                    43,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaptainJosteroAbility),
                    seImageNumber: 194
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainJosteroAbility : GenericAbility
    {
        private bool performedRegularAttack;

        public override void ActivateAbility()
        {
            GenericShip.OnDamageInstanceResolvedGlobal += CheckJosteroAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnDamageInstanceResolvedGlobal -= CheckJosteroAbility;
        }

        private void CheckJosteroAbility(GenericShip damaged, DamageSourceEventArgs damage)
        {
            // Can we even bonus attack?
            if (!HostShip.CanBonusAttack)
                return;

            // Make sure the opposing ship is an enemy.
            if (damaged.Owner == HostShip.Owner)
                return;

            // If the ship is defending we're not interested.
            if (Combat.Defender == damaged || damage.DamageType == DamageTypes.ShipAttack)
                return;

            // Save the value for whether they've attacked or not.
            performedRegularAttack = HostShip.IsAttackPerformed;

            // It may be possible in the future for a non-defender to be damaged in combat so we've got to future proof here.
            if (Combat.AttackStep == CombatStep.None)
            {
                RegisterAbilityTrigger(TriggerTypes.OnDamageInstanceResolved, RegisterBonusAttack);
            }
            else
            {
                Combat.Attacker.OnCombatCheckExtraAttack += StartBonusAttack;
            }
        }

        private void StartBonusAttack(GenericShip ship)
        {
            ship.OnCombatCheckExtraAttack -= StartBonusAttack;
            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, RegisterBonusAttack);
        }

        private void RegisterBonusAttack(object sender, System.EventArgs e)
        {
            if (HostShip.IsDestroyed)
            {
                Triggers.FinishTrigger();
                return;
            }


            HostShip.StartBonusAttack(CleanupBonusAttack);
        }

        private void CleanupBonusAttack()
        {
            // Restore previous value of "is already attacked" flag
            HostShip.IsAttackPerformed = performedRegularAttack;
            Triggers.FinishTrigger();
        }
    }
}

