using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day25_10_22 : MonoBehaviour
{
    void MiningGold(ref int gold)
    {
        if (gold >= 150)
        { 
            Debug.Log("Boosting!");
            gold += 90;
        }
        else
        {
            gold += 50;
        }
    }

    void SwapByValue(int a,int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }
    void SwapByReference(ref int a,ref int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }

    bool TryBaseDamage(float ATK, float CoefDMG, out int BaseDMG)
    {
        if(ATK==0 || CoefDMG == 0)
        {
            BaseDMG = -1;
            return false;
        }
        BaseDMG = (int)(ATK * CoefDMG) / 1000;
        return true;
    }
    // Start is called before the first frame update
    void Start()
    {
        int currentGold=0;
        int a = 30, b = 50;
        int BaseDMG=0;
        float ATK = 15700f;
        float ATK2 = 0f;
        float CoefDMG = 5.7f;
        Debug.Log("Mining Gole"+"\n----------------");
        for (int i = 0; i < 5; i++)
        {
            MiningGold(ref currentGold);
            Debug.Log(currentGold);
        }

        Debug.Log("Swap-Call By Value" + "\n----------------");
        Debug.LogFormat("Before: a is {0}, b is {1}", a, b);
        SwapByValue(a, b);
        Debug.LogFormat("After : a is {0}, b is {1}", a, b);

        Debug.Log("Swap-Call By Reference" + "\n----------------");
        Debug.LogFormat("Before: a is {0}, b is {1}", a, b);
        SwapByReference(ref a, ref b);
        Debug.LogFormat("After : a is {0}, b is {1}", a, b);

        if(TryBaseDamage(ATK,CoefDMG,out BaseDMG))
        {
            Debug.LogFormat("BaseDMG: {0}", BaseDMG);
        }
        else
        {
            Debug.LogWarning("Error");
        }
        if (TryBaseDamage(ATK2, CoefDMG, out BaseDMG))
        {
            Debug.LogFormat("BaseDMG: {0}", BaseDMG);
        }
        else
        {
            Debug.LogWarning("Error");
        }
    }
}
