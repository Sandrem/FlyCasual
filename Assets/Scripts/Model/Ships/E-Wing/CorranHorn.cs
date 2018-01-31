﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;

namespace Ship
{
    namespace EWing
    {
        public class CorranHorn : EWing
        {
            public CorranHorn() : base()
            {
                PilotName = "Corran Horn";
                PilotSkill = 8;
                Cost = 35;

                IsUnique = true;

                SkinName = "Green";

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new CorranHornAbility());
            }
        }
    }
}

namespace Abilities
{
    public class CorranHornAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.OnEndPhaseStart += RegisterCorranHornAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnEndPhaseStart -= RegisterCorranHornAbility;
        }

        private void RegisterCorranHornAbility()
        {
            if (!HostShip.Tokens.HasToken(typeof(Tokens.WeaponsDisabledToken)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, UseCorranHornAbility);
            }
        }

        private void UseCorranHornAbility(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Corran Horn can perform second attack");
            Combat.StartAdditionalAttack(HostShip, AfterExtraAttackSubPhase);
        }

        private void AfterExtraAttackSubPhase()
        {
            // "Weapons disabled" token is assigned only if attack was successfully performed
            if (HostShip.IsAttackPerformed) Phases.OnRoundStart += RegisterAssignWeaponsDisabledTrigger;

            Triggers.FinishTrigger();
        }

        private void RegisterAssignWeaponsDisabledTrigger()
        {
            Phases.OnRoundStart -= RegisterAssignWeaponsDisabledTrigger;
            RegisterAbilityTrigger(TriggerTypes.OnRoundStart, AssignWeaponsDisabledTrigger);
        }

        private void AssignWeaponsDisabledTrigger(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(new Tokens.WeaponsDisabledToken(HostShip), Triggers.FinishTrigger);
        }
    }
}
