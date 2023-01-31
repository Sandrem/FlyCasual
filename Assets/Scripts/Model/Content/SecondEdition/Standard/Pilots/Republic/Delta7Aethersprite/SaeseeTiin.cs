using Content;
using Movement;
using Ship;
using System.Collections.Generic;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class SaeseeTiin : Delta7Aethersprite
    {
        public SaeseeTiin()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Saesee Tiin",
                "Prophetic Pilot",
                Faction.Republic,
                4,
                4,
                8,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.SaeseeTiinAbility),
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                legality: new List<Legality>
                {
                    Legality.StandardBanned,
                    Legality.ExtendedLegal
                },
                skinName: "Saesee Tiin"
            );

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
            TargetShip.Owner.ChangeManeuver(ManeuverSelected, Triggers.FinishTrigger, IsSameComplexityAndSpeed);
        }

        private void ManeuverSelected(string maneuverString)
        {
            if (maneuverString != TargetShip.AssignedManeuver.ToString())
            {
                HostShip.State.SpendForce(
                    1,
                    delegate { ShipMovementScript.SendAssignManeuverCommand(maneuverString); }
                );
            }
            else
            {
                ShipMovementScript.SendAssignManeuverCommand(maneuverString);
            }
        }

        private bool IsSameComplexityAndSpeed(string maneuverString)
        {
            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);

            return movementStruct.ColorComplexity == TargetShip.AssignedManeuver.ColorComplexity
                && movementStruct.SpeedIntUnsigned == TargetShip.AssignedManeuver.Speed;
        }
    }
}
