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
        for (int i = 0; i < 6; i++) // Max should be 7
        {
            //Deck.Add(new CriticalHitCard.DirectHit());
        }

        for (int i = 0; i < 2; i++)
        {
            //Deck.Add(new CriticalHitCard.BlindedPilot());
            Deck.Add(new CriticalHitCard.ConsoleFire());
            //Deck.Add(new CriticalHitCard.DamagedCockpit());
            //Deck.Add(new CriticalHitCard.DamagedEngine());
            //Deck.Add(new CriticalHitCard.DamagedSensorArray());
            //Deck.Add(new CriticalHitCard.LooseStabilizer());
            //Deck.Add(new CriticalHitCard.MajorExplosion());
            //Deck.Add(new CriticalHitCard.MajorHullBreach());
            //Deck.Add(new CriticalHitCard.ShakenPilot());
            //Deck.Add(new CriticalHitCard.StructuralDamage());
            //Deck.Add(new CriticalHitCard.StunnedPilot());
            //Deck.Add(new CriticalHitCard.ThrustControlFire());
            //Deck.Add(new CriticalHitCard.WeaponsFailure());
        }
    }

    public static void DrawCrit(object sender, EventArgs e)
    {
        int deckSize = Deck.Count;

        if (deckSize == 0)
        {
            InitializeDeck();
            deckSize = Deck.Count;
        }

        int index = UnityEngine.Random.Range(0, deckSize);

        CriticalHitCard.GenericCriticalHit crit = Deck[index];
        Deck.Remove(crit);

        (sender as Ship.GenericShip).SufferCrit(crit);
    }

    public static void DrawRegular(object sender, EventArgs e)
    {
        (sender as Ship.GenericShip).SufferHullDamage();
    }

}
