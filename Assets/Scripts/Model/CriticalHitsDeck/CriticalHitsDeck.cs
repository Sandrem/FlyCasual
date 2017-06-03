using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CriticalCardType
{
    Ship,
    Pilot
}

public static class CriticalHitsDeck {

    private static List<CriticalHitCard.GenericCriticalHit> Deck = new List<CriticalHitCard.GenericCriticalHit>();

    public static void InitializeDeck()
    {
        for (int i = 0; i < 3; i++) // Max should be 7
        {
            //  DONE    Deck.Add(new CriticalHitCard.DirectHit());
        }

        for (int i = 0; i < 2; i++)
        {
            //  DONE    Deck.Add(new CriticalHitCard.BlindedPilot());
            //  DONE    Deck.Add(new CriticalHitCard.ConsoleFire());
            //  DONE    Deck.Add(new CriticalHitCard.DamagedCockpit());
            //  DONE    Deck.Add(new CriticalHitCard.DamagedEngine());
            //  DONE    Deck.Add(new CriticalHitCard.DamagedSensorArray());
            //  DONE    Deck.Add(new CriticalHitCard.LooseStabilizer());
            Deck.Add(new CriticalHitCard.MajorExplosion());
            //  (all crits +action)  Deck.Add(new CriticalHitCard.MajorHullBreach());
            //  DONE    Deck.Add(new CriticalHitCard.ShakenPilot());
            //  DONE    Deck.Add(new CriticalHitCard.StructuralDamage());
            //  (on overlap after mov)  Deck.Add(new CriticalHitCard.StunnedPilot());
            //  DONE (no unflipped) Deck.Add(new CriticalHitCard.ThrustControlFire());
            //  DONE    Deck.Add(new CriticalHitCard.WeaponsFailure());
        }

    }

    public static void DrawCrit(Ship.GenericShip host)
    {
        int deckSize = Deck.Count;

        if (deckSize == 0)
        {
            InitializeDeck();
            deckSize = Deck.Count;
        }

        int index = Random.Range(0, deckSize);

        CriticalHitCard.GenericCriticalHit crit = Deck[index];
        Deck.Remove(crit);

        host.SufferCrit(crit);
    }

}
