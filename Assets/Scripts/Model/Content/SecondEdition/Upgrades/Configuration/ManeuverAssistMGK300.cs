using Upgrade;
using Ship;
using Movement;
using ActionsList;
using Actions;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class ManeuverAssistMGK300 : GenericUpgrade
    {
        public ManeuverAssistMGK300() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Maneuver-Assist MGK-300",
                UpgradeType.Configuration,
                cost: 0,
                addActions: new List<ActionInfo>()
                {
                    new ActionInfo(typeof(CalculateAction)),
                    new ActionInfo(typeof(BarrelRollAction)) 
                },
                addActionLink: new LinkedActionInfo(typeof(BarrelRollAction), typeof(CalculateAction), ActionColor.Red),
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.TIERbHeavy.TIERbHeavy)),
                abilityType: typeof(Abilities.SecondEdition.ManeuverAssistMGK300Ability)
            );

            ImageUrl = "https://i.imgur.com/tYqSuOn.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ManeuverAssistMGK300Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += ApplyAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= ApplyAbility;
        }

        private void ApplyAbility(GenericShip ship, ref ManeuverHolder movement)
        {
            if (movement.Speed == ManeuverSpeed.Speed3)
            {
                if (movement.Bearing == ManeuverBearing.Straight || movement.Bearing == ManeuverBearing.Bank)
                {
                    movement.ColorComplexity = GenericMovement.ReduceComplexity(movement.ColorComplexity);
                    // Update revealed dial in UI
                    Roster.UpdateAssignedManeuverDial(HostShip, HostShip.AssignedManeuver);
                    Messages.ShowInfoToHuman("Maneuver-Assist MGK-300: Difficulty of maneuvers is reduced");
                }
            }
        }
    }
}