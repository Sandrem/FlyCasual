using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace Kihraxz
    {
        public class Kihraxz : GenericShip, ISecondEditionShip
        {

            public Kihraxz() : base()
            {
                Type = FullType = "Kihraxz Fighter";
                IconicPilots.Add(Faction.Scum, typeof(TalonbaneCobra));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/d/d8/MS_KIHRAXZ-FIGHTER.png";

                Firepower = 3;
                Agility = 2;
                MaxHull = 4;
                MaxShields = 1;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                ActionBar.AddPrintedAction(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.KihraxzTable();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Hutt Cartel";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 3;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
                
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.Normal);
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Easy);
                Maneuvers.Add("1.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
                Maneuvers.Add("5.F.R", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                Maneuvers.Add("2.L.E", MovementComplexity.Complex);
                Maneuvers.Add("2.R.E", MovementComplexity.Complex);
                Maneuvers["3.F.S"] = MovementComplexity.Easy;
                Maneuvers.Remove("5.F.R");

                MaxHull = 5;

                ActionBar.AddPrintedAction(new BarrelRollAction());

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Modification);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Modification);

                IconicPilots[Faction.Scum] = typeof(BlackSunAce);
            }

        }
    }
}
