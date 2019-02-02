using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEPhPhantom
    {
        public class Echo : TIEPhPhantom
        {
            public Echo() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Echo\"",
                    4,
                    50,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.EchoAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 132
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EchoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableDecloakTemplates += ChangeDecloakTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableDecloakTemplates -= ChangeDecloakTemplates;
        }

        private void ChangeDecloakTemplates(List<ActionsHolder.DecloakTemplates> availableTemplates)
        {
            if (availableTemplates.Contains(ActionsHolder.DecloakTemplates.Straight2))
            {
                availableTemplates.Remove(ActionsHolder.DecloakTemplates.Straight2);
                availableTemplates.Add(ActionsHolder.DecloakTemplates.Bank2);
            }
        }
    }
}
