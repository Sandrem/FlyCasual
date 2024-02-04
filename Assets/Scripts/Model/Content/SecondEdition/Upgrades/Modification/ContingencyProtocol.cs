using System;
using ActionsList;
using BoardTools;
using Ship;
using Upgrade;
using UpgradesList.SecondEdition;

namespace UpgradesList.SecondEdition
{
    public class ContingencyProtocol : GenericUpgrade
    {
        public ContingencyProtocol() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo
            (
                "Contingency Protocol",
                UpgradeType.Modification,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.ContingencyProtocolAbility)
            );

            ImageUrl = "https://i.imgur.com/5MMMAtf.jpg";
        }
        
    }
}

namespace Abilities.SecondEdition
{
    public class ContingencyProtocolAbility : GenericAbility
    {
        GenericShip PreviousActiveShip = null;

        public override void ActivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, bool flag)
        {
            if (Tools.IsSameTeam(HostShip, ship)
                && ship.UpgradeBar.HasUpgradeInstalled(typeof(ContingencyProtocol)))
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                if (distInfo.Range >= 0 && distInfo.Range <= 3)
                {
                    RegisterAbilityTrigger
                    (
                        TriggerTypes.OnShipIsDestroyed,
                        ActivateContingencyProtocol,
                        customTriggerName: $"{HostShip.PilotInfo.PilotName}: {HostUpgrade.UpgradeInfo.Name}"
                    );
                }
            }
        }

        private void ActivateContingencyProtocol(object sender, EventArgs e)
        {
            PreviousActiveShip = Selection.ActiveShip;
            Selection.ChangeActiveShip(HostShip);

            HostShip.OnCanPerformActionWhileStressed += AllowActionsWhileStressed;

            HostShip.AskPerformFreeAction
            (
                Selection.ThisShip.GetAvailableActions(),
                FinishAbility,
                descriptionShort: HostUpgrade.UpgradeInfo.Name,
                descriptionLong: "You may perform actionm even while stressed"
            );
        }

        private void AllowActionsWhileStressed(GenericAction action, ref bool isAllowed)
        {
            isAllowed = true;
        }

        private void FinishAbility()
        {
            HostShip.OnCanPerformActionWhileStressed -= AllowActionsWhileStressed;

            Selection.ChangeActiveShip(PreviousActiveShip);

            Triggers.FinishTrigger();
        }
    }
}
