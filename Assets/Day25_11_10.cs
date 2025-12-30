using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day25_11_10 : MonoBehaviour
{
    public interface IMovable
    {
        public void move(int x, int y);
    }
    struct Point : IMovable
    {
        public int x, y;
        public void move(int x,int y)
        {
            this.x += x;
            this.y += y;
        }
    }
    
    public void moving(IMovable target)
    {
        target.move(5,5);
    }
    public void Start()
    {
        Point p;
        p.x = 10;
        p.y = 10;
        moving(p);
        Debug.Log(p.x);
        Debug.Log(p.y);

        int num = 10;
        object obj = num;
        int x = (int)obj;
        obj = 59;
        Debug.Log(num);
        Debug.Log((int)obj);
        Debug.Log(x);
    }

    public void Update()
    {
        
    }
}
