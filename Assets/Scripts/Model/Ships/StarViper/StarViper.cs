using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace StarViper
    {
        public class StarViper : GenericShip, ISecondEditionShip
        {

            public StarViper() : base()
            {
                Type = "StarViper";
                IconicPilots.Add(Faction.Scum, typeof(Thweek));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/b/bd/MS_STARVIPER.png";

                Firepower = 3;
                Agility = 3;
                MaxHull = 4;
                MaxShields = 1;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new BarrelRollAction());
                ActionBar.AddPrintedAction(new BoostAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.StarviperTable();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Black Sun Enforcer";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 3;

                for (int i = 1; i < 8; i++)
                {
                    SoundFlyPaths.Add("TIE-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.Normal);
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Easy);
                Maneuvers.Add("1.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Normal);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.L.R", MovementComplexity.Complex);
                Maneuvers.Add("3.R.R", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
            }

            public void AdaptShipToSecondEdition()
            {
                //TODO: Maneuvers
                //TODO: Ship Ability

                ActionBar.RemovePrintedAction(typeof(BarrelRollAction));
                ActionBar.RemovePrintedAction(typeof(BoostAction));

                ActionBar.AddPrintedAction(new BarrelRollAction() { LinkedRedAction = new FocusAction() { IsRed = true } });
                ActionBar.AddPrintedAction(new BoostAction() { LinkedRedAction = new FocusAction() { IsRed = true } });

                IconicPilots[Faction.Scum] = typeof(BlackSunEnforcer);
            }

        }
    }
}
