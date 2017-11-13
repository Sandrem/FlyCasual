using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace ProtectorateStarfighter
    {
        public class ProtectorateStarfighter : GenericShip, TIE
        {

            public ProtectorateStarfighter() : base()
            {
                Type = "Protectorate Starfighter";

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/8/83/MS_PROTECTORATE-STARFIGHTER.png";

                Firepower = 3;
                Agility = 3;
                MaxHull = 4;
                MaxShields = 0;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);

                PrintedActions.Add(new TargetLockAction());
                PrintedActions.Add(new BarrelRollAction());
                PrintedActions.Add(new BoostAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = null;

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Protectorate Starfighter";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 3;

                for (int i = 1; i < 8; i++)
                {
                    SoundFlyPaths.Add("TIE-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", ManeuverColor.White);
                Maneuvers.Add("1.R.T", ManeuverColor.White);
                Maneuvers.Add("2.L.T", ManeuverColor.Green);
                Maneuvers.Add("2.L.B", ManeuverColor.Green);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.Green);
                Maneuvers.Add("2.R.T", ManeuverColor.Green);
                Maneuvers.Add("2.L.E", ManeuverColor.Red);
                Maneuvers.Add("2.R.E", ManeuverColor.Red);
                Maneuvers.Add("3.L.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.White);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
                Maneuvers.Add("4.F.R", ManeuverColor.Red);
                Maneuvers.Add("5.F.S", ManeuverColor.White);
            }

        }
    }
}
