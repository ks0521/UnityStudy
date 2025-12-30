using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ObjectPooling
{
    public class AudioManager : MonoBehaviour
    {
        public AudioClip attackClip;
        public AudioSource audioSource;
        private void OnEnable()
        {
            audioSource.PlayOneShot(attackClip);
            StartCoroutine(PlaySound());
        }
        IEnumerator PlaySound()
        {
            yield return new WaitForSeconds(1);
            PoolManager.poolDic["Sound"].ReturnPool(gameObject);
        }
    }
}
