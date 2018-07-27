using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;
using Ship;
using System.Linq;
using Tokens;
using Arcs;

namespace Ship
{
    namespace M12LKimogila
    {
        public class M12LKimogila : GenericShip, ISecondEditionShip
        {

            public M12LKimogila() : base()
            {
                Type = "M12-L Kimogila Fighter";
                IconicPilots.Add(Faction.Scum, typeof(ToraniKulda));

                ShipBaseArcsType = BaseArcsType.ArcBullseye;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/e/e7/Screenshot_2017-12-15_at_1.31.03_PM.png";

                Firepower = 3;
                Agility = 1;
                MaxHull = 6;
                MaxShields = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.SalvagedAstromech);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                ActionBar.AddPrintedAction(new TargetLockAction());
                ActionBar.AddPrintedAction(new BarrelRollAction());
                ActionBar.AddPrintedAction(new ReloadAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.M12LKimogilaTable();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Hutt Cartel";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 3;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.Complex);
                Maneuvers.Add("1.L.B", MovementComplexity.Normal);
                Maneuvers.Add("1.F.S", MovementComplexity.Easy);
                Maneuvers.Add("1.R.B", MovementComplexity.Normal);
                Maneuvers.Add("1.R.T", MovementComplexity.Complex);
                Maneuvers.Add("2.L.T", MovementComplexity.Complex);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Complex);
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                ShipBaseArcsType = BaseArcsType.ArcDefault;

                Maneuvers["2.L.T"] = MovementComplexity.Normal;
                Maneuvers["2.R.T"] = MovementComplexity.Normal;

                MaxHull = 7;

                ShipBaseSize = BaseSize.Medium;

                ActionBar.RemovePrintedAction(typeof(BarrelRollAction));
                ActionBar.AddPrintedAction(new BarrelRollAction() { IsRed = true });

                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.SalvagedAstromech);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);

                ShipAbilities.Add(new Abilities.SecondEdition.DeadToRights());

                IconicPilots[Faction.Scum] = typeof(CartelExecutioner);
            }

        }
    }
}

namespace Abilities.SecondEdition
{
    public class DeadToRights : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnTryAddAvailableDiceModificationGlobal += CheckBullseyeArc;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryAddAvailableDiceModificationGlobal -= CheckBullseyeArc;
        }

        private List<System.Type> TokensForbidden = new List<System.Type>()
        {
            typeof(FocusToken),
            typeof(EvadeToken),
            typeof(ReinforceAftToken),
            typeof(ReinforceForeToken),
        };

        public void CheckBullseyeArc(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (action.TokensSpend.Any(t => TokensForbidden.Contains(t)))
            {
                if (Combat.AttackStep == CombatStep.Defence && Combat.Defender.ShipId == ship.ShipId)
                {
                    if (Combat.Attacker.ShipId == HostShip.ShipId)
                    {
                        if (Combat.ShotInfo.InArcByType(ArcTypes.Bullseye))
                        {
                            Messages.ShowError("Bullseye: " + action.DiceModificationName + " cannot be used");
                            canBeUsed = false;
                        }
                    }
                }
            }
        }
    }
}
