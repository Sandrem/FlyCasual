using Ship;
using Upgrade;
using System.Linq;
using BoardTools;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class GrandInquisitor : GenericUpgrade
    {
        public GrandInquisitor() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Grand Inquisitor",
                UpgradeType.Crew,
                cost: 13,
                addForce: 1,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.SecondEdition.GrandInquisitorCrewAbility),
                seImageNumber: 116
            );
        }
    }
}
namespace Abilities.SecondEdition
{
    public class GrandInquisitorCrewAbility : GenericAbility
    {
        GenericShip PreviousShip;

        public override void ActivateAbility()
        {
            GenericShip.OnManeuverIsRevealedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnManeuverIsRevealedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo) return;
            if (HostShip.State.Force == 0) return;
            //skip if crew is owned by the AI, because it is hard to calculate correct priority of red action
            if (HostShip.Owner.PlayerType == Players.PlayerType.Ai) return;

            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            if (distInfo.Range > 2) return;

            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskToUseCrewAbility);
        }

        private void AskToUseCrewAbility(object sender, System.EventArgs e)
        {
            PreviousShip = Selection.ThisShip;
            Selection.ChangeActiveShip(HostShip);
            Phases.CurrentSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;

            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": You may spend 1 Force to perform an action");

            HostShip.BeforeActionIsPerformed += SpendForce;
            HostShip.OnActionIsPerformed += RemoveSpendForce;

            HostShip.AskPerformFreeAction(
                HostShip.GetAvailableActionsWhiteOnlyAsRed().Where(a => HostShip.ActionBar.HasAction(a.GetType())).ToList(),
                FinishAbility,
                HostUpgrade.UpgradeInfo.Name,
                "After an enemy ship at range 0-2 reveals its dial, you may spend 1 Force to perform 1 white action on your action bar, treating that action as red",
                HostUpgrade
            );
        }

        private void SpendForce(GenericAction action, ref bool isFreeAction)
        {
            HostShip.State.Force--;
        }

        private void RemoveSpendForce(GenericAction action)
        {
            HostShip.BeforeActionIsPerformed -= SpendForce;
            HostShip.OnActionIsPerformed -= RemoveSpendForce;
        }

        private void FinishAbility()
        {
            RemoveSpendForce(null);
            Selection.ChangeActiveShip(PreviousShip);
            Phases.CurrentSubPhase.RequiredPlayer = PreviousShip.Owner.PlayerNo;

            Triggers.FinishTrigger();
        }
    }
}