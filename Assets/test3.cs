using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() 
    {
        int[] score = new int[10];
        for(int i = 0; i < 10; i++)
        {
            score[i] = 90 + i;
            Debug.Log(score[i]);
        }
        int[] copyScore = new int[10];
        for(int i = 0; i < 10; i++)
        {
            copyScore[i] = score[i];
        }
    }
}
