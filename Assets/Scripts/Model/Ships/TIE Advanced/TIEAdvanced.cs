using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;
using System.Linq;

namespace Ship
{
    namespace TIEAdvanced
    {
        public class TIEAdvanced : GenericShip, TIE, ISecondEditionShip
        {

            public TIEAdvanced() : base()
            {
                Type = "TIE Advanced";
                IconicPilots.Add(Faction.Imperial, typeof(DarthVader));

                ManeuversImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/8/85/MI_TIE-ADVANCED.png";

                Firepower = 2;
                Agility = 3;
                MaxHull = 3;
                MaxShields = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);

                PrintedActions.Add(new EvadeAction());
                PrintedActions.Add(new BarrelRollAction());
                PrintedActions.Add(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEAdvancedTable();

                factions.Add(Faction.Imperial);
                faction = Faction.Imperial;

                SkinName = "Gray";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 2;

                for (int i = 1; i < 8; i++)
                {
                    SoundFlyPaths.Add("TIE-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", ManeuverColor.None);
                Maneuvers.Add("1.L.B", ManeuverColor.Green);
                Maneuvers.Add("1.F.S", ManeuverColor.None);
                Maneuvers.Add("1.R.B", ManeuverColor.Green);
                Maneuvers.Add("1.R.T", ManeuverColor.None);
                Maneuvers.Add("1.F.R", ManeuverColor.None);
                Maneuvers.Add("2.L.T", ManeuverColor.White);
                Maneuvers.Add("2.L.B", ManeuverColor.White);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.White);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("2.F.R", ManeuverColor.None);
                Maneuvers.Add("3.L.E", ManeuverColor.None);
                Maneuvers.Add("3.L.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.White);
                Maneuvers.Add("3.R.E", ManeuverColor.None);
                Maneuvers.Add("3.F.R", ManeuverColor.None);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
                Maneuvers.Add("4.F.R", ManeuverColor.Red);
                Maneuvers.Add("5.F.S", ManeuverColor.White);
                Maneuvers.Add("5.F.R", ManeuverColor.None);
            }

            public void AdaptShipToSecondEdition()
            {
                PrintedActions.Remove(PrintedActions.First(n => n is EvadeAction));

                Maneuvers["1.F.S"] = ManeuverColor.White;
                Maneuvers["2.L.B"] = ManeuverColor.Green;
                Maneuvers["2.R.B"] = ManeuverColor.Green;
                Maneuvers["3.L.E"] = ManeuverColor.Red;
                Maneuvers["3.R.E"] = ManeuverColor.Red;

                UpgradeBar.AddSlot(Upgrade.UpgradeType.System);
            }

        }
    }
}
