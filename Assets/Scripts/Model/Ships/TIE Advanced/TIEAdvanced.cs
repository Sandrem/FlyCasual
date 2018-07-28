using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;
using System.Linq;
using Abilities;
using Ship;

namespace Ship
{
    namespace TIEAdvanced
    {
        public class TIEAdvanced : GenericShip, TIE, ISecondEditionShip
        {

            public TIEAdvanced() : base()
            {
                Type = FullType = "TIE Advanced";
                IconicPilots.Add(Faction.Imperial, typeof(DarthVader));

                ManeuversImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/8/85/MI_TIE-ADVANCED.png";

                ShipIconLetter = 'A';

                Firepower = 2;
                Agility = 3;
                MaxHull = 3;
                MaxShields = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);

                ActionBar.AddPrintedAction(new EvadeAction());
                ActionBar.AddPrintedAction(new BarrelRollAction());
                ActionBar.AddPrintedAction(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEAdvancedTable();

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
                Maneuvers.Add("1.L.T", MovementComplexity.None);
                Maneuvers.Add("1.L.B", MovementComplexity.Easy);
                Maneuvers.Add("1.F.S", MovementComplexity.None);
                Maneuvers.Add("1.R.B", MovementComplexity.Easy);
                Maneuvers.Add("1.R.T", MovementComplexity.None);
                Maneuvers.Add("1.F.R", MovementComplexity.None);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Normal);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Normal);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.F.R", MovementComplexity.None);
                Maneuvers.Add("3.L.E", MovementComplexity.None);
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.R.E", MovementComplexity.None);
                Maneuvers.Add("3.F.R", MovementComplexity.None);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
                Maneuvers.Add("5.F.S", MovementComplexity.Normal);
                Maneuvers.Add("5.F.R", MovementComplexity.None);
            }

            public void AdaptShipToSecondEdition()
            {
                FullType = "TIE Advanced x1";

                ActionBar.RemovePrintedAction(typeof(FocusAction));
                ActionBar.RemovePrintedAction(typeof(EvadeAction));

                Maneuvers["1.F.S"] = MovementComplexity.Normal;
                Maneuvers["2.L.B"] = MovementComplexity.Easy;
                Maneuvers["2.R.B"] = MovementComplexity.Easy;
                Maneuvers["3.L.E"] = MovementComplexity.Complex;
                Maneuvers["3.R.E"] = MovementComplexity.Complex;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);

                ActionBar.AddPrintedAction(new FocusAction() { LinkedRedAction = new BarrelRollAction() { IsRed = true } });

                ShipAbilities.Add(new Abilities.SecondEdition.AdvancedTargetingComputer());
            }

        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform a primary attack against a defender you have locked, roll 1 additional attack die and change 1 hit result to a critical hit result.
    public class AdvancedTargetingComputer : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckAbility;
        }

        private void CheckAbility(ref int value)
        {
            if (Combat.AttackStep == CombatStep.Attack && Actions.HasTargetLockOn(Combat.Attacker, Combat.Defender) && Combat.ChosenWeapon.GetType() == typeof(PrimaryWeaponClass))
            {
                Messages.ShowInfo("Advanced Targeting Computer: +1 attack die");
                value++;
                HostShip.OnImmediatelyAfterRolling += ModifyDice;
            }
        }

        private void ModifyDice(DiceRoll diceroll)
        {
            HostShip.OnImmediatelyAfterRolling -= ModifyDice;
            if (diceroll.Change(DieSide.Success, DieSide.Crit, 1) > 0)
            {
                Messages.ShowInfo("Advanced Targeting Computer: 1 hit changed to crit");
            }
        }
    }
}
