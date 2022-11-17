using ActionsList;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class JunoEclipse : TIEAdvancedX1
        {
            public JunoEclipse() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Juno Eclipse",
                    "Corulag's Finest",
                    Faction.Imperial,
                    5,
                    5,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JunoEclipseAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );;

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/44389529-87de-42a0-962c-97d223fb597b/SWZ97_JunoEclipselegal.png";

                RequiredMods = new List<System.Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JunoEclipseAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= RegisterAbility;
        }

        private void RegisterAbility(GenericAction action)
        {
            RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToPerformRedBoost);
        }

        private void AskToPerformRedBoost(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new BoostAction() { HostShip = TargetShip, Color = Actions.ActionColor.Red },
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                "You may perform a red Boost action",
                HostShip
            );
        }
    }
}