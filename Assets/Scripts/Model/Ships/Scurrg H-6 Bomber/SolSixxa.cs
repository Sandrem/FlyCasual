using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bombs;
using Abilities;

namespace Ship
{
    namespace ScurrgH6Bomber
    {
        public class SolSixxa : ScurrgH6Bomber
        {
            public SolSixxa() : base()
            {
                PilotName = "Sol Sixxa";
                PilotSkill = 6;
                Cost = 28;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SolSixxaAbiliity());
            }
        }
    }
}

namespace Abilities
{
    public class SolSixxaAbiliity : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplates += SolSixxaTemplate;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplates -= SolSixxaTemplate;
        }

        private void SolSixxaTemplate(List<BombDropTemplates> availableTemplates)
        {
            if (!availableTemplates.Contains(BombDropTemplates.Turn1Left)) availableTemplates.Add(BombDropTemplates.Turn1Left);
            if (!availableTemplates.Contains(BombDropTemplates.Turn1Right)) availableTemplates.Add(BombDropTemplates.Turn1Right);
        }
    }
}
