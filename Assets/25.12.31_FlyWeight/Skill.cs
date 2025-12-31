using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sm
{
    public class Skill : MonoBehaviour
    {

        // Update is called once per frame
        void Update()
        {
            transform.Translate(Vector3.forward * 30 * Time.deltaTime);
        }
    }

}
