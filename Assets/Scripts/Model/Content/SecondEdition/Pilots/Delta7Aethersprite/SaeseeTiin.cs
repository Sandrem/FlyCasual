using GameModes;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class SaeseeTiin : Delta7Aethersprite
    {
        public SaeseeTiin()
        {
            PilotInfo = new PilotCardInfo(
                "Saesee Tiin",
                4,
                58,
                true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.SaeseeTiinAbility),
                extraUpgradeIcon: UpgradeType.Force
            );

            ModelInfo.SkinName = "Saesee Tiin";

            RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/77/73/777350cb-614b-48fd-ad8d-d9c867053c6b/swz32_saesee-tiin.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After a friendly ship at range 0-2 reveals its dial, you may spend 1 force. 
    //If you do, set its dial to another maneuver of the same speed and difficulty.
    public class SaeseeTiinAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnManeuverIsRevealedGlobal += RegisterAskChangeManeuver;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnManeuverIsRevealedGlobal -= RegisterAskChangeManeuver;
        }

        private void RegisterAskChangeManeuver(GenericShip ship)
        {
            if (HostShip.State.Force > 0
                && ship.Owner == HostShip.Owner
                && new BoardTools.DistanceInfo(ship, HostShip).Range < 3)
            {
                TargetShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);
            }
        }

        private void AskChangeManeuver(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman(HostName + ": You may change the maneuver");
            TargetShip.Owner.ChangeManeuver(ManeuverSelected, IsSameComplexityAndSpeed);
        }

        private void ManeuverSelected(string maneuverString)
        {
            if (maneuverString != TargetShip.AssignedManeuver.ToString()) HostShip.State.Force--;
            GameMode.CurrentGameMode.AssignManeuver(maneuverString);
        }

        private bool IsSameComplexityAndSpeed(string maneuverString)
        {
            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);

            return movementStruct.ColorComplexity == TargetShip.AssignedManeuver.ColorComplexity
                && movementStruct.SpeedInt == TargetShip.AssignedManeuver.Speed;
        }
    }
}
