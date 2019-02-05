using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;
using SubPhases;
using ActionsList;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class SquadLeader : GenericUpgrade, IVariableCost
    {
        public SquadLeader() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Squad Leader",
                UpgradeType.Talent,
                cost: 4,
                isLimited: true,
                addAction: new ActionInfo(typeof(CoordinateAction), ActionColor.Red),
                //abilityType: typeof(Abilities.SecondEdition.SquadLeaderAbility),
                seImageNumber: 16
            );
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> initiativeToCost = new Dictionary<int, int>()
            {
                {0, 2},
                {1, 4},
                {2, 6},
                {3, 8},
                {4, 10},
                {5, 12},
                {6, 14}
            };

            UpgradeInfo.Cost = initiativeToCost[ship.PilotInfo.Initiative];
        }
    }
}

// TODO: Change ability

/*namespace Abilities.SecondEdition
{
    //While you coordinate, the ship you choose can perform an action only if that action is also on your action bar.
    public class SquadLeaderAbility : GenericAbility
    {
        public override void ActivateAbility() { }

        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.AddGrantedAction(new SquadLeaderCoordinateAction() { IsRed = true }, HostUpgrade);
        }

        public override void DeactivateAbility() { }

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.RemoveGrantedAction(typeof(SquadLeaderCoordinateAction), HostUpgrade);
        }

        public class SquadLeaderCoordinateAction : CoordinateAction
        {
            public override void ActionTake()
            {
                var phase = Phases.StartTemporarySubPhaseNew<SquadLeaderTargetSubPhase>(
                    "Select target for Coordinate",
                    Phases.CurrentSubPhase.CallBack
                );
                phase.HostShip = Host;
                phase.Start();
            }
        }

        public class SquadLeaderTargetSubPhase : CoordinateTargetSubPhase
        {
            public GenericShip HostShip;

            protected override List<GenericAction> GetPossibleActions()
            {
                var targetActions = Selection.ThisShip.GetAvailableActions();
                return targetActions.Where(a => HostShip.ActionBar.HasAction(a.GetType())).ToList();
            }
        }
    }
}*/