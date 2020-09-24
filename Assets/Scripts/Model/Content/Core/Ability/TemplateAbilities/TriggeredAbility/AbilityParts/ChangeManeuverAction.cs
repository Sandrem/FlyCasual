using Movement;

namespace Abilities
{
    public class ChangeManeuverAction : AbilityPart
    {
        private TriggeredAbility Ability;
        public bool ChangeToSideslip { get; }

        public ChangeManeuverAction(bool changeToSideslip = false)
        {
            ChangeToSideslip = changeToSideslip;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;

            if (ChangeToSideslip)
            {
                GenericMovement movement = null;
                switch (Ability.HostShip.RevealedManeuver.Bearing)
                {
                    case ManeuverBearing.Bank:
                        movement = new SideslipBankMovement(
                            Ability.HostShip.RevealedManeuver.Speed,
                            Ability.HostShip.RevealedManeuver.Direction,
                            ManeuverBearing.SideslipBank,
                            Ability.HostShip.RevealedManeuver.ColorComplexity
                        );
                        break;
                    case ManeuverBearing.Turn:
                        movement = new SideslipTurnMovement(
                            Ability.HostShip.RevealedManeuver.Speed,
                            Ability.HostShip.RevealedManeuver.Direction,
                            ManeuverBearing.SideslipTurn,
                            Ability.HostShip.RevealedManeuver.ColorComplexity
                        );
                        break;
                    default:
                        break;
                }

                if (movement != null)
                {
                    Messages.ShowInfo("Maneuver is changed to Sideslip");
                    Ability.HostShip.SetAssignedManeuver(movement);
                }
            }

            Triggers.FinishTrigger();
        }
    }
}
