using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using Ship;
using RuleSets;

namespace Ship
{
    namespace XWing
    {
        public class XWing : GenericShip, IMovableWings, ISecondEditionShip
        {
            public WingsPositions CurrentWingsPosition { get; set; }

            public XWing() : base()
            {
                Type = "X-Wing";
                IconicPilots.Add(Faction.Rebel, typeof(WedgeAntilles));

                ManeuversImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/3/3d/MR_T65-X-WING.png";

                ShipIconLetter = 'x';

                Firepower = 3;
                Agility = 2;
                MaxHull = 3;
                MaxShields = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);

                ActionBar.AddPrintedAction(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.XWingTable();

                factions.Add(Faction.Rebel);
                faction = Faction.Rebel;

                SkinName = "Red";

                CurrentWingsPosition = WingsPositions.Opened;

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 3;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.None);
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Easy);
                Maneuvers.Add("1.R.T", MovementComplexity.None);
                Maneuvers.Add("1.F.R", MovementComplexity.None);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Normal);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.F.R", MovementComplexity.None);
                Maneuvers.Add("3.L.E", MovementComplexity.None);
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.R.E", MovementComplexity.None);
                Maneuvers.Add("3.F.R", MovementComplexity.None);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
                Maneuvers.Add("5.F.S", MovementComplexity.None);
                Maneuvers.Add("5.F.R", MovementComplexity.None);
            }

            public void AdaptShipToSecondEdition()
            {
                MaxHull = 4;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Configuration);

                ActionBar.AddPrintedAction(new BarrelRollAction());

                Maneuvers["2.L.B"] = MovementComplexity.Easy;
                Maneuvers["2.R.B"] = MovementComplexity.Easy;
                Maneuvers["3.L.E"] = MovementComplexity.Complex;
                Maneuvers["3.R.E"] = MovementComplexity.Complex;
            }
        }
    }
}
