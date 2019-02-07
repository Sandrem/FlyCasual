using Actions;
using ActionsList;
using BoardTools;
using Movement;
using Editions;
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
            newPhase.HostAction = this;
            newPhase.SpacetugOwner = this.HostShip;
            newPhase.Start();
        }

        public override void RevertActionOnFail(bool hasSecondChance = false)
        {
            if (hasSecondChance)
            {
                UI.ShowSkipButton();
                UI.HighlightSkipButton();
            }
            else
            {
                Phases.GoBack();
            }
        }
    }
}

namespace SubPhases
{
    public class SelectSpacetugTargetSubPhaseSE : SelectShipSubPhase
    {
        public GenericAction HostAction { get; set; }

        public GenericShip SpacetugOwner;

        public override void Prepare()
        {
            PrepareByParameters(
                SelectSpacetugTarget,
                FilterAbilityTargets,
                GetAiAbilityPriority,
                Selection.ThisShip.Owner.PlayerNo,
                false,
                "Spacetug Tractor Array",
                "Choose a ship inside your firing arc at range 1 to assign a tractor beam token to it.",
                SpacetugOwner
            );
        }

        protected override void CancelShipSelection()
        {
            Rules.Actions.ActionIsFailed(TheShip, HostAction, ActionFailReason.WrongRange, true);
        }

        public override void SkipButton()
        {
            Rules.Actions.ActionIsFailed(TheShip, HostAction, ActionFailReason.WrongRange, false);
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(SpacetugOwner, ship, SpacetugOwner.PrimaryWeapons);
            if (shotInfo.InArc && shotInfo.Range == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
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
            CancelShipSelection();
        }

        private void AssignSecondTractorBeamToken()
        {
            Messages.ShowInfo("Spacetug Tractor Array: Ship was in bullseye arc, second Tractor Beam token is assigned");

            TractorBeamToken token = new TractorBeamToken(TargetShip, SpacetugOwner.Owner);
            TargetShip.Tokens.AssignToken(token, CallBack);
        }
    }
}