using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adaptor
{
    public class Bullet : MonoBehaviour
    {
        void Update()
        {
            transform.Translate(Vector3.forward * 30 * Time.deltaTime, Space.Self);
        }
    }

}
