using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace KWing
    {
        public class KWing : GenericShip, ISecondEditionShip
        {

            public KWing() : base()
            {
                Type = FullType = "K-Wing";
                IconicPilots.Add(Faction.Rebel, typeof(MirandaDoni));

                ShipBaseArcsType = Arcs.BaseArcsType.Arc360;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/9/95/MR_K-WING.png";

                Firepower = 2;
                Agility = 1;
                MaxHull = 5;
                MaxShields = 4;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Turret);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new SlamAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.KWingTable();

                factions.Add(Faction.Rebel);
                faction = Faction.Rebel;

                SkinName = "Red";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 2;

                for (int i = 1; i < 3; i++)
                {
                    SoundFlyPaths.Add("YWing-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Normal);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
            }

            public void AdaptShipToSecondEdition()
            {
                FullType = "BTL-S8 K-wing";

                MaxHull = 6;
                MaxShields = 3;

                ShipBaseArcsType = Arcs.BaseArcsType.ArcMobileDual;

                ShipBaseSize = BaseSize.Medium;

                ActionBar.AddPrintedAction(new RotateArcAction());
                ActionBar.AddPrintedAction(new ReloadAction());

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Gunner);
                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Turret);
                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Torpedo);

                IconicPilots[Faction.Rebel] = typeof(WardenSquadronPilot);
            }
        }
    }
}
