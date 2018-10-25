using RuleSets;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace Ship
{
    namespace TIEPhantom
    {
        public class Echo : TIEPhantom, ISecondEditionPilot
        {
            public Echo() : base()
            {
                PilotName = "\"Echo\"";
                PilotSkill = 4;
                Cost = 50;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new Abilities.SecondEdition.EchoAbilitySE());

                SEImageNumber = 132;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EchoAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableDecloakTemplates += ChangeDecloakTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableDecloakTemplates -= ChangeDecloakTemplates;
        }

        private void ChangeDecloakTemplates(List<Actions.DecloakTemplates> availableTemplates)
        {
            if (availableTemplates.Contains(Actions.DecloakTemplates.Straight2))
            {
                availableTemplates.Remove(Actions.DecloakTemplates.Straight2);
                availableTemplates.Add(Actions.DecloakTemplates.Bank2);
            }
        }
    }
}
