using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;
using Bombs;

namespace Ship
{
    namespace TIEBomber
    {
        public class TIEBomber : GenericShip, TIE, ISecondEditionShip
        {

            public TIEBomber() : base()
            {
                Type = FullType = "TIE Bomber";
                IconicPilots.Add(Faction.Imperial, typeof(TomaxBren));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/9/96/MI_TIE-BOMBER.png";

                Firepower = 2;
                Agility = 2;
                MaxHull = 6;
                MaxShields = 0;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new BarrelRollAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEBomberTable();

                factions.Add(Faction.Imperial);
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
                Maneuvers.Add("1.L.B", MovementComplexity.Normal);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.L.T", MovementComplexity.Complex);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Complex);
                Maneuvers.Add("2.F.R", MovementComplexity.None);
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("5.F.R", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                Maneuvers["2.L.T"] = MovementComplexity.Normal;
                Maneuvers["2.R.T"] = MovementComplexity.Normal;
                Maneuvers.Add("3.F.R", MovementComplexity.Complex);

                FullType = "TIE/sa Bomber";

                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Gunner);

                ActionBar.AddActionLink(typeof(BarrelRollAction), new TargetLockAction() { IsRed = true });
                ActionBar.AddPrintedAction(new ReloadAction() { IsRed = true });

                IconicPilots[Faction.Imperial] = typeof(CaptainJonus);

                ShipAbilities.Add(new Abilities.SecondEdition.NimbleBomber());
            }

        }
    }
}

namespace Abilities.SecondEdition
{
    public class NimbleBomber : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplates += AddNimbleBomberTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplates -= AddNimbleBomberTemplates;
        }

        private void AddNimbleBomberTemplates(List<BombDropTemplates> availableTemplates)
        {
            if (!availableTemplates.Contains(BombDropTemplates.Bank_1_Left)) availableTemplates.Add(BombDropTemplates.Bank_1_Left);
            if (!availableTemplates.Contains(BombDropTemplates.Bank_1_Right)) availableTemplates.Add(BombDropTemplates.Bank_1_Right);
        }
    }
}
