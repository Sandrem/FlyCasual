using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class SabineWren : TIELnFighter
        {
            public SabineWren() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sabine Wren",
                    3,
                    28,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.SabineWrenPilotAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ShipInfo.Faction = Faction.Rebel;

                ModelInfo.ModelName = "TIE Fighter Rebel";
                ModelInfo.SkinName = "Rebel";

                SEImageNumber = 47;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class SabineWrenPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterSabineWrenPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterSabineWrenPilotAbility;
        }

        private void RegisterSabineWrenPilotAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, PerformFreeReposition);
        }

        private void PerformFreeReposition(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = new List<GenericAction>() { new BoostAction(), new BarrelRollAction() };

            HostShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }
    }
}