using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;

namespace SubPhases
{
    public class SelectDamageCardDecisionSubPhase : DecisionSubPhase
    {
        public GenericShip DecisionOwnerPilot;
        public GenericShip DamageCardsOwner;
        Action<GenericDamageCard> damageCardHandler;

        public override void PrepareDecision(Action callBack)
        {
            DescriptionShort = DecisionOwnerPilot.PilotInfo.PilotName + ": " + "Select a facedown damage card.";
            DecisionOwner = DecisionOwnerPilot.Owner;
            DecisionViewType = DecisionViewTypes.ImagesDamageCard;

            Dictionary<string, GenericDamageCard> selection = new Dictionary<string, GenericDamageCard>();

            foreach(GenericDamageCard card in DamageCardsOwner.Damage.DamageCards)
            {
                if (card.IsFaceup)
                    continue;

                if (selection.ContainsKey(card.Name))
                    continue;

                selection[card.Name] = card;
            }

            foreach(KeyValuePair<string, GenericDamageCard> kv in selection)
            {
                AddDecision(kv.Key, delegate { damageCardHandler(kv.Value); }, kv.Value.ImageUrl);
            }

            DefaultDecisionName = decisions.First().Name;

            callBack();
        }

        public void RegisterDamageCardHandler(Action<GenericDamageCard> handler)
        {
            damageCardHandler = handler;
        }
    }
}

