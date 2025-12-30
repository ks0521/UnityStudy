using UnityEngine;

public class Day25_10_30 : MonoBehaviour
{
    //핸디캡만큼 상대의 구매가격을 깎음(-2: 아주어려움, -1: 어려움, 0: 보통, 1: 쉬움, 2: 겁쟁이들의 쉼터)
    enum Handi
    {
        겁쟁이들의_쉼터, 쉬움, 보통, 어려움, 아주어려움
    }

    enum Attribute
    { //3속성 상성표(순->냉->광->순)
        순수,
        냉정,
        광기
    }
    enum Name
    {
        알레트,
        스피키,
        비비,
        파트라,
        에스피,
        아이시아,
        유미미,
        마요,
        리뉴아,
        size
    }
    enum Skill
    {
        삽치기,
        펌킨_매직,
        퀵실버랜스,
        민트머겅,
        헤롱헤롱_촛불,
        신제품_시연,
        발싸,
        수집의법칙임,
        타임_브레이크,
        size
    }
    //사용자 정의 데이터타입.
    struct Character
    {
        int needCost;
        int characterId;
        //각 캐릭터의 이름과 스킬은 동일한 enum식별자에 정의되어 있음
        double characterPower;
        int characterRank;

        public Character(int id, int rank)
        {
            characterId = id;
            characterRank = rank;
            switch (rank)
            {
                case 1: 
                    characterPower = 100;
                    needCost = 10;
                    break;
                case 2:
                    characterPower = 125;
                    needCost = 20;
                    break;
                case 3:
                    characterPower = 156;
                    needCost = 30;
                    break;
                default:
                    characterPower = 0;
                    needCost = 0;
                    break;
            }
        }

        public Name GetName() { return (Name)characterId; }
        public Skill GetSkill() { return (Skill)characterId; }
        public Attribute GetAttribute() { return (Attribute)(characterId/3); } //0~2는 순수(0), 3~5는 냉정(1), 6~8은 광기(2)
        public int GetCost() { return needCost; }
        public double GetPower() { return characterPower; }
        public int GetRank() { return characterRank; }

    }

    struct Player
    {
        int maxCost;
        int nowCost;
        Character [] card;
        int maxCard;
        //핸디캡만큼 상대의 구매가격을 깎음(-2: 아주어려움, -1: 어려움, 0: 보통, 1: 쉬움, 2: 겁쟁이들의 쉼터)
        public Player(int handicap)
        {
            maxCost = 80 + handicap*10;
            nowCost = 0;
            card = new Character[5];
            maxCard = 5;
        }
        public Character GetCharacter(int i) { return card[i]; }
        public void RandomSelect()
        {
            //상대방이 무작위 5장의 카드를 뽑음(중복 X)
            bool[] used = new bool[(int)Name.size];
            int randId;
            int randRank;
            do
            {
                for(int i = 0; i < (int)Name.size; i++)
                {
                    used[i] = false;
                }
                nowCost = 0;
                for (int i = 0; i < maxCard; i++)
                {
                    do
                    {
                        randId = UnityEngine.Random.Range(0, (int)Name.size);
                    } while (used[randId]);
                    used[randId] = true;
                    randRank = UnityEngine.Random.Range(1, 4);
                    card[i] = new Character(randId, randRank);
                    nowCost += card[i].GetCost();
                }
            } while (nowCost > maxCost || nowCost < maxCost - 10); //총 가격 내에서, 최대 가격의 5장을 뽑음(돈 안남김)
        }
    }

    struct Battle
    {
        Player []play;
        int[] match;
        string[] star;

        public Battle(int handicap)
        {
            play = new Player[2];
            play[0] = new Player(0); //유저는 핸디캡 X
            play[1] = new Player(handicap); //컴퓨터만 핸디캡 존재
            match = new int[5];
            Debug.Log($"{(Handi)(handicap + 2)}난이도 선택");
            star = new string[] { "", "★", "★★", "★★★" };
        }
        public void SelectComputer(int number)
        {
            Debug.Log("컴퓨터 선택!");
            play[number].RandomSelect();
            Character character;
            for (int i = 0; i < 5; i++)
            {
                character = play[number].GetCharacter(i);
                Debug.Log($"{i + 1}번째 구매: {star[character.GetRank()]}{character.GetName()} - ({character.GetCost()})원 소모");
            }
        }
        public void StartBattle()
        {
            int rankP1;
            int rankP2;
            Attribute attributeP1;
            Attribute attributeP2;
            Name nameP1;
            Name nameP2;
            double powerP1;
            double powerP2;
            int win = 0;
            int loose = 0;
            int draw = 0;
            for (int i = 0; i < match.Length; i++)
            {
                rankP1 = play[0].GetCharacter(i).GetRank();
                rankP2 = play[1].GetCharacter(i).GetRank();
                attributeP1 = play[0].GetCharacter(i).GetAttribute();
                attributeP2 = play[1].GetCharacter(i).GetAttribute();
                powerP1 = play[0].GetCharacter(i).GetPower();
                powerP2 = play[1].GetCharacter(i).GetPower();
                nameP1 = play[0].GetCharacter(i).GetName();
                nameP2 = play[1].GetCharacter(i).GetName();

                if (((int)attributeP1 + 1) % 3 == (int)attributeP2)
                {
                    Debug.Log($"{attributeP1} 속성 우세! 25% 추가 데미지");
                    powerP1 *= 1.25;
                }
                else if (((int)attributeP2 + 1) % 3 == (int)attributeP1)
                {
                    Debug.Log($"{attributeP2} 속성 우세! 25% 추가 데미지");
                    powerP2 *= 1.25;
                }
                Debug.Log($"{star[rankP1]}{nameP1}의 {(Skill)nameP1}! ({powerP1})vs {star[rankP1]}{nameP2}의 {(Skill)nameP2}! ({powerP2})");
                if (powerP1 > powerP2)
                { //유저 승리
                    Debug.Log($"{i}차전 유저 승리!");
                    win++;
                }
                else if (powerP1 == powerP2)
                {
                    Debug.Log("무승부!");
                    draw++;
                }
                else
                {
                    Debug.Log($"{i}차전 컴퓨터 승리!");
                    loose++;
                }
            }
            if (win>loose)
            {
                Debug.Log($"{win + draw} : {loose + draw}로 유저 승리! 더 높은 난이도를 도전해보세요!");
            }
            else if(win == loose)
            {
                Debug.Log($"{win + draw} : {loose + draw}로 무승부! 다시 도전해 보세요!");
            }
            else
            {
                Debug.Log($"{win + draw} : {loose + draw}로 컴퓨터 승리! 조금 어려웠나요??");
            }
        }
    }
    private void Start()
    {
        int handicap = 0;
        if (handicap <= 2 && handicap >= -2) { 
            Battle b = new Battle(handicap); 
            b.SelectComputer(0);
            b.SelectComputer(1);
            b.StartBattle();
        } //내부 들어가는 값 : 핸디캡(-2 ~ 2)
        else
        {
            Debug.LogWarning("Handicap out of range");
            return;
        }
        
    }
    private void Update()
    {
        
    }
}
