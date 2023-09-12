# Object Pool Manager

## 기능 설명
오브젝트 풀링을 관리한다.

GetInstance 함수를 통해 `activeSelf == false` 상태의 프리팹 인스턴스를 얻을 수 있다.
ReleaseInstance 함수를 통해 `activeSelf` 상태에 관계없이 프리팹 인스턴스를 오브젝트 풀에 반환할 수 있다.

매개변수로는 프리팹(GameObject)을 사용한다.

## 사용법
```csharp
var instance = ObjectPoolManager.GetInstance(m_Prefab);
        
// Initialize instance
        
instance.SetActive(true);
        
// Do Something
        
ObjectPoolManager.ReleaseInstance(instance);
```

```csharp
var instance = ObjectPoolManager.GetInstance<T>(m_Prefab);
        
// Initialize instance
        
instance.gameObject.SetActive(true);
        
// Do Something
        
ObjectPoolManager.ReleaseInstance(instance.gameObject);
```

## 스크린샷
Object Pool Manager

![image](https://github.com/Eu4ng/unity-object-pool-manager/assets/59055049/d6f178e6-6fb0-4456-86db-cbbc887899f4)

Pool Tracker

![image](https://github.com/Eu4ng/unity-object-pool-manager/assets/59055049/3697fd64-ec21-4ee4-8988-2ca38a264180)
