using BoardTools;
using Movement;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEFoFighter
    {
        public class ZetaAce : TIEFoFighter
        {
            public ZetaAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Zeta Ace\"",
                    5,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.ZetaAceAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
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

        private void ChangeBarrelRollTemplates(List<ManeuverTemplate> availableTemplates)
        {
            availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed2));
        }
    }
}