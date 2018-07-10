using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace TIEStriker
    {
        public class TIEStriker : GenericShip, TIE, ISecondEditionShip
        {

            public TIEStriker() : base()
            {
                Type = "TIE Striker";
                IconicPilots.Add(Faction.Imperial, typeof(PureSabacc));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/9/9b/MI_TIE-STRIKER.png";

                Firepower = 3;
                Agility = 2;
                MaxHull = 4;
                MaxShields = 0;

                PrintedActions.Add(new EvadeAction());
                PrintedActions.Add(new BarrelRollAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEStrikerTable();

                factions.Add(Faction.Imperial);
                faction = Faction.Imperial;

                SkinName = "Gray";

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
                Maneuvers.Add("2.L.R", MovementComplexity.Complex);
                Maneuvers.Add("2.F.R", MovementComplexity.Complex);
                Maneuvers.Add("2.R.R", MovementComplexity.Complex);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
            }

            public void AdaptShipToSecondEdition()
            {
                //TODO: Maneuvers
                //TODO: Ship ability

                IconicPilots[Faction.Imperial] = typeof(PlanetarySentinel);
            }

        }
    }
}
