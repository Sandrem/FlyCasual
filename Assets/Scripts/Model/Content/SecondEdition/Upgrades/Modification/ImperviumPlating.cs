using Upgrade;
using Ship;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class ImperviumPlating : GenericUpgrade
    {
        public ImperviumPlating() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Impervium Plating",
                UpgradeType.Modification,
                cost: 0, //TODO
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.Belbullab22Starfighter.Belbullab22Starfighter)),
                abilityType: typeof(Abilities.SecondEdition.ImperviumPlatingAbility),
                charges: 2
                //seImageNumber: ??
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/10/67/10676484-b596-43fd-a218-2d85707cf476/swz29_impervium-plating.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //Before you would be dealt a faceup Ship damage card, you may spend 1 charge to discard it instead.
    public class ImperviumPlatingAbility : GenericAbility 
    {
        public override void ActivateAbility()
        {
            HostShip.OnDamageCardIsDealt += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDamageCardIsDealt -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            if (HostUpgrade.State.Charges > 0 && Combat.CurrentCriticalHitCard.IsFaceup && Combat.CurrentCriticalHitCard.Type == CriticalCardType.Ship)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = HostName,
                    TriggerType = TriggerTypes.OnDamageCardIsDealt,
                    TriggerOwner = ship.Owner.PlayerNo,
                    EventHandler = AskUseAbility,
                    Sender = ship
                });
            }
        }

        private void AskUseAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                AlwaysUseByDefault,
                DiscardDamage,
                null,
                Triggers.FinishTrigger
            );
        }

        private void DiscardDamage(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostName + " discards " + Combat.CurrentCriticalHitCard.Name);
            Combat.CurrentCriticalHitCard = null;
            HostUpgrade.State.SpendCharge();
            DecisionSubPhase.ConfirmDecision();
        }
    }
}