using Actions;
using ActionsList;
using BoardTools;
using Movement;
using Editions;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Arcs;
using Ship.CardInfo;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.QuadrijetTransferSpacetug
    {
        public class QuadrijetTransferSpacetug : GenericShip
        {
            public QuadrijetTransferSpacetug() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "Quadrijet Transfer Spacetug",
                    BaseSize.Small,
                    new FactionData
                    (
                        new Dictionary<Faction, System.Type>
                        {
                            { Faction.Scum, typeof(ConstableZuvio) }
                        }
                    ),
                    new ShipArcsInfo(ArcType.Front, 2), 2, 5, 0,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(BarrelRollAction)),
                        new ActionInfo(typeof(EvadeAction), ActionColor.Red)
                    ),
                    new ShipUpgradesInfo(),
                    legality: new List<Content.Legality>() { Content.Legality.ExtendedLegal }
                );

                ModelInfo = new ShipModelInfo
                (
                    "Quadjumper",
                    "Quadjumper",
                    new Vector3(-4f, 8f, 5.55f),
                    1.75f
                );

                DialInfo = new ShipDialInfo
                (
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.ReverseStraight, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.ReverseStraight, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.ReverseStraight, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal)
                );

                SoundInfo = new ShipSoundInfo
                (
                    new List<string>()
                    {
                        "XWing-Fly1",
                        "XWing-Fly2",
                        "XWing-Fly3"
                    },
                    "XWing-Laser", 2
                );

                ShipIconLetter = 'q';

                ShipAbilities.Add(new Abilities.SecondEdition.SpacetugAbility());
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SpacetugAbility : GenericAbility
    {
        public override string Name { get { return "Spacetug Tractor Array"; } }

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

                Messages.ShowInfo("Spacetug Tractor Array: " + SpacetugOwner.PilotInfo.PilotName + " has assigned a Tractor Beam token to " + TargetShip.PilotInfo.PilotName);

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
            Messages.ShowInfo(TargetShip.PilotInfo.PilotName + " was in " + SpacetugOwner.PilotInfo.PilotName + "'s Bullseye arc: a second Tractor Beam token has been assigned");

            TractorBeamToken token = new TractorBeamToken(TargetShip, SpacetugOwner.Owner);
            TargetShip.Tokens.AssignToken(token, CallBack);
        }
    }
}

namespace Abilities.FirstEdition
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
            ActionsList.GenericAction newAction = new ActionsList.SpacetugAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = HostShip,
                Source = HostUpgrade
            };
            HostShip.AddAvailableAction(newAction);
        }
    }
}

namespace ActionsList
{

    public class SpacetugAction : GenericAction
    {

        public SpacetugAction()
        {
            Name = DiceModificationName = "Spacetug Tractor Array";
        }

        public override void ActionTake()
        {
            SelectSpacetugTargetSubPhase newPhase = (SelectSpacetugTargetSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select target for Spacetug Tractor Array",
                typeof(SelectSpacetugTargetSubPhase),
                FinishAction
            );

            newPhase.SpacetugOwner = this.HostShip;
            newPhase.SpacetugUpgrade = this.Source;
            newPhase.Start();
        }

        private void FinishAction()
        {
            Phases.CurrentSubPhase.CallBack();
        }
    }
}


namespace SubPhases
{
    public class SelectSpacetugTargetSubPhase : SelectShipSubPhase
    {
        public GenericShip SpacetugOwner;
        public GenericUpgrade SpacetugUpgrade;

        public override void Prepare()
        {
            PrepareByParameters(
                SelectSpacetugTarget,
                FilterAbilityTargets,
                GetAiAbilityPriority,
                Selection.ThisShip.Owner.PlayerNo,
                true,
                SpacetugUpgrade.UpgradeInfo.Name,
                "Choose a ship inside your firing arc to assign a tractor beam token to it.",
                SpacetugUpgrade
            );
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(SpacetugOwner, ship, SpacetugOwner.PrimaryWeapons);
            return shotInfo.InArc && shotInfo.Range == 1;
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            // TODO, calculate inverse distance to closest rock
            return 0;
        }

        private void SelectSpacetugTarget()
        {
            MovementTemplates.ReturnRangeRuler();
            TractorBeamToken token = new TractorBeamToken(TargetShip, SpacetugOwner.Owner);
            TargetShip.Tokens.AssignToken(token, SelectShipSubPhase.FinishSelection);
        }

        public override void SkipButton()
        {
            Phases.FinishSubPhase(typeof(SelectSpacetugTargetSubPhase));
            CallBack();
        }
    }
}