using Ship;
using Upgrade;

namespace UpgradesList
{
    public class Chewbacca : GenericUpgrade
    {
        public Chewbacca() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Chewbacca";
            Cost = 4;

            isUnique = true;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            Host.OnDamageCardIsDealt += RegisterChewbaccaCrewTrigger;
        }

        private void RegisterChewbaccaCrewTrigger(GenericShip ship, CriticalHitCard.GenericCriticalHit damageCard, System.EventArgs e = null)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Chewbacca's ability",
                TriggerType = TriggerTypes.OnDamageCardIsDealt,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = UseChewbaccaCrewAbility,
                EventArgs = new ChewbaccaCrewEventArgs()
                {
                    ship = ship,
                    damageCard = damageCard
                }
            });
        }

        private class ChewbaccaCrewEventArgs: System.EventArgs
        {
            public GenericShip ship;
            public CriticalHitCard.GenericCriticalHit damageCard;
        }

        public override void Discard()
        {
            Host.OnDamageCardIsDealt -= RegisterChewbaccaCrewTrigger;
            base.Discard();
        }

        private void UseChewbaccaCrewAbility(object sender, System.EventArgs e)
        {
            Sounds.PlayShipSound("Chewbacca");
            Messages.ShowInfo("Chewbacca (crew) is used");
            ChewbaccaCrewEventArgs arguments = (e as ChewbaccaCrewEventArgs);
            arguments.damageCard = null;
            if (arguments.ship.TryRegenShields()) Messages.ShowInfo("Shield is restored");
            Discard();
            Triggers.FinishTrigger();
        }
    }
}
