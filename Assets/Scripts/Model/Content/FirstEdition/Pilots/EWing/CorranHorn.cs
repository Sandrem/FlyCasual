using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.EWing
    {
        public class CorranHorn : EWing
        {
            public CorranHorn() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Corran Horn",
                    8,
                    35,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.CorranHornAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                ModelInfo.SkinName = "Green";
            }
        }
    }
}

namespace Abilities
{
    public abstract class CorranHornBaseAbility : GenericAbility
    {
        protected TriggerTypes TriggerType;
        protected string Description;
        protected Func<GenericShip, IShipWeapon, bool, bool> ExtraAttackFilter;

        protected void RegisterCorranHornAbility()
        {
            if (!HostShip.Tokens.HasToken(typeof(WeaponsDisabledToken)))
            {
                RegisterAbilityTrigger(TriggerType, UseCorranHornAbility);
            }
        }

        protected void UseCorranHornAbility(object sender, System.EventArgs e)
        {
            Combat.StartAdditionalAttack(
                HostShip,
                AfterExtraAttackSubPhase,
                ExtraAttackFilter,
                HostShip.PilotName,
                Description,
                HostShip
            );
        }

        private void AfterExtraAttackSubPhase()
        {
            // "Weapons disabled" token is assigned only if attack was successfully performed
            if (HostShip.IsAttackPerformed) Phases.Events.OnRoundStart += RegisterAssignWeaponsDisabledTrigger;

            Triggers.FinishTrigger();
        }

        private void RegisterAssignWeaponsDisabledTrigger()
        {
            Phases.Events.OnRoundStart -= RegisterAssignWeaponsDisabledTrigger;
            RegisterAbilityTrigger(TriggerTypes.OnRoundStart, AssignWeaponsDisabledTrigger);
        }

        private void AssignWeaponsDisabledTrigger(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), Triggers.FinishTrigger);
        }
    }

    namespace FirstEdition
    {
        //At the start of the End phase, you may perform one attack. You cannot attack during the next round.
        public class CorranHornAbility : CorranHornBaseAbility
        {
            public CorranHornAbility()
            {
                TriggerType = TriggerTypes.OnEndPhaseStart;
                Description = "You may perform an additional attack.\nYou cannot attack during next round.";
            }

            public override void ActivateAbility()
            {
                Phases.Events.OnEndPhaseStart_Triggers += RegisterCorranHornAbility;
            }

            public override void DeactivateAbility()
            {
                Phases.Events.OnEndPhaseStart_Triggers -= RegisterCorranHornAbility;
            }
        }
    }
}