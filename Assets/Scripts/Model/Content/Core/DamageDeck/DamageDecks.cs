using System;
using System.Collections;
using System.Collections.Generic;
using GameModes;
using Players;
using Editions;
using GameCommands;
using UnityEngine;

public static class DamageDecks
{
    private static List<DamageDeck> damadeDecks;

    public static bool Initialized
    {
        get { return damadeDecks != null; }
    }

    public static IEnumerator Initialize()
    {
        damadeDecks = new List<DamageDeck>
        {
            new DamageDeck(PlayerNo.Player1),
            new DamageDeck(PlayerNo.Player2)
        };

        GameInitializer.SetState(typeof(DamageDeckSyncCommand));

        foreach (DamageDeck deck in damadeDecks)
        {
            deck.ShuffleFirstTime();
        }

        while (GameInitializer.AcceptsCommandType == typeof(DamageDeckSyncCommand) && GameInitializer.CommandsReceived < 2)
        {
            yield return new WaitForEndOfFrame();
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

    public static GameCommand GenerateDeckShuffleCommand(PlayerNo playerNo, int seed)
    {
        JSONObject parameters = new JSONObject();
        parameters.AddField("player", playerNo.ToString());
        parameters.AddField("seed", seed.ToString());

        return GameController.GenerateGameCommand(
            GameCommandTypes.DamageDecksSync,
            null,
            parameters.ToString()
        );
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
        System.Random random = new System.Random();
        GameMode.CurrentGameMode.GenerateDamageDeck(PlayerNo, random.Next());
    }

    private void CreateDeck()
    {
        Deck = new List<GenericDamageCard>();

        foreach (var cardInfo in Edition.Current.DamageDeckContent)
        {
            for (int i = 0; i < cardInfo.Value; i++)
            {
                GenericDamageCard card = (GenericDamageCard) Activator.CreateInstance(cardInfo.Key);
                Deck.Add(card);
            }
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
        System.Random random = new System.Random(seed);

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