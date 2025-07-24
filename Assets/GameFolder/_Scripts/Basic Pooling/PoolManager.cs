using UnityEngine;
using System.Collections.Generic;

namespace SKC.Managers
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            public GameObject prefab;
            public int initialSize;
            public Transform parentTransform;
        }

        public List<Pool> pools;

        private Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            InitializePools();
        }

        void InitializePools()
        {
            foreach (Pool p in pools)
            {
                if (p.prefab == null)
                {
                    Debug.LogWarning($"Pool for a prefab is null.");
                    continue;
                }
                
                string prefabName = p.prefab.name; 
                if (!objectPools.ContainsKey(prefabName))
                {
                    objectPools.Add(prefabName, new Queue<GameObject>());
                    
                    if (p.parentTransform == null)
                    {
                        GameObject poolParentGO = new GameObject($"Pool_{prefabName}");
                        p.parentTransform = poolParentGO.transform;
                        p.parentTransform.SetParent(this.transform);
                    }

                    for (int i = 0; i < p.initialSize; i++)
                    {
                        GameObject obj = Instantiate(p.prefab, p.parentTransform);
                        obj.SetActive(false);
                        objectPools[prefabName].Enqueue(obj);
                    }
                    Debug.Log($"Pool for '{prefabName}' initialized with {p.initialSize} objects.");
                }
            }
        }
        
        public GameObject GetObject(string prefabName, Transform parent = null, bool worldPositionStays = false)
        {
            if (!objectPools.ContainsKey(prefabName))
            {
                Debug.LogError($"Pool for '{prefabName}' does not exist.");
                return null;
            }

            GameObject obj;
            if (objectPools[prefabName].Count > 0)
            {
                obj = objectPools[prefabName].Dequeue();
            }
            else
            {
                Debug.LogWarning($"Pool for '{prefabName}' is empty.");
                Pool targetPool = pools.Find(p => p.prefab.name == prefabName);
                if (targetPool != null && targetPool.prefab != null)
                {
                    obj = Instantiate(targetPool.prefab, targetPool.parentTransform); 
                }
                else
                {
                    Debug.LogError($"Prefab not found in pools list.");
                    return null;
                }
            }

            obj.SetActive(true);
            if (obj.transform is RectTransform)
            {
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.SetParent(parent, worldPositionStays);
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = Vector2.zero; 
                rectTransform.localEulerAngles = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }
            else
            {
                obj.transform.SetParent(parent, worldPositionStays);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localEulerAngles = Vector3.zero;
                obj.transform.localScale = Vector3.one;
            }
            
            return obj;
        }
        
        public void ReturnObject(GameObject obj)
        {
            if (obj == null) return;

            string prefabName = obj.name.Replace("(Clone)", "");
            if (!objectPools.ContainsKey(prefabName))
            {
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            Pool targetPool = pools.Find(p => p.prefab != null && p.prefab.name == prefabName);
            if (targetPool != null && targetPool.parentTransform != null)
            {
                obj.transform.SetParent(targetPool.parentTransform);
            }
            else
            {
                obj.transform.SetParent(this.transform); 
            }
            
            objectPools[prefabName].Enqueue(obj);
        }
    }
}