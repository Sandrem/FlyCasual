using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace ScurrgH6Bomber
    {
        public class ScurrgH6Bomber : GenericShip, ISecondEditionShip
        {

            public ScurrgH6Bomber() : base()
            {
                Type = FullType = "Scurrg H-6 Bomber";
                IconicPilots.Add(Faction.Rebel, typeof(CaptainNymRebel));
                IconicPilots.Add(Faction.Scum, typeof(CaptainNymScum));

                ManeuversImageUrl = "https://i.imgur.com/CfJoyso.jpg";

                Firepower = 3;
                Agility = 1;
                MaxHull = 5;
                MaxShields = 5;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Turret);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new BarrelRollAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.ScurrgH6BomberTable();

                factions.Add(Faction.Scum);
                factions.Add(Faction.Rebel);
                faction = Faction.Scum;

                SkinName = "Lok Revenant";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 2;

                for (int i = 1; i < 8; i++)
                {
                    SoundFlyPaths.Add("TIE-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.B", MovementComplexity.Normal);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.T", MovementComplexity.Complex);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Complex);
                Maneuvers.Add("3.L.E", MovementComplexity.Complex);
                Maneuvers.Add("3.R.E", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("5.F.S", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                Maneuvers["1.L.B"] = MovementComplexity.Easy;
                Maneuvers["1.R.B"] = MovementComplexity.Easy;
                Maneuvers["2.L.B"] = MovementComplexity.Normal;
                Maneuvers["2.R.B"] = MovementComplexity.Normal;
                Maneuvers["3.F.S"] = MovementComplexity.Normal;
                Maneuvers["4.F.S"] = MovementComplexity.Complex;
                Maneuvers.Remove("5.F.S");

                factions.Remove(Faction.Rebel);

                MaxHull = 6;
                MaxShields = 4;

                ShipBaseSize = BaseSize.Medium;

                ActionBar.RemovePrintedAction(typeof(BarrelRollAction));
                ActionBar.AddPrintedAction(new BarrelRollAction() { IsRed = true });

                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Missile);

                IconicPilots[Faction.Scum] = typeof(LokRevenant);
            }

        }
    }
}
