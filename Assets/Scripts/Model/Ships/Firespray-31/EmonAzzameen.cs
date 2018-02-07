using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using Bombs;

namespace Ship
{
    namespace Firespray31
    {
        public class EmonAzzameen : Firespray31
        {
            public EmonAzzameen() : base()
            {
                PilotName = "Emon Azzameen";
                PilotSkill = 6;
                Cost = 36;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.EmonAzzameenAbility());

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                faction = Faction.Scum;

                SkinName = "Emon Azzameen";
            }
        }
    }
}

namespace Abilities
{
    public class EmonAzzameenAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplates += AddEmonAzzameenTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplates -= AddEmonAzzameenTemplates;
        }

        private void AddEmonAzzameenTemplates(List<BombDropTemplates> availableTemplates)
        {
            if (!availableTemplates.Contains(BombDropTemplates.Straight_3)) availableTemplates.Add(BombDropTemplates.Straight_3);
            if (!availableTemplates.Contains(BombDropTemplates.Turn_3_Left)) availableTemplates.Add(BombDropTemplates.Turn_3_Left);
            if (!availableTemplates.Contains(BombDropTemplates.Turn_3_Right)) availableTemplates.Add(BombDropTemplates.Turn_3_Right);
        }
    }
}