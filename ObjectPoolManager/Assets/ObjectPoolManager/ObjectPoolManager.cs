using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace E4.ObjectPoolManager
{
    public class ObjectPoolManager : GenericMonoSingleton<ObjectPoolManager>
    {
        Dictionary<int, IObjectPool<MonoBehaviour>> m_Pools = new Dictionary<int, IObjectPool<MonoBehaviour>>();

        [Header("Registered Prefabs")] 
        [SerializeField] List<MonoBehaviour> m_PrefabList = new List<MonoBehaviour>();
        [SerializeField] List<int> m_PrefabIDList = new List<int>();

        /// <summary>
        /// 매개인자로 전달받은 원본 프리팹으로부터 activeSelf = false 상태인 프리팹 인스턴스를 생성한 뒤,
        /// 프리팹 인스턴스를 대상으로 하는 Object Pool을 생성하고 Dictionary에 저장한다.
        /// </summary>
        public IObjectPool<T> Initialize<T>(T _prefab, bool _collectionCheck = true,
            int _defaultCapacity = 10, int _maxSize = 1000) where T : MonoBehaviour
        {
            var prefabID = _prefab.GetInstanceID();

            return Initialize<T>(prefabID, _prefab, _collectionCheck,
                _defaultCapacity, _maxSize);
        }
        
        /// <summary>
        /// 런타임 최적화를 위해 오브젝트 풀을 사용하는 클래스의 OnValidate 이벤트에서 프리팹의 인스턴스 ID를 미리 가져와 멤버 변수로 저장해두는 경우 사용
        /// </summary>
        public IObjectPool<T> Initialize<T>(int _prefabID, T _prefab, bool _collectionCheck = true,
            int _defaultCapacity = 10, int _maxSize = 1000) where T : MonoBehaviour
        {
            // 인터페이스 상속 여부 확인
            if (_prefab is not IObjectPoolInstance<T>)
            {
                print(_prefab.name + " must inherit IObjectPoolInstance<" + typeof(T).Name + ">");
                return null;
            }

            // 이미 등록된 상태라면 미리 생성된 오브젝트 풀 반환
            IObjectPool<T> objectPool;
            if (m_Pools.TryGetValue(_prefabID, out var createdPool))
            {
                objectPool = createdPool as IObjectPool<T>;
            }
            else
            {
                // 오브젝트 풀 생성 및 등록
                objectPool = CreatePool(_prefab, _collectionCheck, _defaultCapacity, _maxSize);
                m_Pools.Add(_prefabID, objectPool as IObjectPool<MonoBehaviour>);
                m_PrefabList.Add(_prefab);
                m_PrefabIDList.Add(_prefabID);
            }

            return objectPool;
        }
        
        /// <summary>
        /// 매개인자로 전달받은 원본 프리팹으로부터 activeSelf = false 상태인 프리팹 인스턴스를 생성한 뒤,
        /// 프리팹 인스턴스를 대상으로 하는 Object Pool을 생성하고 Dictionary에 저장한다.
        /// </summary>
        IObjectPool<T> CreatePool<T>(T _prefab, bool _collectionCheck,
            int _defaultCapacity, int _maxSize) where T : MonoBehaviour
        {
            // 생성된 오브젝트들을 담아둘 빈 오브젝트
            var instanceGroup = new GameObject(_prefab.name + " Pool")
            {
                transform =
                {
                    parent = transform
                }
            };

            // 비활성화된 프리팹 인스턴스 생성
            var isActive = _prefab.gameObject.activeSelf;
            _prefab.gameObject.SetActive(false);
            var instance = Instantiate(_prefab, instanceGroup.transform);
            instance.name = _prefab.name;
            _prefab.gameObject.SetActive(isActive);

            // Pool 생성
            var poolInstance = new PoolInstance<T>(instance, instanceGroup.transform, _collectionCheck,
                _defaultCapacity, _maxSize);

            return poolInstance.GetPool();
        }
    }

    public class PoolInstance<T> where T : MonoBehaviour
    {
        readonly T m_PrefabInstance; // 복제 대상
        readonly IObjectPool<T> m_Pool; // 실제 오브젝트 풀
        readonly Transform m_InstanceGroup; // 생성된 오브젝트들을 담아두는 빈 오브젝트

        public IObjectPool<T> GetPool() => m_Pool;

        public PoolInstance(T _prefabInstance, Transform _instanceGroup, bool _collectionCheck,
            int _defaultCapacity, int _maxSize)
        {
            // 변수 설정
            m_PrefabInstance = _prefabInstance;
            m_InstanceGroup = _instanceGroup;

            // 오브젝트 풀 생성
            m_Pool = new ObjectPool<T>(OnCreate, OnGet, OnRelease, OnDestroy, _collectionCheck,
                _defaultCapacity,
                _maxSize);
        }

        T OnCreate()
        {
            var instance = Object.Instantiate(m_PrefabInstance, m_InstanceGroup);
            (instance as IObjectPoolInstance<T>).ObjectPool = m_Pool; // ObjectPoolManager.Initialize 에서 인터페이스 상속 여부 확인 완료

            return instance;
        }

        void OnGet(T _instance)
        {

        }

        void OnRelease(T _instance)
        {
            _instance.gameObject.SetActive(false);
        }

        void OnDestroy(T _instance)
        {
            Object.Destroy(_instance.gameObject);
        }
    }
}