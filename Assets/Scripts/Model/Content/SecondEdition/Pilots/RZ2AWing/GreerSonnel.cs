using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class GreerSonnel : RZ2AWing
        {
            public GreerSonnel() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Greer Sonnel",
                    4,
                    36,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.GreerSonnelAbility),
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Talent, UpgradeType.Talent }
                );

                ModelInfo.SkinName = "Red";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/0e/a9/0ea93e07-5308-434d-b1ef-28b4cb8cc9d9/swz22_greer_sonnel.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GreerSonnelAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckGreerSonnelAbilityPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckGreerSonnelAbilityPilotAbility;
        }

        protected void CheckGreerSonnelAbilityPilotAbility(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, PerformAction);
        }

        private void PerformAction(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                UseGreerSonnelAbility,
                descriptionLong: "Do you want to rotate your turret arc indicator?",
                imageHolder: HostShip
            );
        }

        private void UseGreerSonnelAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            new RotateArcAction().DoOnlyEffect(Triggers.FinishTrigger);
        }
    }
}