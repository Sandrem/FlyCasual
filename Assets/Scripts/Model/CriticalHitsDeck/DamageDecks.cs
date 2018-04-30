using System;
using System.Collections;
using System.Collections.Generic;
using GameModes;
using DamageDeckCard;
using Players;

public static class DamageDecks
{
    private static List<DamageDeck> damadeDecks;

    public static bool Initialized
    {
        get { return damadeDecks != null; }
    }

    public static void Initialize()
    {
        damadeDecks = new List<DamageDeck>
        {
            new DamageDeck(PlayerNo.Player1),
            new DamageDeck(PlayerNo.Player2)
        };

        foreach (DamageDeck deck in damadeDecks)
        {
            deck.ShuffleFirstTime();
        }
    }

    public static DamageDeck GetDamageDeck(PlayerNo playerNo)
    {
        return damadeDecks.Find(n => n.PlayerNo == playerNo);
    }

    public static void DrawDamageCard(PlayerNo playerNo, bool isFaceup, Action<EventArgs> doWithDamageCard, EventArgs e)
    {
        GetDamageDeck(playerNo).DrawDamageCard(isFaceup, doWithDamageCard, e);
    }
}

public class DamageDeck
{
    public List<GenericDamageCard> Deck { get; private set; }
    public PlayerNo PlayerNo { get; private set; }
    public int Seed { get; private set; }

    public DamageDeck(PlayerNo playerNo)
    {
        PlayerNo = playerNo;
        CreateDeck();
    }

    public void ShuffleFirstTime()
    {
        Random random = new Random();
        GameMode.CurrentGameMode.GenerateDamageDeck(PlayerNo, random.Next());
    }

    private void CreateDeck()
    {
        Deck = new List<GenericDamageCard>();

        for (int i = 0; i < 7; i++) // 7
        {
            Deck.Add(new DirectHit());
        }

        for (int i = 0; i < 2; i++) // 2
        {
            Deck.Add(new BlindedPilot());
            Deck.Add(new DamagedCockpit());
            Deck.Add(new DamagedEngine());
            Deck.Add(new DamagedSensorArray());
            Deck.Add(new LooseStabilizer());
            Deck.Add(new MajorHullBreach());
            Deck.Add(new ShakenPilot());
            Deck.Add(new StructuralDamage());
            Deck.Add(new ThrustControlFire());
            Deck.Add(new WeaponsFailure());
            Deck.Add(new ConsoleFire());
            Deck.Add(new StunnedPilot());
            Deck.Add(new MajorExplosion());
        }
    }

    public void PutOnTop(GenericDamageCard card)
    {
        Deck.Insert(0, card);
    }

    public void DrawDamageCard(bool isFaceup, Action<EventArgs> doWithDamageCard, EventArgs e)
    {
        if (Deck.Count == 0) ReCreateDeck();

        GenericDamageCard drawedCard = Deck[0];
        Deck.Remove(drawedCard);
        drawedCard.IsFaceup = isFaceup;

        Combat.CurrentCriticalHitCard = drawedCard;

        doWithDamageCard(e);
    }

    public void RemoveFromDamageDeck(GenericDamageCard card)
    {
        Deck.Remove(card);
    }

    private void ReCreateDeck()
    {
        CreateDeck();
        ReShuffleDeck();
    }

    public void ReShuffleDeck()
    {
        if (Seed < int.MaxValue)
        {
            ShuffleDeck(Seed + 1);
        }
        else
        {
            ShuffleDeck(int.MinValue);
        }
    }

    public void ShuffleDeck(int seed)
    {
        Seed = seed;
        Random random = new Random(seed);

        int n = Deck.Count;
        for (int i = 0; i < n; i++)
        {
            int r = i + random.Next(n - i);
            GenericDamageCard t = Deck[r];
            Deck[r] = Deck[i];
            Deck[i] = t;
        }
    }
}