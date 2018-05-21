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

        public List<GenericDamageCard> GetFaceupCrits()
        {
            return DamageCards.Where(n => n.IsFaceup).ToList();
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
                faceDownCards.RemoveAt(0);
                Host.CallAfterAssignedDamageIsChanged();
            }
            return result;
        }

        public void FlipFaceupCritFacedown(GenericDamageCard critCard)
        {
            critCard.DiscardEffect();
            DamageCards.Remove(critCard);

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
    }
}
