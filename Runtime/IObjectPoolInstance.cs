using UnityEngine;
using UnityEngine.Pool;

namespace E4.GameFramework
{
    // For IObjectPool<T>.Release(T _instance)
    public interface IObjectPoolInstance<T> where T : MonoBehaviour
    {
        public IObjectPool<T> ObjectPool { set; }
        
        // TODO 추가 예정
        // public int DefaultCapacity { get; }
        // public int MaxSize { get; }
    }
}