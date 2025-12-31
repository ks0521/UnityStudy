using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Day25_10_29 : MonoBehaviour
{
    enum Status
    {
        Full,
        neturality,
        empty
    }
    struct Card
    {
        private int id;
        private string name;
        public Card(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
        public void PrintCard()
        {
            Debug.LogFormat("{0}-{1}", this.name, this.id);
        }
        public int GetId() { return this.id; }
        public string GetName() { return this.name; }
        
    }
    struct Deck
    {
        private Card[] deck;
        private int index; //현재 카드 매수
        private int size; //최대 카드 매수
        private bool[] isVisited;
        public Deck(int size)
        {
            deck = new Card[size];
            isVisited = new bool[size];
            index= 0; //현재 카드 매수
            this.size = size;
        }
        public void SetDeck(string name, int id)
        {
            if (index < size)
            {
                deck[index++] = new Card(id, name);
            }
            else
            {
                Debug.LogWarning("덱 매수 한계 도달");
            }
        }
        public int getDeckSize()
        {//현재 덱 매수 반환
            return index;
        }
        public void DrawCard()
        {
            if (this.index < 1)
            {
                Debug.LogWarning("남은 카드가 없습니다");
                return;
            }
            int card = UnityEngine.Random.Range(0, index);
            Debug.Log("Draw!");
            deck[card].PrintCard();

            DeleteCard(card);
        }
        public void PrintDeck()
        {
            Debug.Log("덱에 남은 카드 출력: ");
            for(int i = 0; i < index; i++)
            {
                deck[i].PrintCard();
            }
        }
        private void DeleteCard(int delete)
        { //카드 한장을 삭제
            //끝번호의 카드와 뽑은 번호의 카드를 교환한 후 현재 카드 매수 -1
            Card temp = deck[delete];
            deck[delete] = deck[index - 1];
            deck[index - 1] = temp;
            index--;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Deck d1 = new Deck(5);
        d1.SetDeck("오염된 노움",1);
        d1.SetDeck("안녕로봇", 2);
        d1.SetDeck("전기 토템", 3);
        d1.SetDeck("슬라임", 4);
        d1.SetDeck("성기사", 5);
        d1.PrintDeck();
        Debug.LogFormat("현재 덱 매수: {0}",d1.getDeckSize());
        d1.DrawCard();
        d1.DrawCard();
        d1.DrawCard();
        d1.PrintDeck();
        Debug.LogFormat("현재 덱 매수: {0}", d1.getDeckSize());
        d1.DrawCard();
        d1.DrawCard();
        d1.DrawCard();
        Debug.LogFormat("현재 덱 매수: {0}", d1.getDeckSize());
        d1.DrawCard();
        d1.PrintDeck();
        Debug.LogFormat("현재 덱 매수: {0}", d1.getDeckSize());


    }
}
