﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Firespray31
    {
        public class KathScarletEmpire : Firespray31
        {
            public KathScarletEmpire() : base()
            {
                PilotName = "Kath Scarlet";
                PilotSkill = 7;
                Cost = 38;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                faction = Faction.Imperial;

                SkinName = "Kath Scarlet";

                PilotAbilities.Add(new Abilities.KathScarletEmpireAbility());
            }
        }
    }
}

namespace Abilities
{
    public class KathScarletEmpireAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAtLeastOneCritWasCancelledByDefender += RegisterKathScarletPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAtLeastOneCritWasCancelledByDefender -= RegisterKathScarletPilotAbility;
        }

        private void RegisterKathScarletPilotAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAtLeastOneCritWasCancelledByDefender, KathScarletPilotAbility);
        }

        private void KathScarletPilotAbility(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Critical hit was cancelled - stress token is assigned to the defender");
            Combat.Defender.Tokens.AssignToken(new Tokens.StressToken(Combat.Defender), Triggers.FinishTrigger);
        }
    }
}
