using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YT2400
    {
        public class DashRendar : YT2400
        {
            public DashRendar() : base()
            {
                PilotName = "Dash Rendar";
                PilotSkill = 7;
                Cost = 36;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new PilotAbilitiesNamespace.DashRendarAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class DashRendarAbility : GenericPilotAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            Phases.OnActivationPhaseStart += ActivateDashRendarAbility;
            Phases.OnCombatPhaseStart += DeactivateDashRendarAbility;
        }

        private void ActivateDashRendarAbility()
        {
            Host.IsIgnoreObstacles = true;
        }

        private void DeactivateDashRendarAbility()
        {
            Host.IsIgnoreObstacles = false;
        }
    }
}
