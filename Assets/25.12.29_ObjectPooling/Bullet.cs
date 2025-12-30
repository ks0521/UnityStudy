using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
namespace ObjectPooling
{
    public class Bullet : MonoBehaviour
    {
        Rigidbody rb;
        public float speed = 5;
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
        private void OnEnable()
        {
            rb.velocity = Vector3.zero;
            rb.AddRelativeForce(Vector3.forward * speed, ForceMode.Impulse);
            StartCoroutine(DeleteObj(3));
        }

        IEnumerator DeleteObj(float time)
        {
            yield return new WaitForSeconds(time);
            PoolManager.poolDic["Bullet"].ReturnPool(gameObject);
        }
        //private void OnCollisionEnter(Collision collision)
        //{
        //    PoolManager.poolDic["Bullet"].ReturnPool(gameObject);
        //}
    }

}
