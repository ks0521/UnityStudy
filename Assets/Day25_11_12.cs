using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day25_11_12 : MonoBehaviour
{

    public class CustomStack : IEnumerable
    {
        int[] arr;
        int index = 0;
        public CustomStack(int size)
        {
            arr = new int[size];
        }
        public bool Push(int num)
        {
            arr[index++] = num;
            return true;
        }
        public bool Pop()
        {
            index--;
            return true;
        }
        public int Top()
        {
            if (index > 0) { return arr[index - 1]; }
            else return 0;
        }
        public int At(int index)
        {
            return arr[index];
        }
        public int Count()
        {
            return index;
        }
        public IEnumerator GetEnumerator()
        {
            return new StackEnumerator(this);
        }
        class StackEnumerator : IEnumerator 
        {
            private CustomStack stack;
            private int currentIndex;
            public StackEnumerator(CustomStack stack)
            {
                this.stack = stack;
                currentIndex = stack.Count();
            }

            public object Current
            {
                get
                {
                    return stack.At(currentIndex);
                }
            }
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
            public void Reset()
            {
                currentIndex = stack.Count();
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
            CustomStack stack = new CustomStack(5);
            stack.Push(10);
            stack.Push(20);
            stack.Push(30);
            foreach(object item in stack)
            {
                Debug.Log(item);
            }
        }

    // Update is called once per frame
    void Update()
    {
        
    }       
}
