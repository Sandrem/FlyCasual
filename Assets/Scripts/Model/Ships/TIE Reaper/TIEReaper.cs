using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace TIEReaper
    {
        public class TIEReaper : GenericShip, TIE, ISecondEditionShip
        {

            public TIEReaper() : base()
            {
                Type = "TIE Reaper";
                IconicPilots.Add(Faction.Imperial, typeof(ScarifBasePilot));

                ManeuversImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/fe/d9/fed9939b-8331-462b-a3b8-d8359d1342bd/swx75_a3_dial.png"; // TODO

                Firepower = 3;
                Agility = 1;
                MaxHull = 6;
                MaxShields = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);

                PrintedActions.Add(new EvadeAction());
                PrintedActions.Add(new JamAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEReaperTable();

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
                Maneuvers.Add("0.S.S", ManeuverColor.Red);
                Maneuvers.Add("1.L.R", ManeuverColor.Red);
                Maneuvers.Add("1.L.T", ManeuverColor.White);
                Maneuvers.Add("1.L.B", ManeuverColor.White);
                Maneuvers.Add("1.F.S", ManeuverColor.Green);
                Maneuvers.Add("1.R.B", ManeuverColor.White);
                Maneuvers.Add("1.R.T", ManeuverColor.White);
                Maneuvers.Add("1.R.R", ManeuverColor.Red);
                Maneuvers.Add("2.L.T", ManeuverColor.Red);
                Maneuvers.Add("2.L.B", ManeuverColor.Green);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.Green);
                Maneuvers.Add("2.R.T", ManeuverColor.Red);
                Maneuvers.Add("3.L.B", ManeuverColor.Red);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.Red);
            }

            public void AdaptShipToSecondEdition()
            {
                ShipBaseSize = BaseSize.Medium;
                
                PrintedActions.Add(new CoordinateAction());
            }
        }
    }
}
