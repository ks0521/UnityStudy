using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectPooling
{
    [System.Serializable]
    public struct PoolData
    {
        public GameObject poolingObj;
        public int count;
        public string name;
    }
    public class ObjectPool
    {
        public PoolData data;
        public Queue<GameObject> pool;
        public ObjectPool(PoolData _data)
        {
            pool = new Queue<GameObject>();
            data = _data;
            Init();
        }
        public void Init()
        {
            for (int i = 0; i < data.count; i++)
            {
                GameObject newObj = GameObject.Instantiate(data.poolingObj);
                pool.Enqueue(newObj);
                newObj.SetActive(false);
            }
        }
        public void UsePool(Vector3 pos, Quaternion rot)
        {
            if (pool.Count <= 0)
            {
                Debug.LogWarning("풀 내부 남은 오브젝트 없음");
                return;
            }
            GameObject newObj = pool.Dequeue();
            newObj.transform.position = pos;
            newObj.transform.rotation = rot;
            newObj.SetActive(true);
        }
        public void ReturnPool(GameObject returnObj)
        {
            returnObj.SetActive(false);
            pool.Enqueue(returnObj);
        }
    }
    public class PoolManager : MonoBehaviour
    {
        public List<PoolData> datas;
        public static Dictionary<string, ObjectPool> poolDic;

        private void Awake()
        {
            poolDic = new Dictionary<string, ObjectPool>();
            for (int i = 0; i < datas.Count; i++)
            {
                AddDictionary(datas[i]);
            }
        }

        void AddDictionary(PoolData data)
        {
            poolDic.Add(data.name, new ObjectPool(data));
        }
    }

}
