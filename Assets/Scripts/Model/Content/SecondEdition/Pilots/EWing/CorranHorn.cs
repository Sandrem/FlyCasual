using BoardTools;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace SecondEdition.EWing
    {
        public class CorranHorn : EWing
        {
            public CorranHorn() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Corran Horn",
                    5,
                    74,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.CorranHornAbility)
                );

                ModelInfo.SkinName = "Green";

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 50;
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

    namespace SecondEdition
    {
        //At initiative 0, you may perform a bonus primary attack against an enemy ship in your bullseye firing arc. 
        //If you do, at the start of the next Planning Phase, gain 1 disarm token.
        public class CorranHornAbility : CorranHornBaseAbility
        {
            public CorranHornAbility()
            {
                TriggerType = TriggerTypes.OnCombatPhaseEnd;
                Description = "You may perform a bonus bullseye primary attack\nGain 1 disarm token next round";
                ExtraAttackFilter = IsBullsEyePrimary;
            }

            public override void ActivateAbility()
            {
                //This is technically not the correct timing, but works for now. The combat phase should be rewritten to allow 
                //for abilities to add extra activations 
                Phases.Events.OnCombatPhaseEnd_Triggers += RegisterCorranHornAbility;
            }

            public override void DeactivateAbility()
            {
                Phases.Events.OnCombatPhaseEnd_Triggers -= RegisterCorranHornAbility;
            }

            private bool IsBullsEyePrimary(GenericShip defender, IShipWeapon weapon, bool isSilent)
            {
                bool result = false;
                if (weapon is PrimaryWeaponClass && new ShotInfo(HostShip, defender, weapon).InArcByType(Arcs.ArcTypes.Bullseye))
                {
                    result = true;
                }
                else
                {
                    if (!isSilent) Messages.ShowError("Attack must be performed with primary weapon in bullseye arc");
                }
                return result;
            }
        }
    }
}
