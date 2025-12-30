using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day10_31 : MonoBehaviour
{
    enum Symbol{
        // 문양 세기 : 스페이드 > 하트 > 다이아 > 클로버
        Spade,
        Heart,
        Diamond,
        Club
    }
    enum Rank
    {
        NaN,Ace, two, three, four, five, six, seven, eight, nine, ten, Jack, Queen, King
    }
    void swap(ref int numA, ref int numB)
    {
        int temp = numA;
        numA = numB;
        numB = temp;
    }
    void SelectSort(int[] arr)
    {   //내림차순 정렬
        int max,maxindex;
        int length = arr.Length;
        for (int i = 0; i < length; i++)
        {
            max = arr[i];
            maxindex = i;
            for (int j = i; j < length; j++)
            {
                if (arr[j] > max)
                {
                    max = arr[j];
                    maxindex = j;
                }
            }
            swap(ref arr[i], ref arr[maxindex]);    
        }

    }

    struct Card
    {
        Symbol symbol;
        Rank rank;
        public Card(Symbol symbol, Rank rank)
        {
            this.symbol = symbol;
            this.rank = rank;
        }
        public Symbol GetSymbol() { return symbol; }
        public Rank GetRank() { return rank; }
    }
    struct Deck
    {
        Card[] card;
        int size;
        int idx;
        int maxIdx;
        int maxRank;
        public Deck(int size){
            this.size = size;
            card = new Card[size];
            idx = 0;
            maxIdx = 0;
            maxRank = 0;
        }
        public void SwapCard(ref Card card1, ref Card card2)
        {
            Card temp = card1;
            card1 = card2;
            card2 = temp;
        }
        public void DrawCard(Card card)
        {
            if (idx < size) this.card[idx++] = card;
            else Debug.LogError("out of range");
        }
        public void SortDeck()
        {
            int compRank;
            for(int i = 0; i < size; i++)
            {
                maxIdx = i;
                maxRank = (int)card[i].GetRank();
                for(int j = i; j < size; j++)
                {
                    compRank = (int)card[j].GetRank();
                    if (compRank < maxRank)
                    {
                        maxRank = compRank;
                        maxIdx = j;
                    }
                    else if(compRank == maxRank)
                    {
                        if ((int)card[j].GetSymbol() < (int)card[i].GetSymbol())
                        {
                            maxRank = compRank;
                            maxIdx = j;
                        }
                    }
                }
                SwapCard(ref card[i], ref card[maxIdx]);
            }
        }
        public void PrintDeck()
        {
            for(int i = 0; i < size; i++)
            {
                if ((int)card[i].GetRank() >= 2 && (int)card[i].GetRank() <= 10)
                {
                    Debug.Log($"{(int)card[i].GetRank()} of {card[i].GetSymbol()}");
                }
                else
                {
                    Debug.Log($"{card[i].GetRank()} of {card[i].GetSymbol()}");

                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        int[] arr = new int[7] { 5, 4, 10, 1, 17, 12, 13 };
        Debug.Log("정렬 전");
        for(int i = 0; i < 7; i++)
        {
            Debug.Log(arr[i]);
        }
        SelectSort(arr);
        Debug.Log("정렬 후");
        for(int i = 0; i < 7; i++)
        {
            Debug.Log(arr[i]);
        }
        Deck deck = new Deck(7);
        deck.DrawCard(new Card(Symbol.Diamond, (Rank)1));
        deck.DrawCard(new Card(Symbol.Heart, (Rank)7));
        deck.DrawCard(new Card(Symbol.Heart, (Rank)13));
        deck.DrawCard(new Card(Symbol.Spade, (Rank)5));
        deck.DrawCard(new Card(Symbol.Club, (Rank)2));
        deck.DrawCard(new Card(Symbol.Diamond, (Rank)3));
        deck.DrawCard(new Card(Symbol.Spade, (Rank)3));
        Debug.Log("정렬 전");
        deck.PrintDeck();
        deck.SortDeck();
        Debug.Log("정렬 후");
        deck.PrintDeck();
    }

}
