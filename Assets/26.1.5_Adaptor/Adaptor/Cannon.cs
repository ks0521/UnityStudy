using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adaptor
{
    public class Cannon : MonoBehaviour
    {
        public GameObject bulletPrafab;
        IEnumerator CannonCo()
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(10);
            while (true)
            {
                Fire();
                yield return waitForSeconds;
            }
        }
        private void Start()
        {
            StartCoroutine(CannonCo());
        }

        public void Fire()
        {
            Debug.Log("빵!");
            Instantiate(bulletPrafab, transform.position, transform.rotation);
        }
    }

}
