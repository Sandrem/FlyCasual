using Bombs;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Mg100StarFortress
    {
        public class EdonKappehl : Mg100StarFortress
        {
            public EdonKappehl() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Edon Kappehl",
                    3,
                    66,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.EdonKappehlAbility),
                    pilotTitle: "Crimson Hailstorm"
                );

                ModelInfo.SkinName = "Crimson";

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/f58fe0b57dc4a9c878627f0fea9cf1ef.png";
            }
        }
    }

}

namespace Abilities.SecondEdition
{
    public class EdonKappehlAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckEdonKappehlAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckEdonKappehlAbility;
        }

        private void CheckEdonKappehlAbility(GenericShip ship)
        {
            if(ship.AssignedManeuver.ColorComplexity != Movement.MovementComplexity.Easy &&
                ship.AssignedManeuver.ColorComplexity != Movement.MovementComplexity.Normal)
            {
                return;
            }
            if (ship.IsBombAlreadyDropped || !BombsManager.HasBombsToDrop(ship))
            {
                return;
            }

            RegisterAbilityTrigger(TriggerTypes.OnMovementActivation, AskEdonKappehlAbility);
        }

        private void AskEdonKappehlAbility(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, UseEdonKappehlAbility);
        }

        private void UseEdonKappehlAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            BombsManager.CreateAskBombDropSubPhase(HostShip);
        }
    }
}
