using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ObjectPooling
{
    public class Player : MonoBehaviour
    {
        void Fire()
        {
            PoolManager.poolDic["Bullet"].UsePool(transform.position, transform.rotation);
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fire();
            }
        }
    }

}
