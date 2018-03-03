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
            isLimited = false;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Ship.Quadjumper.Quadjumper;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);
            host.AfterGenerateAvailableActionsList += SpacetugAddAction;
        }

        private void SpacetugAddAction(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.SpacetugAction() { ImageUrl = ImageUrl, Host = this.Host };
            host.AddAvailableAction(newAction);
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
                delegate {}
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
            MovementTemplates.ReturnRangeRuler();

            Tokens.TractorBeamToken token = new Tokens.TractorBeamToken(TargetShip, SpacetugOwner.Owner);
            TargetShip.Tokens.AssignToken(token, delegate {
                Triggers.ResolveTriggers(TriggerTypes.OnActionIsPerformed, delegate {
                    Phases.FinishSubPhase(typeof(SelectSpacetugTargetSubPhase));
                    Phases.FinishSubPhase(typeof(ActionDecisonSubPhase));
                    Triggers.FinishTrigger();
                    CallBack();
                });	
            });
        }

        public override void SkipButton()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }
    }
}


