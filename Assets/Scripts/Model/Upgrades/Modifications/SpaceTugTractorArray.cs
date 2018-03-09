using Board;
using Ship;
using Upgrade;
using SubPhases;
using System.Collections.Generic;

namespace UpgradesList
{
    public class SpacetugTractorArray : GenericUpgradeSlotUpgrade
    {
        public SpacetugTractorArray() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Spacetug Tractor Array";
            Cost = 2;
            UpgradeAbilities.Add (new Abilities.SpacetugAbility ());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Ship.Quadjumper.Quadjumper;
        }
    }
}

namespace Abilities
{
    public class SpacetugAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList += AddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList -= AddAction;
        }

        private void AddAction(GenericShip ship)
        {
            ActionsList.GenericAction newAction = new ActionsList.SpacetugAction() 
            { 
                ImageUrl = HostUpgrade.ImageUrl, 
                Host = HostShip 
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
            Name = EffectName = "Spacetug Tractor Array";
        }

        public override void ActionTake()
        {
            SelectSpacetugTargetSubPhase newPhase = (SelectSpacetugTargetSubPhase) Phases.StartTemporarySubPhaseNew(
                "Select target for Spacetug Tractor Array",
                typeof(SelectSpacetugTargetSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(ActionSubPhase));
                }
            );

            newPhase.SpacetugOwner = this.Host;
            newPhase.Start();
        }
    }
}


namespace SubPhases
{
    public class SelectSpacetugTargetSubPhase : SelectShipSubPhase
    {
        public GenericShip SpacetugOwner;

        public override void Prepare()
        {
            targetsAllowed.Add(TargetTypes.Enemy);
            targetsAllowed.Add(TargetTypes.OtherFriendly);
            maxRange = 1;
            finishAction = SelectSpacetugTarget;

            FilterTargets = FilterAbilityTargets;
            GetAiPriority = GetAiAbilityPriority;

            UI.ShowSkipButton();
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            ShipShotDistanceInformation shotInfo = new ShipShotDistanceInformation(SpacetugOwner, ship);
            return shotInfo.InArc && shotInfo.Range == 1;
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            // TODO, calculate inverse distance to closest rock
            return 0;
        }

        private void SelectSpacetugTarget()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();
            MovementTemplates.ReturnRangeRuler();
            Tokens.TractorBeamToken token = new Tokens.TractorBeamToken(TargetShip, SpacetugOwner.Owner);
            TargetShip.Tokens.AssignToken(token, Triggers.FinishTrigger);
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            Phases.CurrentSubPhase.Next();
            UpdateHelpInfo();
            CallBack();
        }

        public override void SkipButton()
        {
            Phases.FinishSubPhase(this.GetType());
        }
    }
}


