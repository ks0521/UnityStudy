using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Day25_10_27 : MonoBehaviour
{
    Debuf Status = Debuf.Poison;
    enum Debuf
    {
        Basic=0,
        Prarlysis=1,
        Poison=2,
        Burn=3,
        Freeze=4,

    }
    void Start()
    {
        Debug.LogFormat("현재 상태이상: {0}",Status);
        Debug.Log("1: 마비약, 2: 해독약, 3: 화상연고, 4: 온열팩");
    }

    // Update is called once per frame
    void Update()
    {
        bool isKeyDown1 = false;
        bool isKeyDown2 = false;
        bool isKeyDown3 = false;
        bool isKeyDown4 = false;
        isKeyDown1 = Input.GetKeyDown(KeyCode.Alpha1);
        isKeyDown2 = Input.GetKeyDown(KeyCode.Alpha2);
        isKeyDown3 = Input.GetKeyDown(KeyCode.Alpha3);
        isKeyDown4 = Input.GetKeyDown(KeyCode.Alpha4);
        if (isKeyDown1 && (Status==Debuf.Prarlysis))
        {
            Debug.LogFormat("마비약 사용, {0}상태 해제", Status);
        }
        else if (isKeyDown2 && (Status == Debuf.Poison))
        {
            Debug.LogFormat("해독약 사용, {0}상태 해제", Status);
        }
        else if (isKeyDown3 && (Status == Debuf.Burn))
        {
            Debug.LogFormat("화상연고 사용, {0}상태 해제", Status);
        }
        else if (isKeyDown4 && (Status == Debuf.Freeze))
        {
            Debug.LogFormat("온열팩 사용, {0}상태 해제", Status);
        }

    }
}
