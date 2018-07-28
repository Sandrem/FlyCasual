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
                Type = FullType = "TIE Reaper";
                IconicPilots.Add(Faction.Imperial, typeof(ScarifBasePilot));

                ManeuversImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/fe/d9/fed9939b-8331-462b-a3b8-d8359d1342bd/swx75_a3_dial.png"; // TODO

                Firepower = 3;
                Agility = 1;
                MaxHull = 6;
                MaxShields = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);

                ActionBar.AddPrintedAction(new EvadeAction());
                ActionBar.AddPrintedAction(new JamAction());

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
                Maneuvers.Add("0.S.S", MovementComplexity.Complex);
                Maneuvers.Add("1.L.R", MovementComplexity.Complex);
                Maneuvers.Add("1.L.T", MovementComplexity.Normal);
                Maneuvers.Add("1.L.B", MovementComplexity.Normal);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Normal);
                Maneuvers.Add("1.R.T", MovementComplexity.Normal);
                Maneuvers.Add("1.R.R", MovementComplexity.Complex);
                Maneuvers.Add("2.L.T", MovementComplexity.Complex);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Complex);
                Maneuvers.Add("3.L.B", MovementComplexity.Complex);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                ShipBaseSize = BaseSize.Medium;
                
                ActionBar.AddPrintedAction(new CoordinateAction() { IsRed = true });

                IconicPilots[Faction.Imperial] = typeof(CaptainFeroph);

                ShipAbilities.Add(new Abilities.AdvancedAileronsAbility());

                Maneuvers["1.L.T"] = MovementComplexity.Complex;
                Maneuvers["1.L.B"] = MovementComplexity.Easy;
                Maneuvers["1.R.B"] = MovementComplexity.Easy;
                Maneuvers["1.R.T"] = MovementComplexity.Complex;
                Maneuvers["2.L.B"] = MovementComplexity.Normal;
                Maneuvers["2.R.B"] = MovementComplexity.Normal;
                Maneuvers["3.L.B"] = MovementComplexity.Normal;
                Maneuvers["3.R.B"] = MovementComplexity.Normal;
            }
        }
    }
}
