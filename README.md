# Object Pool Manager (v0.2.0)

## 1. 주요 기능
### ObjectPoolManager
- 오브젝트 풀 생성 및 관리하는 클래스
- `Initialize` 함수를 통해 생성된 오브젝트 풀(`IObjectPool<T>`)을 얻을 수 있다
- `IObjectPool<T>.Get()`을 통해 얻은 컴포넌트의 게임 오브젝트는 `activeSelf == false` 상태이다
- `IObjectPool<T>.Release(T _instance)`를 통해 반환할 수 있으며, 이때 `_instance.gameObject.SetActive(false)`가 호출된다

### ObjectPoolClient\<T\>
- `IObjectPool<T>.Get()`을 호출하는 클래스
- `Awake()`에서 `m_Prefab`을 바탕으로 오브젝트 풀을 배정받는다
- `OnValidate()`에서 `m_Prefab`이 `IObjectPoolInstance<T>` 인터페이스를 구현하고 있는지 확인
- 런타임 최적화를 위해 `OnValidate()`에서 프리팹의 인스턴스 ID를 멤버 변수로 저장

### IObjectPoolInstance\<T\>
- 오브젝트 풀에서 생성될 프리팹 컴포넌트 T에서 반드시 구현되어야 하는 인터페이스
- 프리팹 인스턴스 생성 시 소속된 오브젝트 풀을 전달해줌으로써,
  프리팹 인스턴스에서 ` IObjectPool<T>.Release(T _instance)`을 호출할 수 있도록 한다

## 2. 업데이트 내역
- `Pool Tracker` 삭제 / `IObjectPoolInstance<T>`로 대체
- `GameObject` 대신 컴포넌트 `T` 를 기반으로 `ObjectPool` 생성
  - 불필요한 `GetComponent<T>()`  호출 방지
  - `GameObject`는 `T.gameObject`로 접근 가능
- 검색 성능 향상을 위해 오브젝트 풀 검색 시 `Dictionary`의 `Key`를 `GameObject` 대신 `T.GetInstanceID()`로 대체

## 3. 사용 예
```csharp
using System.Collections;
using E4.ObjectPoolManager;
using UnityEngine;

public class Gun : ObjectPoolClient<Bullet>
{
    [SerializeField] float m_BulletSpeed;
    void Start()
    {
        StartCoroutine(Fire());
    }

    IEnumerator Fire()
    {
        int count = 0;
        while (count < 10)
        {
            yield return new WaitForSeconds(1);
            var bullet = m_ObjectPool.Get();
            bullet.Initialize(m_BulletSpeed);
            bullet.gameObject.SetActive(true);

            count++;
        }
    }
}
```

```csharp
using System.Collections;
using E4.ObjectPoolManager;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour, IObjectPoolInstance<Bullet>
{
    Rigidbody m_Rigidbody;
    
    float m_Speed;
    
    /* IObjectPoolInstance 시작 */
    protected IObjectPool<Bullet> m_ObjectPool;
    public IObjectPool<Bullet> ObjectPool
    {
        set => m_ObjectPool = value;
    }
    /* IObjectPoolInstance 끝 */
    
    public void Initialize(float _speed)
    {
        m_Speed = _speed;
    }

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        m_Rigidbody.velocity = transform.forward * m_Speed;
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(10);
        m_ObjectPool.Release(this);
    }
}
```

## 4. 스크린샷
Object Pool Manager

![image](https://github.com/Eu4ng/unity-object-pool-manager/assets/59055049/f15fe320-a0b9-45be-a9a7-57c15bd96ace)


