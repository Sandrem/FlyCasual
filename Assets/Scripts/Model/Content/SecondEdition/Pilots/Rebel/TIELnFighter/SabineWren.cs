using ActionsList;
using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Sabine Wren",
                    "Spectre-5",
                    Faction.Rebel,
                    3,
                    2,
                    3,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SabineWrenPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Mandalorian,
                        Tags.Tie,
                        Tags.Spectre
                    },
                    seImageNumber: 47
                );

                PilotNameCanonical = "sabinewren-tielnfighter";

                ModelInfo.ModelName = "TIE Fighter Rebel";
                ModelInfo.SkinName = "Rebel";
            }
        }
    }
}


namespace Abilities.SecondEdition
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

            HostShip.AskPerformFreeAction(
                actions,
                Triggers.FinishTrigger,
                descriptionShort: HostShip.PilotInfo.PilotName,
                descriptionLong: "Before you activate, you may perform a Barrel Roll or Boost action",
                imageHolder: HostShip
            );
        }
    }
}
