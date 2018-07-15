using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace ProtectorateStarfighter
    {
        public class ProtectorateStarfighter : GenericShip, ISecondEditionShip
        {

            public ProtectorateStarfighter() : base()
            {
                Type = "Protectorate Starfighter";
                IconicPilots.Add(Faction.Scum, typeof(ConcordDawnAce));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/8/83/MS_PROTECTORATE-STARFIGHTER.png";

                ShipIconLetter = 'M';

                Firepower = 3;
                Agility = 3;
                MaxHull = 4;
                MaxShields = 0;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new BarrelRollAction());
                ActionBar.AddPrintedAction(new BoostAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.ProtectorateStarfighterTable();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Protectorate Starfighter";

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
                Maneuvers.Add("1.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.T", MovementComplexity.Easy);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Easy);
                Maneuvers.Add("2.L.E", MovementComplexity.Complex);
                Maneuvers.Add("2.R.E", MovementComplexity.Complex);
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
                Maneuvers.Add("5.F.S", MovementComplexity.Normal);
            }

            public void AdaptShipToSecondEdition()
            {
                IconicPilots[Faction.Scum] = typeof(FennRau);

                ShipAbilities.Add(new Abilities.SecondEdition.ConcordiaFaceoffAbility());

                ActionBar.RemovePrintedAction(typeof(BarrelRollAction));
                ActionBar.RemovePrintedAction(typeof(BoostAction));
                ActionBar.AddPrintedAction(new BarrelRollAction() { LinkedRedAction = new FocusAction() { IsRed = true } });
                ActionBar.AddPrintedAction(new BoostAction() { LinkedRedAction = new FocusAction() { IsRed = true } });
            }

        }
    }
}

namespace Abilities.SecondEdition
{
    //While you defend, if the attack range is 1 and you are in the attacker's forward firing arc, change 1 result to an evade result.
    public class ConcordiaFaceoffAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Concordia Faceoff",
                IsAvailable,
                AiPriority,
                DiceModificationType.Change,
                1,
                sideCanBeChangedTo: DieSide.Success
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsAvailable()
        {
            return
            (
                Combat.AttackStep == CombatStep.Defence &&
                Combat.Defender == HostShip &&
                Combat.ShotInfo.Range == 1 &&
                Combat.ShotInfo.InPrimaryArc
            );
        }

        public int AiPriority()
        {
            //TODO: Change to enum
            return 100;
        }
    }
}
