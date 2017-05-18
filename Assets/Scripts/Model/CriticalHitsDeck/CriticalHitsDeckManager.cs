using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CriticalCardType
{
    Ship,
    Pilot
}

public class CriticalHitsDeckManager {

    private List<CriticalHitCard.GenericCriticalHit> Deck = new List<CriticalHitCard.GenericCriticalHit>();

    public void InitializeDeck()
    {
        for (int i = 0; i < 3; i++) // Max should be 7
        {
            Deck.Add(new CriticalHitCard.DirectHit());
        }

        for (int i = 0; i < 2; i++)
        {
            //Deck.Add(new CriticalHitCard.BlindedPilot());
            //Deck.Add(new CriticalHitCard.ConsoleFire());
            Deck.Add(new CriticalHitCard.DamagedCockpit());
            Deck.Add(new CriticalHitCard.DamagedEngine());
            //Deck.Add(new CriticalHitCard.DamagedSensorArray());
            Deck.Add(new CriticalHitCard.LooseStabilizer());
            //Deck.Add(new CriticalHitCard.MajorExplosion());
            //Deck.Add(new CriticalHitCard.MajorHullBreach());
            Deck.Add(new CriticalHitCard.ShakenPilot());
            Deck.Add(new CriticalHitCard.StructuralDamage());
            //Deck.Add(new CriticalHitCard.StunnedPilot());
            Deck.Add(new CriticalHitCard.ThrustControlFire());
            Deck.Add(new CriticalHitCard.WeaponsFailure());
        }

    }

    public void DrawCrit(Ship.GenericShip host)
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
