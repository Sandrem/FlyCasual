using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ST70AssaultShip
    {
        public class Q9Dash0 : ST70AssaultShip
        {
            public Q9Dash0() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Q9-0",
                    "Zero",
                    Faction.Scum,
                    5,
                    6,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.Q9Dash0Ability),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Cannon,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.Droid
                    }
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/0/0f/Q90.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Q9Dash0Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (ship.AssignedManeuver != null && ship.AssignedManeuver.IsAdvancedManeuver)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToPerformStrainedActions);
            }
        }

        private void AskToPerformStrainedActions(object sender, EventArgs e)
        {
            HostShip.OnActionIsPerformed += CheckGainStrainToken;

            HostShip.AskPerformFreeAction
            (
                new List<GenericAction>
                {
                    new CalculateAction() {CanBePerformedWhileStressed = true},
                    new BarrelRollAction() {CanBePerformedWhileStressed = true}
                },
                Triggers.FinishTrigger,
                descriptionShort: HostShip.PilotInfo.PilotName,
                descriptionLong: "You may perform one of these actions. If you do, gain 1 strain token.",
                imageHolder: HostShip
            );
        }

        private void CheckGainStrainToken(GenericAction action)
        {
            HostShip.OnActionIsPerformed -= CheckGainStrainToken;

            if (action != null)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, GainStrainToken);
            };
        }

        private void GainStrainToken(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(Tokens.StrainToken), Triggers.FinishTrigger);
        }
    }
}