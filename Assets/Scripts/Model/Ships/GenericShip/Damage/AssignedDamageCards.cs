using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship
{
    public class AssignedDamageCards
    {
        public List<GenericDamageCard> DamageCards { get; private set; }
        public GenericShip Host { get; private set; }

        public AssignedDamageCards(GenericShip host)
        {
            Host = host;
            DamageCards = new List<GenericDamageCard>();
        }

        public List<GenericDamageCard> GetFaceupCrits(CriticalCardType? type = null)
        {
            return DamageCards.Where(n => n.IsFaceup && (n.Type == type || type == null)).ToList();
        }

        public List<GenericDamageCard> GetFacedownCards()
        {
            return DamageCards.Where(n => !n.IsFaceup).ToList();
        }

        public bool IsDamaged()
        {
            return DamageCards.Count > 0;
        }

        public int CountAssignedDamage()
        {
            return DamageCards.Sum(n => n.DamageValue);
        }

        public bool DiscardRandomFacedownCard()
        {
            bool result = false;

            List<GenericDamageCard> faceDownCards = GetFacedownCards();

            if (faceDownCards.Count != 0)
            {
                result = true;
                DamageCards.Remove(faceDownCards.First());
                Host.CallAfterAssignedDamageIsChanged();
            }
            return result;
        }

        public void FlipFaceupCritFacedown(GenericDamageCard critCard)
        {
            critCard.DiscardEffect();

            Messages.ShowInfo("Critical damage card \"" + critCard.Name + "\" is flipped facedown");
        }

        public void DealDrawnCard(Action callback)
        {
            if (Combat.CurrentCriticalHitCard != null)
            {
                DamageCards.Add(Combat.CurrentCriticalHitCard);
                Combat.CurrentCriticalHitCard.Assign(
                    Host,
                    delegate { Host.CallHullValueIsDecreased(callback); }
                );
            }
            else
            {
                callback();
            }
        }

        public bool HasFacedownCards { get { return DamageCards.Any(n => !n.IsFaceup); } }

        public void ExposeRandomFacedownCard(Action callback)
        {
            int randomIndex = UnityEngine.Random.Range(0, DamageCards.Count);
            GenericDamageCard randomCard = DamageCards[randomIndex];

            randomCard.Expose(callback);
        }

        // EXTRA

        // Suffer Facedown Damage Card

        public void SufferFacedownDamageCard(DamageSourceEventArgs e, Action callback)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer facedown damage card",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = SufferHullDamage,
                EventArgs = e
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, callback);
        }

        private void SufferHullDamage(object sender, EventArgs e)
        {
            Messages.ShowInfoToHuman(string.Format("{0}: Facedown card is dealt", Host.PilotName));
            Host.SufferHullDamage(false, e);
        }

        // Suffer regular damage

        public void SufferRegularDamage(DamageSourceEventArgs e, Action callback)
        {
            Host.AssignedDamageDiceroll.AddDice(DieSide.Success);

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Damage",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = Host.Owner.PlayerNo,
                    EventHandler = Host.SufferDamage,
                    EventArgs = e
                });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, callback);
        }

    }
}
