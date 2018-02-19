using Abilities;
using GameModes;
using Ship;
using Upgrade;

namespace UpgradesList
{
    public class HotshotCoPilot : GenericUpgrade
    {
        public HotshotCoPilot() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Hotshot Co-pilot";
            Cost = 4;

            UpgradeAbilities.Add(new HotshotCoPilotAbility());
        }
    }
}

namespace Abilities
{
    public class HotshotCoPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}