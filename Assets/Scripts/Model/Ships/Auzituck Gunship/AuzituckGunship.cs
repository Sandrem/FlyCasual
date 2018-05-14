using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace AuzituckGunship
    {
        public class AuzituckGunship : GenericShip
        {

            public AuzituckGunship() : base()
            {
                Type = "Auzituck Gunship";
                IconicPilots.Add(Faction.Rebel, typeof(Lowhhrick));
                ShipBaseArcsType = Arcs.BaseArcsType.Arc180;

                ManeuversImageUrl = "https://i.imgur.com/d8r9zJB.jpg";

                Firepower = 3;
                Agility = 1;
                MaxHull = 6;
                MaxShields = 3;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);

                PrintedActions.Add(new ReinforceForeAction() { Host = this });
                PrintedActions.Add(new ReinforceAftAction() { Host = this });

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.AuzituckGunshipTable();

                factions.Add(Faction.Rebel);
                faction = Faction.Rebel;

                SkinName = "Kashyyyk Defender";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 3;

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
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("5.F.S", MovementComplexity.Complex);
            }

        }
    }
}
