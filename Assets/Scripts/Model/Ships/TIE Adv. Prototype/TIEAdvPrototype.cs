using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace TIEAdvPrototype
    {
        public class TIEAdvPrototype : GenericShip, TIE, ISecondEditionShip
        {

            public TIEAdvPrototype() : base()
            {
                Type = "TIE Adv. Prototype";
                IconicPilots.Add(Faction.Imperial, typeof(TheInquisitor));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/b/b4/MI_TIE-ADV.-PROTOTYPE.png";

                Firepower = 2;
                Agility = 3;
                MaxHull = 2;
                MaxShields = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new BarrelRollAction());
                ActionBar.AddPrintedAction(new BoostAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEAdvPrototypeTable();

                factions.Add(Faction.Imperial);
                faction = Faction.Imperial;

                SkinName = "White";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 2;

                for (int i = 1; i < 8; i++)
                {
                    SoundFlyPaths.Add("TIE-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.Easy);
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Easy);
                Maneuvers.Add("1.R.T", MovementComplexity.Easy);
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
                Maneuvers.Add("4.F.S", MovementComplexity.Easy);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
                Maneuvers.Add("5.F.S", MovementComplexity.Normal);
            }

            public void AdaptShipToSecondEdition()
            {
                Maneuvers.Add("2.L.E", MovementComplexity.Complex);
                Maneuvers.Add("2.R.E", MovementComplexity.Complex);
                Maneuvers["4.F.S"] = MovementComplexity.Normal;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);
                UpgradeBar.RemoveSlot(Upgrade.UpgradeType.Modification);

                ActionBar.RemovePrintedAction(typeof(BoostAction));
                ActionBar.RemovePrintedAction(typeof(BarrelRollAction));

                ActionBar.AddPrintedAction(new EvadeAction());
                ActionBar.AddPrintedAction(new BoostAction() { LinkedRedAction = new FocusAction() { IsRed = true } });
                ActionBar.AddPrintedAction(new BarrelRollAction() { LinkedRedAction = new FocusAction() { IsRed = true } });

                IconicPilots[Faction.Imperial] = typeof(BaronOfTheEmpire);
            }

        }
    }
}
