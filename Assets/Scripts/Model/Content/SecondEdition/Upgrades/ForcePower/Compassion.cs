using Upgrade;
using Ship;
using System;

namespace UpgradesList.SecondEdition
{
    public class Compassion : GenericUpgrade
    {
        public Compassion() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Compassion",
                UpgradeType.ForcePower,
                cost: 1,
                restriction: new TagRestriction(Content.Tags.LightSide),
                abilityType: typeof(Abilities.SecondEdition.CompassionAbility)       
            );

            ImageUrl = "https://i.imgur.com/AcRepjE.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CompassionAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnDamageCardIsDealtGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnDamageCardIsDealtGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (HostShip.State.Force > 0
                && Combat.CurrentCriticalHitCard.IsFaceup
                && Combat.CurrentCriticalHitCard.Type == CriticalCardType.Pilot
                && Tools.IsAnotherFriendly(HostShip, ship)
                && BoardTools.Board.IsShipBetweenRange(HostShip, ship, 0, 2))
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = HostName,
                    TriggerType = TriggerTypes.OnDamageCardIsDealt,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = AskCompassionAbility,
                    Sender = HostShip
                });
            }
        }

        private void AskCompassionAbility(object sender, EventArgs e)
        {
            GenericShip previousShip = Selection.ActiveShip;
            Selection.ActiveShip = sender as GenericShip;

            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                UseCompassionAbility,
                null,
                delegate {
                    Selection.ActiveShip = previousShip;
                    Triggers.FinishTrigger();
                },
                descriptionLong: "Do you want to spend 1 force token to discard the Pilot damage card and instead deal 1 facedown damager card to yourself? Then, if you have two or more damage cards, recover 2 Force",
                imageHolder: HostShip
            );
        }

        private void UseCompassionAbility(object sender, EventArgs e)
        {
            
            HostShip.Damage.SufferFacedownDamageCard(
                new DamageSourceEventArgs()
                {
                    Source = HostShip,
                    DamageType = DamageTypes.CardAbility
                },
                delegate { }
            );
            Combat.CurrentCriticalHitCard = null;
            HostShip.State.SpendForce(1, delegate {
                if (HostShip.Damage.CountAssignedDamage() >= 2)
                {
                    HostShip.State.RestoreForce(2);
                }
            });
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }
    }
}