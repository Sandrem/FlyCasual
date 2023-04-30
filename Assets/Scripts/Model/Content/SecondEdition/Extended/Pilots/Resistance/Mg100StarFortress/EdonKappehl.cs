using Bombs;
using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Edon Kappehl",
                    "Crimson Hailstorm",
                    Faction.Resistance,
                    3,
                    6,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.EdonKappehlAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Tech,
                        UpgradeType.Cannon,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ModelInfo.SkinName = "Crimson";
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

            RegisterAbilityTrigger(TriggerTypes.OnMovementActivationStart, AskEdonKappehlAbility);
        }

        private void AskEdonKappehlAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                UseEdonKappehlAbility,
                descriptionLong: "Do you want to drop 1 device?",
                imageHolder: HostShip
            );
        }

        private void UseEdonKappehlAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            BombsManager.CreateAskBombDropSubPhase(HostShip, onlyDrop: true);
        }
    }
}
