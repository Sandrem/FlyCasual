using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship
{
    namespace TIEFO
    {
        public class ZetaAce : TIEFO
        {
            public ZetaAce() : base()
            {
                PilotName = "\"Zeta Ace\"";
                PilotSkill = 5;
                Cost = 18;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.ZetaAceAbility());
            }
        }
    }
}

namespace Abilities
{
    public class ZetaAceAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollTemplates += ChangeBarrelRollTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollTemplates -= ChangeBarrelRollTemplates;
        }

        private void ChangeBarrelRollTemplates(List<Actions.BarrelRollTemplates> availableTemplates)
        {
            if (availableTemplates.Contains(Actions.BarrelRollTemplates.Straight1))
            {
                availableTemplates.Add(Actions.BarrelRollTemplates.Straight2);
            }
        }
    }
}