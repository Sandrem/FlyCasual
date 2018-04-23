using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using System;
using Ship;

namespace Ship
{
    namespace JumpMaster5000
    {
        public class Dengar : JumpMaster5000
        {
            public Dengar() : base()
            {
                PilotName = "Dengar";
                PilotSkill = 9;
                Cost = 33;

                IsUnique = true;

                // Already have Elite icon from JumpMaster5000 class

                PilotAbilities.Add(new DengarPilotAbility());
            }
        }
    }
}

namespace Abilities
{
    public class DengarPilotAbility : GenericAbility
    {
        private GenericShip shipToPunish;
        private bool isPerformedRegularAttack;

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsDefender += CheckAbility;
            Phases.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsDefender -= CheckAbility;
            Phases.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (IsAbilityUsed) return;

            if (HostShip.IsCannotAttackSecondTime) return;

            Board.ShipShotDistanceInformation counterAttackInfo = new Board.ShipShotDistanceInformation(Combat.Defender, Combat.Attacker);
            if (!counterAttackInfo.InArc) return;

            // Save his attacker, becuase combat data will be cleared
            shipToPunish = Combat.Attacker;

            Combat.Attacker.OnCombatCheckExtraAttack += RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            ship.OnCombatCheckExtraAttack -= RegisterAbility;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, DoCounterAttack);
        }

        private void DoCounterAttack(object sender, EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                // Temporary fix
                if (HostShip.IsDestroyed)
                {
                    Triggers.FinishTrigger();
                    return;
                }

                // Save his "is already attacked" flag
                isPerformedRegularAttack = HostShip.IsAttackPerformed;

                // Plan to set IsAbilityUsed only after attack that was successfully started
                HostShip.OnAttackFinishAsAttacker += SetIsAbilityIsUsed;

                Combat.StartAdditionalAttack(
                    HostShip,
                    FinishExtraAttack,
                    CounterAttackFilter,
                    HostShip.PilotName,
                    "You may perform an additional attack against " + shipToPunish.PilotName + ".",
                    HostShip.ImageUrl
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack one more time", HostShip.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void FinishExtraAttack()
        {
            // Restore previous value of "is already attacked" flag
            HostShip.IsAttackPerformed = isPerformedRegularAttack;

            // Set IsAbilityUsed only after attack that was successfully started
            HostShip.OnAttackFinishAsAttacker -= SetIsAbilityIsUsed;

            Triggers.FinishTrigger();
        }

        private bool CounterAttackFilter(GenericShip targetShip, IShipWeapon weapon)
        {
            bool result = true;

            if (targetShip != shipToPunish)
            {
                Messages.ShowErrorToHuman(string.Format("{0} can attack only {1}", HostShip.PilotName, shipToPunish.PilotName));
                result = false;
            }

            return result;
        }

    }
}