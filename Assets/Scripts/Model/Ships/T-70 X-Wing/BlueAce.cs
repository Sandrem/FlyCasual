using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace T70XWing
    {
        public class BlueAce : T70XWing
        {
            public BlueAce() : base()
            {
                PilotName = "\"Blue Ace\"";
                PilotSkill = 5;
                Cost = 27;

                IsUnique = true;
                PilotAbilities.Add(new Abilities.BlueAceAbility());
            }
        }
    }
}

namespace Abilities
{
    public class BlueAceAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates += ChangeBoostTemplate;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates -= ChangeBoostTemplate;
        }

        private void ChangeBoostTemplate(List<Actions.BoostTemplates> availableTemplates)
        {
            availableTemplates.Add(Actions.BoostTemplates.LeftTurn1);
            availableTemplates.Add(Actions.BoostTemplates.RightTurn1);
        }
    }
}
