using Abilities.Parameters;
using ActionsList;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ProtectorateGleb : GenericUpgrade
    {
        public ProtectorateGleb() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Protectorate Gleb",
                UpgradeType.Crew,
                cost: 6,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum, Faction.Imperial, Faction.FirstOrder),
                addAction: new Actions.ActionInfo(typeof(CoordinateAction), Actions.ActionColor.Red),
                abilityType: typeof(Abilities.SecondEdition.ProtectorateGlebAbility)
            );

            ImageUrl = "https://i.imgur.com/tWQx5NL.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    //After you coordinate, you may transfer 1 orange or red token to the ship you coordinated.
    public class ProtectorateGlebAbility : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new AfterAction(typeof(CoordinateAction));

        public override AbilityPart Action => new SelectToken
        (
            abilityDescription: new AbilityDescription
            (
                HostUpgrade.UpgradeInfo.Name,
                "You may transfer 1 orange or red token to the ship you coordinated",
                HostUpgrade
            ),
            colorsFilter: new List<TokenColors>() {TokenColors.Orange, TokenColors.Red },
            decisionOwner: HostShip.Owner,
            next: new TransferToken
            (
                target: ShipRole.CoordinatedShip,
                showMessage: ShowTransferTokenMessage
            )
        );

        private string ShowTransferTokenMessage()
        {
            return $"{HostUpgrade.UpgradeInfo.Name}: {TargetToken.Name} is transfered to {HostShip.State.LastCoordinatedShip.PilotInfo.PilotName}";
        }
    }
}