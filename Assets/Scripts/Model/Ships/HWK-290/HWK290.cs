using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace HWK290
    {
        public class HWK290 : GenericShip, ISecondEditionShip
        {

            public HWK290() : base()
            {
                Type = FullType = "HWK-290";
                IconicPilots.Add(Faction.Rebel, typeof(JanOrs));
                IconicPilots.Add(Faction.Scum, typeof(PalobGodalhi));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/d/d1/MR_HWK-290.png";

                Firepower = 1;
                Agility = 2;
                MaxHull = 4;
                MaxShields = 1;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Turret);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);

                ActionBar.AddPrintedAction(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.HWK290Table();

                factions.Add(Faction.Rebel);
                factions.Add(Faction.Scum);

                SkinName = "Brown";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 1;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
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
                Maneuvers.Add("3.L.B", MovementComplexity.Complex);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                FullType = "HWK-290 Light Freighter";

                Maneuvers.Add("0.S.S", MovementComplexity.Complex);
                Maneuvers.Add("3.L.T", MovementComplexity.Complex);
                Maneuvers["3.L.B"] = MovementComplexity.Normal;
                Maneuvers["3.F.S"] = MovementComplexity.Easy;
                Maneuvers["3.R.B"] = MovementComplexity.Normal;
                Maneuvers.Add("3.R.T", MovementComplexity.Complex);
                Maneuvers["4.F.S"] = MovementComplexity.Normal;

                Firepower = 2;
                MaxHull = 3;
                MaxShields = 2;

                ActionBar.AddActionLink(typeof(FocusAction), new RotateArcAction() { IsRed = true });
                ActionBar.AddActionLink(typeof(TargetLockAction), new RotateArcAction() { IsRed = true });

                ActionBar.AddPrintedAction(new BoostAction() { IsRed = true });
                ActionBar.AddPrintedAction(new RotateArcAction());
                ActionBar.AddPrintedAction(new JamAction() { IsRed = true });

                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Turret);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Modification);

                ShipBaseArcsType = Arcs.BaseArcsType.ArcMobileOnly;

                IconicPilots[Faction.Scum] = typeof(SpiceRunner);
                IconicPilots[Faction.Rebel] = typeof(KyleKatarn);
            }

        }
    }
}
