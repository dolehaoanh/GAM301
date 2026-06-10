using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;

namespace VladislavTsurikov.UnityUtility.Runtime
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

#if UNITY_6000_0_OR_NEWER
                T[] singletonInstances = FindObjectsByType<T>(FindObjectsSortMode.None);
#else
                T[] singletonInstances = FindObjectsOfType<T>();
#endif

                if (singletonInstances.Length == 0)
                {
                    MonoBehaviourNameAttribute monoBehaviourNameAttribute =
                        (MonoBehaviourNameAttribute)typeof(T).GetAttribute(typeof(MonoBehaviourNameAttribute));
                    DontDestroyOnLoadAttribute dontDestroyOnLoadAttribute =
                        (DontDestroyOnLoadAttribute)typeof(T).GetAttribute(typeof(DontDestroyOnLoadAttribute));

                    GameObject obj = new() { name = monoBehaviourNameAttribute.Name };
                    if (dontDestroyOnLoadAttribute != null)
                    {
                        DontDestroyOnLoad(obj);
                    }

                    _instance = obj.AddComponent<T>();
                }
                else if (singletonInstances.Length > 1)
                {
                    for (int i = 0; i < singletonInstances.Length - 1; i++)
                    {
                        DestroyImmediate(singletonInstances[i]);
                    }
                }
                else
                {
                    _instance = singletonInstances[0];
                }

                return _instance;
            }
        }

        public static T GetInstance()
        {
            return _instance;
        }

        public static void DestroyGameObject()
        {
            if (_instance != null)
            {
                DestroyImmediate(_instance.gameObject);
            }
        }
    }
}
