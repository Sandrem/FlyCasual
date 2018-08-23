using RuleSets;
using Ship;

namespace Ship
{
    namespace Kihraxz
    {
        public class CaptainJostero : Kihraxz, ISecondEditionPilot
        {
            public CaptainJostero()
            {
                PilotName = "Captain Jostero";
                PilotSkill = 3;
                Cost = 43;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new Abilities.SecondEdition.CaptainJosteroAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
                // nope
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainJosteroAbilitySE : GenericAbility
    {
        private bool performedRegularAttack;

        public override void ActivateAbility()
        {
            GenericShip.OnDamageInstanceResolvedGlobal += CheckJosteroAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnDamageInstanceResolvedGlobal += CheckJosteroAbility;
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
