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
                Type = "HWK-290";
                IconicPilots.Add(Faction.Rebel, typeof(JanOrs));
                IconicPilots.Add(Faction.Scum, typeof(PalobGodalhi));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/d/d1/MR_HWK-290.png";

                Firepower = 1;
                Agility = 2;
                MaxHull = 4;
                MaxShields = 1;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Turret);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);

                PrintedActions.Add(new TargetLockAction());

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
                Maneuvers.Add("3.L.T", MovementComplexity.None);
                Maneuvers.Add("3.L.B", MovementComplexity.Complex);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Complex);
                Maneuvers.Add("3.R.T", MovementComplexity.None);
                Maneuvers.Add("3.F.R", MovementComplexity.None);
                Maneuvers.Add("4.F.S", MovementComplexity.Complex);
                Maneuvers.Add("4.F.R", MovementComplexity.None);
                Maneuvers.Add("5.F.S", MovementComplexity.None);
                Maneuvers.Add("5.F.R", MovementComplexity.None);
            }

            public void AdaptShipToSecondEdition()
            {
                //TODO: Maneuvers
                //TODO: Only mobile arc

                Firepower = 2;
                MaxHull = 3;
                MaxShields = 2;

                PrintedActions.RemoveAll(a => a is FocusAction);
                PrintedActions.RemoveAll(a => a is TargetLockAction);

                PrintedActions.Add(new FocusAction() { LinkedRedAction = new RotateArcAction() { IsRed = true } });
                PrintedActions.Add(new TargetLockAction() { LinkedRedAction = new RotateArcAction() { IsRed = true } });

                PrintedActions.Add(new BoostAction() { IsRed = true });
                PrintedActions.Add(new RotateArcAction());
                PrintedActions.Add(new JamAction() { IsRed = true });

                IconicPilots[Faction.Scum] = typeof(SpiceRunner);
            }

        }
    }
}
