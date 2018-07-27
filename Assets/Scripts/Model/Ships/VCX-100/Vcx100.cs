using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace Vcx100
    {
        public class Vcx100 : GenericShip, ISecondEditionShip
        {

            public Vcx100() : base()
            {
                Type = "VCX-100";
                IconicPilots.Add(Faction.Rebel, typeof(KananJarrus));
                ShipBaseSize = BaseSize.Large;
                ShipBaseArcsType = Arcs.BaseArcsType.ArcSpecialGhost;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/c/cf/MR_VCX-100.png";

                Firepower = 4;
                Agility = 0;
                MaxHull = 10;
                MaxShields = 6;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Turret);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new EvadeAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.VCX100Table();

                factions.Add(Faction.Rebel);
                faction = Faction.Rebel;

                SkinName = "VCX-100";

                SoundShotsPath = "Falcon-Fire";
                ShotsCount = 4;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("Falcon-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.Complex);
                Maneuvers.Add("1.L.B", MovementComplexity.Normal);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Normal);
                Maneuvers.Add("1.R.T", MovementComplexity.Complex);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.T", MovementComplexity.Complex);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Normal);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("5.F.R", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                //TODO: Ability

                MaxShields = 4;

                ActionBar.AddPrintedAction(new ReinforceForeAction());
                ActionBar.AddPrintedAction(new ReinforceAftAction());
                ActionBar.RemovePrintedAction(typeof(EvadeAction));

                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.System);
                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Gunner);

                Maneuvers.Remove("5.F.R");
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
            }
        }
    }
}
