using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using RuleSets;

namespace Ship
{
    namespace YT2400
    {
        public class DashRendar : YT2400, ISecondEditionPilot
        {
            public DashRendar() : base()
            {
                PilotName = "Dash Rendar";
                PilotSkill = 7;
                Cost = 36;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.DashRendarAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 100;

                PilotAbilities.RemoveAll(ability => ability is Abilities.DashRendarAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.DashRendarAbilitySE());
            }
        }
    }
}

namespace Abilities
{
    public class DashRendarAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnActivationPhaseStart += ActivateDashRendarAbility;
            Phases.Events.OnCombatPhaseStart_NoTriggers += DeactivateDashRendarAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnActivationPhaseStart -= ActivateDashRendarAbility;
            Phases.Events.OnCombatPhaseStart_NoTriggers -= DeactivateDashRendarAbility;
        }

        private void ActivateDashRendarAbility()
        {
            HostShip.IsIgnoreObstacles = true;
        }

        private void DeactivateDashRendarAbility()
        {
            HostShip.IsIgnoreObstacles = false;
        }

    }
}

namespace Abilities.SecondEdition
{
    public class DashRendarAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.IsIgnoreObstacles = true;
        }

        public override void DeactivateAbility()
        {
            HostShip.IsIgnoreObstacles = false;
        }
    }
}