using Ship;
using Upgrade;
using UnityEngine;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class Chewbacca : GenericUpgrade
    {
        public Chewbacca() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Chewbacca",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.FirstEdition.ChewbaccaCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(66, 1));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ChewbaccaCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDamageCardIsDealt += RegisterChewbaccaCrewTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDamageCardIsDealt -= RegisterChewbaccaCrewTrigger;
        }

        private void RegisterChewbaccaCrewTrigger(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Chewbacca's ability",
                TriggerType = TriggerTypes.OnDamageCardIsDealt,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = AskUseChewbaccaCrewAbility,
                Sender = ship
            });
        }

        private void AskUseChewbaccaCrewAbility(object sender, System.EventArgs e)
        {
            GenericShip previousShip = Selection.ActiveShip;
            Selection.ActiveShip = sender as GenericShip;

            AskToUseAbility(
                AiForChewbaccaCrewAbility,
                UseChewbaccaCrewAbility,
                null,
                delegate {
                    Selection.ActiveShip = previousShip;
                    Triggers.FinishTrigger();
                }
            );
        }

        private bool AiForChewbaccaCrewAbility()
        {
            bool result = false;

            if (HostShip.State.HullCurrent == 1 || Combat.CurrentCriticalHitCard.AiAvoids) result = true;

            return result;
        }

        private void UseChewbaccaCrewAbility(object sender, System.EventArgs e)
        {
            Sounds.PlayShipSound("Chewbacca");
            Messages.ShowInfo("Chewbacca (crew) has been used");

            Combat.CurrentCriticalHitCard = null;
            if (Selection.ActiveShip.TryRegenShields())
            {
                Messages.ShowInfo("1 shield has been restored");
            }
            else
            {
                Messages.ShowInfo("No shields could be restored");
            }
            HostUpgrade.TryDiscard(DecisionSubPhase.ConfirmDecision);
        }

    }
}