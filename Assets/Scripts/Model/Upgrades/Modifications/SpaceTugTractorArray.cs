using Board;
using Ship;
using Upgrade;
using SubPhases;
using System.Collections.Generic;
using UnityEngine;

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
                FinishAction
            );

            newPhase.SpacetugOwner = this.Host;
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
                SpacetugUpgrade.Name,
                "Choose a ship inside your firing arc to assign a tractor beam token to it.",
                SpacetugUpgrade.ImageUrl
            );
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
            TargetShip.Tokens.AssignToken(token, SelectShipSubPhase.FinishSelection);
        }

    }
}


