using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;

namespace Ship
{
    namespace SecondEdition.StarViperClassAttackPlatform
    {
        public class StarViperClassAttackPlatform : FirstEdition.StarViper.StarViper
        {
            public StarViperClassAttackPlatform() : base()
            {
                ShipInfo.ShipName = "StarViper-class Attack Platform";

                IconicPilots[Faction.Scum] = typeof(BlackSunEnforcer);

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.System);

                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(FocusAction)));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BoostAction), typeof(FocusAction)));

                ShipAbilities.Add(new Abilities.FirstEdition.StarViperMkIIAbility());

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);

                //TODO: ManeuversImageUrl
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class StarViperMkIIAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollTemplates += ChangeBarrelRollTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollTemplates -= ChangeBarrelRollTemplates;
        }

        private void ChangeBarrelRollTemplates(List<ActionsHolder.BarrelRollTemplates> availableTemplates)
        {
            if (availableTemplates.Contains(ActionsHolder.BarrelRollTemplates.Straight1))
            {
                availableTemplates.Remove(ActionsHolder.BarrelRollTemplates.Straight1);
                availableTemplates.Add(ActionsHolder.BarrelRollTemplates.Bank1);
            }
        }
    }
}