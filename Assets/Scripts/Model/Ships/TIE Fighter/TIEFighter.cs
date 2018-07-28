using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace TIEFighter
    {
        public class TIEFighter : GenericShip, TIE, ISecondEditionShip
        {

            public TIEFighter() : base()
            {
                Type = FullType = "TIE Fighter";
                IconicPilots.Add(Faction.Rebel, typeof(ZebOrrelios));
                IconicPilots.Add(Faction.Imperial, typeof(BlackSquadronPilot));

                ManeuversImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/b/b6/TIE_Fighter_Move.png";

                ShipIconLetter = 'F';

                Firepower = 2;
                Agility = 3;
                MaxHull = 3;
                MaxShields = 0;

                ActionBar.AddPrintedAction(new EvadeAction());
                ActionBar.AddPrintedAction(new BarrelRollAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEFighterTable();

                factions.Add(Faction.Imperial);
                factions.Add(Faction.Rebel);
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
                Maneuvers.Add("1.L.T", MovementComplexity.Normal);
                Maneuvers.Add("1.L.B", MovementComplexity.None);
                Maneuvers.Add("1.F.S", MovementComplexity.None);
                Maneuvers.Add("1.R.B", MovementComplexity.None);
                Maneuvers.Add("1.R.T", MovementComplexity.Normal);
                Maneuvers.Add("1.F.R", MovementComplexity.None);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.F.R", MovementComplexity.None);
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.F.R", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
                Maneuvers.Add("5.F.S", MovementComplexity.Normal);
                Maneuvers.Add("5.F.R", MovementComplexity.None);
            }

            public void AdaptShipToSecondEdition()
            {
                FullType = "TIE/ln Fighter";

                IconicPilots[Faction.Imperial] = typeof(BlackSquadronAce);
                IconicPilots[Faction.Rebel] = typeof(SabineWren);
            }

        }
    }
}
