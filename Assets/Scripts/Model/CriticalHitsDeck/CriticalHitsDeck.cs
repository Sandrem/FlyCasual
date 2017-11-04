using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GameModes;

public enum CriticalCardType
{
    Ship,
    Pilot
}

public static class CriticalHitsDeck{

    private static List<CriticalHitCard.GenericCriticalHit> Deck;

    public static void InitializeDeck()
    {
        Deck = new List<CriticalHitCard.GenericCriticalHit>();

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

    public static void GetCritCard(Action callBack)
    {
        GameMode.CurrentGameMode.GetCritCard(callBack);
    }

    public static void SetCurrentCriticalCardByIndex(int[] randomHolder)
    {
        CriticalHitCard.GenericCriticalHit critCard = null;

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
