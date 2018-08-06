using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{
    public class SelectDamageCardDecisionSubPhase : DecisionSubPhase
    {
        Action<GenericDamageCard> damageCardHandler;

        public override void PrepareDecision(Action callBack)
        {
            InfoText = Selection.ActiveShip.PilotName + ": " + "Select a face down damage card.";
            DecisionOwner = Selection.ActiveShip.Owner;

            Dictionary<string, GenericDamageCard> selection = new Dictionary<string, GenericDamageCard>();

            foreach(GenericDamageCard card in Selection.ThisShip.Damage.DamageCards)
            {
                if (card.IsFaceup)
                    continue;

                if (selection.ContainsKey(card.Name))
                    continue;

                selection[card.Name] = card;
            }

            foreach(KeyValuePair<string, GenericDamageCard> kv in selection)
            {
                AddDecision(kv.Key, delegate { damageCardHandler(kv.Value); });
            }

            callBack();
        }

        public void RegisterDamageCardHandler(Action<GenericDamageCard> handler)
        {
            damageCardHandler = handler;
        }
    }
}

