using Bombs;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Firespray31
    {
        public class EmonAzzameen : Firespray31
        {
            public EmonAzzameen() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Emon Azzameen",
                    6,
                    36,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.EmonAzzameenAbility),
                    extraUpgradeIcon: UpgradeType.Illicit,
                    factionOverride: Faction.Scum
                );

                ModelInfo.SkinName = "Emon Azzameen";
            }
        }
    }
}

namespace Abilities.FirstEdition
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