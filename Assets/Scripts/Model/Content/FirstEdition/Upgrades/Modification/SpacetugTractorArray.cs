using BoardTools;
using Ship;
using Upgrade;
using SubPhases;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class SpacetugTractorArray : GenericUpgrade
    {
        public SpacetugTractorArray() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Spacetug Tractor Array",
                UpgradeType.Modification,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.SpacetugAbility),
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.Quadjumper.Quadjumper))
            );
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
                Host = HostShip,
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

            newPhase.SpacetugOwner = this.Host;
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
            ShotInfo shotInfo = new ShotInfo(SpacetugOwner, ship, SpacetugOwner.PrimaryWeapon);
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