using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CriticalCardType
{
    Ship,
    Pilot
}

public static class CriticalHitsDeck{

    private static List<CriticalHitCard.GenericCriticalHit> Deck = new List<CriticalHitCard.GenericCriticalHit>();

    public static void InitializeDeck()
    {
        for (int i = 0; i < 7; i++) // Max should be 7
        {
            Deck.Add(new CriticalHitCard.DirectHit());
        }

        for (int i = 0; i < 2; i++)
        {
            Deck.Add(new CriticalHitCard.BlindedPilot());
            Deck.Add(new CriticalHitCard.DamagedCockpit());
            Deck.Add(new CriticalHitCard.DamagedEngine());
            Deck.Add(new CriticalHitCard.DamagedSensorArray());
            Deck.Add(new CriticalHitCard.LooseStabilizer());
            Deck.Add(new CriticalHitCard.MajorHullBreach());
            Deck.Add(new CriticalHitCard.ShakenPilot());
            Deck.Add(new CriticalHitCard.StructuralDamage());
            Deck.Add(new CriticalHitCard.ThrustControlFire());
            Deck.Add(new CriticalHitCard.WeaponsFailure());
            Deck.Add(new CriticalHitCard.ConsoleFire());
            Deck.Add(new CriticalHitCard.StunnedPilot());
            Deck.Add(new CriticalHitCard.MajorExplosion());
        }
    }

    public static void GetCritCard(Action SufferChosenCriticalHitCard)
    {
        int deckSize = CheckDeck();

        if (!Network.IsNetworkGame)
        {
            int[] randomHolder = new int[1];
            randomHolder[0] = UnityEngine.Random.Range(0, deckSize);
            SetCurrentCriticalCardByIndex(randomHolder);
            SufferChosenCriticalHitCard();
        }
        else
        {
            Network.GenerateRandom(new Vector2(0, deckSize - 1), 1, SetCurrentCriticalCardByIndex, SufferChosenCriticalHitCard);
        }
    }

    private static void SetCurrentCriticalCardByIndex(int[] randomHolder)
    {
        CriticalHitCard.GenericCriticalHit critCard = null;

        critCard = Deck[randomHolder[0]];
        Deck.Remove(critCard);

        Combat.CurrentCriticalHitCard = critCard;
    }

    private static int CheckDeck()
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
