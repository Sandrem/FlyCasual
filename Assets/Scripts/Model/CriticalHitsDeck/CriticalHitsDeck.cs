using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GameModes;

/*public enum CriticalCardType
{
    Ship,
    Pilot
}*/

public static class CriticalHitsDeck{

    private static List<DamageDeckCard.GenericDamageCard> Deck;

    public static void InitializeDeck()
    {
        Deck = new List<DamageDeckCard.GenericDamageCard>();

        for (int i = 0; i < 7; i++) // Max should be 7
        {
            Deck.Add(new DamageDeckCard.DirectHit());
        }

        for (int i = 0; i < 2; i++)
        {
            Deck.Add(new DamageDeckCard.BlindedPilot());
            Deck.Add(new DamageDeckCard.DamagedCockpit());
            Deck.Add(new DamageDeckCard.DamagedEngine());
            Deck.Add(new DamageDeckCard.DamagedSensorArray());
            Deck.Add(new DamageDeckCard.LooseStabilizer());
            Deck.Add(new DamageDeckCard.MajorHullBreach());
            Deck.Add(new DamageDeckCard.ShakenPilot());
            Deck.Add(new DamageDeckCard.StructuralDamage());
            Deck.Add(new DamageDeckCard.ThrustControlFire());
            Deck.Add(new DamageDeckCard.WeaponsFailure());
            Deck.Add(new DamageDeckCard.ConsoleFire());
            Deck.Add(new DamageDeckCard.StunnedPilot());
            Deck.Add(new DamageDeckCard.MajorExplosion());
        }
    }

    public static void GetCritCard(bool isFaceUp, Action callBack)
    {
        GameMode.CurrentGameMode.GetCritCard(isFaceUp, callBack);
    }

    public static void SetCurrentCriticalCardByIndex(int[] randomHolder)
    {
        DamageDeckCard.GenericDamageCard critCard = null;

        critCard = Deck[randomHolder[0]];
        Deck.Remove(critCard);

        Combat.CurrentCriticalHitCard = critCard;
    }

    public static int GetDeckSize()
    {
        int deckSize = Deck.Count;

        if (deckSize == 0)
        {
            InitializeDeck();
            deckSize = Deck.Count;
        }

        return deckSize;
    }

}
