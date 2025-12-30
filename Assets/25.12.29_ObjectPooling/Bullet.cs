using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ObjectPooling
{
    public class Bullet : MonoBehaviour
    {
        private void OnEnable()
        {
            StartCoroutine(DeleteObj(3));
        }
        IEnumerator DeleteObj(float time)
        {
            yield return new WaitForSeconds(time);
            PoolManager.poolDic["Bullet"].ReturnPool(gameObject);
        }
        void Update()
        {
            transform.Translate(Vector3.forward * 10 * Time.deltaTime, Space.Self);
        }
    }

}
