using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;

namespace Ship
{
    namespace SecondEdition.AggressorAssaultFighter
    {
        public class AggressorAssaultFighter : FirstEdition.Aggressor.Aggressor
        {
            public AggressorAssaultFighter() : base()
            {
                ShipInfo.ShipName = "Aggressor Assault Fighter";
                ShipInfo.BaseSize = BaseSize.Medium;

                ShipInfo.ActionIcons.SwitchToDroidActions();

                ShipInfo.Hull = 5;
                ShipInfo.Shields = 3;

                IconicPilots[Faction.Scum] = typeof(IG88A);

                ShipAbilities.Add(new Abilities.SecondEdition.AdvancedDroidBrain());

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Normal);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/3/33/Maneuver_aggressor.png";

                OldShipTypeName = "Aggressor";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AdvancedDroidBrain : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (action is CalculateAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AssignCalculateToken);
            }
        }

        private void AssignCalculateToken(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(Tokens.CalculateToken), Triggers.FinishTrigger);
        }
    }
}