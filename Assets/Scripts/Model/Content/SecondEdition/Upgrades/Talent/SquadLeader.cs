using Upgrade;
using ActionsList;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class SquadLeader : GenericUpgrade
    {
        public SquadLeader() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Squad Leader",
                UpgradeType.Talent,
                cost: 6,
                isLimited: true,
                addAction: new ActionInfo(typeof(CoordinateAction), ActionColor.Red),
                //abilityType: typeof(Abilities.SecondEdition.SquadLeaderAbility),
                seImageNumber: 16
            );
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