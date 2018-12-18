using Actions;
using ActionsList;
using BoardTools;
using Movement;
using RuleSets;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace SecondEdition.QuadrijetTransferSpacetug
    {
        public class QuadrijetTransferSpacetug : FirstEdition.Quadjumper.Quadjumper
        {
            public QuadrijetTransferSpacetug() : base()
            {
                ShipInfo.ShipName = "Quadrijet Transfer Spacetug";

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(EvadeAction), ActionColor.Red));

                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Reverse));
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Reverse), MovementComplexity.Complex);

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Normal);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Normal);

                IconicPilots = new Dictionary<Faction, System.Type> {
                    { Faction.Scum, typeof(JakkuGunrunner) }
                };

                ShipAbilities.Add(new Abilities.SecondEdition.SpacetugAbility());

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/6/64/Maneuver_quadrijet.png";

                OldShipTypeName = "Quadjumper";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SpacetugAbility : GenericAbility
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
                HostShip = HostShip
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

            newPhase.SpacetugOwner = this.HostShip;
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
            ShotInfo shotInfo = new ShotInfo(SpacetugOwner, TargetShip, SpacetugOwner.PrimaryWeapons);
            if (shotInfo.InArc && shotInfo.Range == 1)
            {
                SelectShipSubPhase.FinishSelectionNoCallback();

                MovementTemplates.ReturnRangeRuler();

                Messages.ShowInfo("Spacetug Tractor Array: Tractor Beam token is assigned to " + TargetShip.PilotInfo.PilotName);

                TractorBeamToken token = new TractorBeamToken(TargetShip, SpacetugOwner.Owner);

                if (SpacetugOwner.SectorsInfo.IsShipInSector(TargetShip, Arcs.ArcType.Bullseye) && SpacetugOwner.SectorsInfo.RangeToShipBySector(TargetShip, Arcs.ArcType.Bullseye) == 1)
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
            Edition.Current.ActionIsFailed(TheShip, typeof(SpacetugActionSE));
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