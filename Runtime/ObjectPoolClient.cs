using UnityEngine;
using UnityEngine.Pool;

namespace E4.GameFramework
{
    // TODO 인터페이스로 대체할 방법이 없을까?
    // For IObjectPool<T>.Get()
    public class ObjectPoolClient<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] T m_Prefab;
        [SerializeField] int m_PrefabID;
        protected IObjectPool<T> m_ObjectPool;

        protected virtual void Awake()
        {
            m_ObjectPool = ObjectPoolManager.Instance.Initialize(m_Prefab);
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            // None이 할당된 경우
            if (m_Prefab is null)
            {
                m_PrefabID = -1;
                return;
            }
            
            // 인터페이스 상속 여부 확인
            if (m_Prefab is not IObjectPoolInstance<T>)
            {
                Debug.LogError(typeof(T).Name + " must inherit IObjectPoolInstance<" + typeof(T).Name + ">");
                m_Prefab = null;
                m_PrefabID = -1;
            }
            else
            {
                m_PrefabID = m_Prefab.GetInstanceID();
            }
        }
#endif
    }
}