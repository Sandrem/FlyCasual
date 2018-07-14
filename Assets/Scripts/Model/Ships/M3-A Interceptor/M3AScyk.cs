using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace M3AScyk
    {
        public class M3AScyk : GenericShip, ISecondEditionShip
        {

            public M3AScyk() : base()
            {
                Type = "M3-A Interceptor";
                IconicPilots.Add(Faction.Scum, typeof(Inaldra));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/4/48/MS_M3-A-INTERCEPTOR.png";

                Firepower = 2;
                Agility = 3;
                MaxHull = 2;
                MaxShields = 1;

                PrintedActions.Add(new EvadeAction());
                PrintedActions.Add(new BarrelRollAction());
                PrintedActions.Add(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.M3AScykTable();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Inaldra";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 2;

                for (int i = 1; i < 8; i++)
                {
                    SoundFlyPaths.Add("TIE-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.Normal);
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.F.S", MovementComplexity.None);
                Maneuvers.Add("1.R.B", MovementComplexity.Easy);
                Maneuvers.Add("1.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.R", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("5.F.S", MovementComplexity.None);
                Maneuvers.Add("5.F.R", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                // TODO: Maneuvers
                // TODO: Ship ability
            }

        }
    }
}
