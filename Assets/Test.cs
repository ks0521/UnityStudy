/*
//1. 카드 구조체를 만들었습니다.
struct Card
{
    private string Name;
    private int power;


    //카드 구조체의 생성자.
    public Card(string _name, int _value)
    {
        Name = _name;
        power = _value;
    }

    public void PrintInfo()
    {
        Debug.LogFormat("{0} {1}", power, Name);
    }
}


//3. Card구조체를 멤버변수로 쓰는, Deck 구조체를 만들었습니다.
struct Deck
{
    //4. Deck 구조체에는 멤버변수로 Card의 배열을 두었습니다.
    //(구조체 Card)자체는, 값타입이지만, (Card의 배열)은 레퍼런스 타입입니다.
    //int[] int의 배열과 같은 관계라고 생각하시면 됩니다.
    private Card[] cards;
    private int maxCount; //덱에 들어갈 최대 수.
    private int currentCount; //현재 몇장이 덱에 들어가있는지.


    //6. Deck 생성자입니다. 
    public Deck(int _maxCount)
    {
        currentCount = 0;
        maxCount = _maxCount;
        //int[] int의 배열처럼 힙영역에 할당되고.
        //배열은 레퍼런스타입, 얕은복사가 일어나므로,
        //cards 에는 주소가 들어갑니다.
        cards = new Card[maxCount];  //4000
    }
    public void ShowCards()
    {
        Debug.Log("------카드 리스트------");
        for (int i = 0; i < currentCount; i++)
        {
            //Card 구조체의 함수인 PrintInfo();
            cards[i].PrintInfo();
        }
        Debug.Log("----------------------");
    }

    public void InsertCard(Card card)
    {
        //8. 현재장수가 덱의 최대크기를 벗어나면 안되기때문에
        //   예외처리합니다.
        if (currentCount >= maxCount)
        {
            Debug.Log("꽉 참");
            return;
        }
        //7. 현재 장수가 0이라면, 배열의 0번째 위치에,
        //   인자로 전달한 카드를 넣습니다.
        //    그리고 현재장수(currentCount)를 1증가시켜줍니다.
        //    즉, InsertCard 함수 호출 시, 덱에 카드가 증가됩니다.
        cards[currentCount] = card;
        currentCount++;
    }

    //12. 리턴타입이 Card라는것은 최종적으로 보내지는것이,
    //    Card 데이터타입이라는 것을 의미합니다.
    public Card Draw()
    {
        //14. 현재장수(currentCount)가 0장 이하일때는 뽑을 수 없겠죠?
        //    예외처리 부분입니다.
        Card drawCard = new Card();
        if (currentCount <= 0)
        {
            Debug.Log("뽑기 실패하였음.");
            return drawCard;
        }


        //13. 0번째 카드를 담아서 보내주기전에,
        //    예시 8번의 RemoveCard()를 수행해, cards 배열의 0번째 카드를 덮어씌워줍니다.
        drawCard = cards[0];
        RemoveCard();
        return drawCard;
    }


    //8. 덱의 0번째 카드 즉, cards[0]의 카드를 없애줍니다.
    private void RemoveCard()
    {
        if (currentCount <= 0)
        {
            Debug.Log("덱이 비었습니다.");
            return;
        }

        //9. 0번째부터 시작해서 뒤에있는 카드들을,
        //   한칸씩 앞으로 당기면서 덮어씌워지면
        //        0          1           2          3          4
        //   [오염된노움] [박사붐] [오염된박사붐] [노움] [파멸의예언자]
        //
        //   이렇게 카드들이 있다고 가정했을 때,
        //       0           1              2          3              4
        //   [박사붐]   [오염된박사붐]   [노움]   [파멸의예언자] | [파멸의예언자]
        //
        //   된다는 것을 이해하실 수 있을 것 입니다.
        //   마지막에 카드는 변경이 일어나지 않지만,
        //   현재 장수(currentCount)를 1줄이면, 없는 카드로 취급됩니다.
        for (int index = 0; index < currentCount - 1; index++)
        {
            cards[index] = cards[index + 1];
        }
        currentCount--;
    }

    //17. 덱을 섞는 부분입니다.
    public void Shuffle()
    {
        for (int i = 0; i < 5; i++)
        {
            //UnityEngine.Random.Range(최소값부터, 최대값이전까지) 입니다.
            int randNumA = UnityEngine.Random.Range(0, currentCount);
            int randNumB = UnityEngine.Random.Range(0, currentCount);

            Swap(randNumA, randNumB);
        }
    }

    //16. cards 배열의 인덱스 두개를 보내줘서 바꾸게하는 부분입니다.
    private void Swap(int indexA, int indexB)
    {
        Swap(ref cards[indexA], ref cards[indexB]);
    }

    //15. 이전 수업의 콜바이레퍼런스 설명 시 사용한
    //    Swap(ref int a, ref int b)를 복습해보세요.
    //    Card구조체도 값타입이므로, int와 마찬가지로 깊은복사가 일어났습니다.
    //    그렇기에 주소를 복사하는 얕은 복사하기 위해서, ref 키워드를 사용했습니다.
    private void Swap(ref Card cardA, ref Card cardB)
    {
        Card temp;
        temp = cardA;
        cardA = cardB;
        cardB = temp;
    }

}

private void Start()
{

    //2. 구조체 Card의 변수들을 선언했습니다.
    //메모리에 할당됨과 동시에 생성자가 호출되었습니다.
    //각 멤버변수들이 초기화 되었습니다.
    Card cardA = new Card("오염된 노움", 10);
    Card cardB = new Card("박사 붐", 70);
    Card cardC = new Card("오염된 박사 붐", 100);
    Card cardD = new Card("노움", 5);
    Card cardE = new Card("파멸의 예언자", 200);

    //5. Deck의 생성자를 호출했습니다.
    //5는 Card배열의 크기입니다.
    Deck deck = new Deck(5);

    //6. 만들어진 카드를 덱에 넣습니다.
    deck.InsertCard(cardA);
    deck.InsertCard(cardB);
    deck.InsertCard(cardC);
    deck.InsertCard(cardD);
    deck.InsertCard(cardE);
    deck.InsertCard(cardE);

    //10. 덱에 있는 카드 전부를 출력합니다.
    deck.ShowCards();

    //11. 카드를 한장 뽑습니다.
    //   deck.Draw() 시 리턴되는 데이터타입은 Card입니다.
    //   즉, (리턴된 Card).PrintInfo(); 를 수행합니다.
    deck.Draw().PrintInfo();

    deck.ShowCards();

    Debug.Log("섞고");

    //14. 덱 안의 카드들을 섞습니다.
    deck.Shuffle();
    deck.ShowCards();
}*/