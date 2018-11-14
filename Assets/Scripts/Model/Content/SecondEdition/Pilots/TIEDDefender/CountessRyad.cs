using GameModes;
using Movement;
using Ship;
using System;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class CountessRyad : TIEDDefender
        {
            public CountessRyad() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Countess Ryad",
                    4,
                    86,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.CountessRyadAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                ModelInfo.SkinName = "Crimson";

                SEImageNumber = 124;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you would execute a straight maneuver, you may increase difficulty of the maeuver. If you do, execute it as a koiogran turn maneuver instead.
    public class CountessRyadAbility : Abilities.FirstEdition.CountessRyadAbility
    {
        public override void ActivateAbility()
        {
            HostShip.BeforeMovementIsExecuted += RegisterAskChangeManeuver;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeMovementIsExecuted -= RegisterAskChangeManeuver;
        }

        protected override void RegisterAskChangeManeuver(GenericShip ship)
        {
            //I have assumed that you can not use this ability if you execute a red maneuver
            if (HostShip.AssignedManeuver.ColorComplexity != MovementComplexity.Complex && HostShip.AssignedManeuver.Bearing == ManeuverBearing.Straight)
            {
                RegisterAbilityTrigger(TriggerTypes.BeforeMovementIsExecuted, AskChangeManeuver);
            }
        }

        protected override MovementComplexity GetNewManeuverComplexity()
        {
            if (HostShip.AssignedManeuver.ColorComplexity == MovementComplexity.Complex)
                throw new Exception("Can't increase difficulty of red maneuvers");

            return HostShip.AssignedManeuver.ColorComplexity + 1;
        }
    }
}

namespace Abilities.FirstEdition
{
    public class CountessRyadAbility : GenericAbility
    {
        string maneuverKey;
        MovementComplexity originalColor;

        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterAskChangeManeuver;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterAskChangeManeuver;
        }

        protected virtual void RegisterAskChangeManeuver(GenericShip ship)
        {
            if (HostShip.AssignedManeuver.Bearing == ManeuverBearing.Straight)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);
            }
        }

        protected virtual MovementComplexity GetNewManeuverComplexity()
        {
            return HostShip.AssignedManeuver.ColorComplexity;
        }

        protected void AskChangeManeuver(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman("Countess Ryad: You can change your maneuver to Koiogran turn");
            maneuverKey = HostShip.AssignedManeuver.Speed + ".F.R";
            originalColor = (HostShip.Maneuvers.ContainsKey(maneuverKey)) ? HostShip.Maneuvers[maneuverKey] : MovementComplexity.None;
            HostShip.Maneuvers[maneuverKey] = GetNewManeuverComplexity();
            HostShip.Owner.ChangeManeuver((maneuverCode) => {
                GameMode.CurrentGameMode.AssignManeuver(maneuverCode);
                HostShip.OnMovementFinish += RestoreManuvers;
            }, StraightOrKoiogran);
        }

        private void RestoreManuvers(GenericShip ship)
        {
            HostShip.OnMovementFinish -= RestoreManuvers;

            if (originalColor != MovementComplexity.None)
            {
                HostShip.Maneuvers[maneuverKey] = originalColor;
            }
            else
            {
                HostShip.Maneuvers.Remove(maneuverKey);
            }
        }

        private bool StraightOrKoiogran(string maneuverString)
        {
            bool result = false;
            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);
            if (movementStruct.Speed == Selection.ThisShip.AssignedManeuver.ManeuverSpeed &&
                (movementStruct.Bearing == ManeuverBearing.Straight ||
                movementStruct.Bearing == ManeuverBearing.KoiogranTurn))
            {
                result = true;
            }
            return result;
        }
    }
}