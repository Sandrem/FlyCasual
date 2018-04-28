using Ship;
using System;
using UnityEngine;
using Upgrade;
using Abilities;
using SubPhases;

namespace UpgradesList
{
    public class Chewbacca : GenericUpgrade
    {
        public Chewbacca() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Chewbacca";
            Cost = 4;

            isUnique = true;

            AvatarOffset = new Vector2(66, 1);

            UpgradeAbilities.Add(new ChewbaccaAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
{
    public class ChewbaccaAbility : GenericAbility
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

            if (HostShip.Hull == 1 || Combat.CurrentCriticalHitCard.AiAvoids) result = true;

            return result;
        }

        private void UseChewbaccaCrewAbility(object sender, System.EventArgs e)
        {
            Sounds.PlayShipSound("Chewbacca");
            Messages.ShowInfo("Chewbacca (crew) is used");

            Combat.CurrentCriticalHitCard = null;
            if (Selection.ActiveShip.TryRegenShields()) Messages.ShowInfo("Shield is restored");

            HostUpgrade.TryDiscard(DecisionSubPhase.ConfirmDecision);
        }

    }
}
