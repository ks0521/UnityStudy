using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ObjectPooling
{
    public class Player : MonoBehaviour
    {
        [SerializeField]Transform firePoint;
        void Fire()
        {
            PoolManager.poolDic["Bullet"].UsePool(firePoint.position, firePoint.rotation);
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
