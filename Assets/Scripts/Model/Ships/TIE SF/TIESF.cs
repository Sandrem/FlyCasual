using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace TIESF
    {
        public class TIESF : GenericShip, TIE
        {

            public TIESF() : base()
            {
                Type = "TIE/SF Fighter";
                IconicPilots.Add(Faction.Imperial, typeof(QuickDraw));

                ShipBaseArcsType = Arcs.BaseArcsType.ArcRear;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/f/ff/MI_TIE-SF-FIGHTER.png";

                Firepower = 2;
                Agility = 2;
                MaxHull = 3;
                MaxShields = 3;

                SubFaction = SubFaction.FirstOrder;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Tech);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new BarrelRollAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIESFTable();

                factions.Add(Faction.Imperial);
                faction = Faction.Imperial;

                SkinName = "First Order";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 2;

                for (int i = 1; i < 8; i++)
                {
                    SoundFlyPaths.Add("TIE-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.Complex);
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Easy);
                Maneuvers.Add("1.R.T", MovementComplexity.Complex);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Normal);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.T", MovementComplexity.Complex);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Complex);
                Maneuvers.Add("3.L.R", MovementComplexity.Complex);
                Maneuvers.Add("3.R.R", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
            }

        }
    }
}
