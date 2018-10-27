using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;
using Ship;
using SubPhases;
using BoardTools;
using Tokens;

namespace Ship
{
    namespace Quadjumper
    {
        public class Quadjumper : GenericShip, ISecondEditionShip
        {

            public Quadjumper() : base()
            {
                Type = FullType = "Quadjumper";
                IconicPilots.Add(Faction.Scum, typeof(JakkuGunrunner));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/4/4d/MS_QUADJUMPER.png";

                Firepower = 2;
                Agility = 2;
                MaxHull = 5;
                MaxShields = 0;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Bomb);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Tech);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                ActionBar.AddPrintedAction(new BarrelRollAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.QuadjumperTable();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Quadjumper";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 2;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.F.V", MovementComplexity.Complex);
                Maneuvers.Add("1.L.V", MovementComplexity.Complex);
                Maneuvers.Add("1.R.V", MovementComplexity.Complex);
                Maneuvers.Add("1.L.T", MovementComplexity.Normal);
                Maneuvers.Add("1.F.S", MovementComplexity.Normal);
                Maneuvers.Add("1.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.R", MovementComplexity.Complex);
                Maneuvers.Add("2.R.R", MovementComplexity.Complex);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
            }

            public void AdaptShipToSecondEdition()
            {
                ShipAbilities.Add(new Abilities.SecondEdition.SpacetugAbilitySE());

                FullType = "Quadrijet Transfer Spacetug";

                Maneuvers.Remove("1.F.V");
                Maneuvers.Add("2.F.V", MovementComplexity.Complex);
                Maneuvers.Add("1.L.B", MovementComplexity.Normal);
                Maneuvers.Add("1.R.B", MovementComplexity.Normal);

                ActionBar.AddPrintedAction(new EvadeAction() { IsRed = true });
            }

        }
    }
}

namespace Abilities.SecondEdition
{
    public class SpacetugAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += AddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= AddAction;
        }

        private void AddAction(GenericShip ship)
        {
            GenericAction newAction = new SpacetugActionSE()
            {
                ImageUrl = HostShip.ImageUrl,
                Host = HostShip
            };
            HostShip.AddAvailableAction(newAction);
        }
    }
}

namespace ActionsList
{
    public class SpacetugActionSE : SpacetugAction
    {
        public override void ActionTake()
        {
            SelectSpacetugTargetSubPhaseSE newPhase = (SelectSpacetugTargetSubPhaseSE)Phases.StartTemporarySubPhaseNew(
                "Select target for Spacetug Tractor Array",
                typeof(SelectSpacetugTargetSubPhaseSE),
                Phases.CurrentSubPhase.CallBack
            );

            newPhase.SpacetugOwner = this.Host;
            newPhase.Start();
        }
    }
}

namespace SubPhases
{
    public class SelectSpacetugTargetSubPhaseSE : SelectShipSubPhase
    {
        public GenericShip SpacetugOwner;

        public override void Prepare()
        {
            PrepareByParameters(
                SelectSpacetugTarget,
                FilterAbilityTargets,
                GetAiAbilityPriority,
                Selection.ThisShip.Owner.PlayerNo,
                true,
                "Spacetug Tractor Array",
                "Choose a ship inside your firing arc at range 1 to assign a tractor beam token to it.",
                SpacetugOwner
            );
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            return true; // Check in "SelectSpacetugTarget" to handle wrong target
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            // TODO, calculate inverse distance to closest rock
            return 0;
        }

        private void SelectSpacetugTarget()
        {
            ShotInfo shotInfo = new ShotInfo(SpacetugOwner, TargetShip, SpacetugOwner.PrimaryWeapon);
            if (shotInfo.InArc && shotInfo.Range == 1)
            {
                SelectShipSubPhase.FinishSelectionNoCallback();

                MovementTemplates.ReturnRangeRuler();

                Messages.ShowInfo("Spacetug Tractor Array: Tractor Beam token is assigned to " + TargetShip.PilotName);

                TractorBeamToken token = new TractorBeamToken(TargetShip, SpacetugOwner.Owner);

                ShotInfoArc shotInfoBullseye = new ShotInfoArc(SpacetugOwner, TargetShip, SpacetugOwner.ArcInfo.GetArc<Arcs.ArcBullseye>());
                if (shotInfoBullseye.InArc && shotInfoBullseye.Range == 1)
                {
                    TargetShip.Tokens.AssignToken(token, AssignSecondTractorBeamToken);
                }
                else
                {
                    TargetShip.Tokens.AssignToken(token, CallBack);
                }
            }
            else
            {
                RevertSubPhase();
            }
        }

        public override void RevertSubPhase()
        {
            RuleSet.Instance.ActionIsFailed(TheShip, typeof(SpacetugActionSE));
            UpdateHelpInfo();
        }

        private void AssignSecondTractorBeamToken()
        {
            Messages.ShowInfo("Spacetug Tractor Array: Ship was in bullseye arc, second Tractor Beam token is assigned");

            TractorBeamToken token = new TractorBeamToken(TargetShip, SpacetugOwner.Owner);
            TargetShip.Tokens.AssignToken(token, CallBack);
        }

        public override void SkipButton()
        {
            SelectShipSubPhase.FinishSelection();
        }
    }
}
