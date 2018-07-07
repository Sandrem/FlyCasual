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

    }
}
