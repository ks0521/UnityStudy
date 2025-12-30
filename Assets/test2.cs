using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : MonoBehaviour
{
    //        열거(하나하나 세는것이)가가능하다.
    class CustomStack : IEnumerable
    {
        int[] array;
        int count;

        public CustomStack()
        {
            array = new int[5];
            count = 0;
        }

        public int At(int index)
        {
            return array[index];
        }
        public void Push(int item)
        {
            array[count] = item;
            count++;
        }
        public int Pop()
        {
            int popItem = array[count - 1];
            count--;
            return popItem;
        }
        public int Peek()
        {
            return array[count - 1];
        }

        // 열거하는놈
        public IEnumerator GetEnumerator()
        {
            return new StackEnumerator(this);
        }
        public class StackEnumerator : IEnumerator
        {
            private CustomStack stack;
            private int currentIndex;
            public StackEnumerator(CustomStack stack)
            {
                this.stack = stack;
                currentIndex = stack.count;
            }

            //하나하나 보여주는 그 순간 때,
            //보여주기 위해 들고 있는 녀석.
            public object Current
            {
                get
                {
                    return stack.At(currentIndex);
                }
            }

            //다음으로 넘어갈 수 있는지에 대한 여부를 반환함.
            public bool MoveNext()
            {
                if (currentIndex <= 0)
                {
                    return false;
                }
                else
                {
                    currentIndex--;
                    return true;
                }
            }

            //이 열거자의 초기화하는 부분.
            public void Reset()
            {
                currentIndex = stack.count;
            }
        }

    }

    private void Start()
    {
        CustomStack stack = new CustomStack();
        stack.Push(10);
        stack.Push(20);
        stack.Push(30);

        //1. GetEnumerator를 통해서 Enumerator를 가져옴.
        //2. MoveNext를 통해 다음것을 가져올 수 있는지 물어봄.
        //3-1. 가져올 수 있으면, Current의 것을 가져와 담음.
        //3-2. 가져올 수 없으면, Reset호출하고 반복문 종료.
        foreach (object item in stack)
        {
            Debug.Log(item);
        }


        int[] array = new int[5];
        array[0] = 10;
        array[1] = 20;
        array[2] = 30;

        Stack stackB = new Stack();
        stackB.Push(10);
        stackB.Push(20);
        stackB.Push(30);

        while (stackB.Count > 0)
        {
            Debug.Log(stackB.Pop());
        }

        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log(array[i]);
        }


        foreach (object item in stackB)
        {
            Debug.Log(item);
        }

    }
}

/*
 
    class Monster
    {
        private int hp;
        private int atk;
        //atk도 프로퍼티로 만들어봅시다.
        
        //프로퍼티 사용한 접근
        //프로퍼티도 함수입니다.
        //프로퍼티는 우리가 기존에 사용하던 게터와 세터를
        //직관성 좋고, 편하게 사용하도록해준 문법입니다.
        public int Hp
        {
            get
            {
                return hp;
            }
            set
            {
                if (value >= 0)
                {
                    hp = value;
                }
            }
        }
            
        //아래는 기존의 방법 게터,세터
        public int GetHp()
        {
            return hp;
        }
        public void SetHp(int value)
        {
            if(value >= 0)
            {
                hp = value;
            }
        }

    }

    private void Start()
    {

        Monster monster = new Monster();

        //프로퍼티 사용했을 때,
        monster.Hp = 100;
        Debug.Log(monster.Hp);

        //기존의 게터,세터 사용했을 때,
        monster.SetHp(100);
        Debug.Log(monster.GetHp());
    }

 */