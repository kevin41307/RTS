using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler<T> where T : MonoBehaviour, IPooled<T>
{
    //public T[] instances;
    public List<T> instances = new List<T>();
    string prefab_name = default;
    protected Stack<int> m_FreeIdx;
    public bool autoExtend = true;
    T m_prefab;
    public void Initialize(int count, T prefab)
    {
        //instances = new T[count];
        m_prefab = prefab;
        prefab_name = prefab.name;
        m_FreeIdx = new Stack<int>(count);

        for (int i = 0; i < count; ++i)
        {
            T instance = Object.Instantiate(m_prefab);
            instance.gameObject.SetActive(false);
            instance.poolID = i;
            instance.pool = this;
            instances.Add(instance);

            /*
            T instance = null;
            instances.Add(instance);

            instances[i] = Object.Instantiate(prefab);
            instances[i].gameObject.SetActive(false);
            instances[i].poolID = i;
            instances[i].pool = this;
            */
            m_FreeIdx.Push(i);
        }
    }

    public T GetNew()
    {
        if (m_FreeIdx.Count <= 0)
        {
#if UNITY_EDITOR            
            Debug.Log("ObjectPool: " + prefab_name + " is Empty.");
#endif
        }
        int idx = m_FreeIdx.Pop();
        if (instances[idx] == null)
        {
            if(autoExtend)
            {
                Extend(5);
                idx = m_FreeIdx.Pop();
            }
        }
        if (instances[idx] != null)
        {
            instances[idx].gameObject.SetActive(true);
            return instances[idx];
        }
        else return null;       
    }
    public T GetNew(float expiredTime) //TODO: auto extend pool size?
    {
        T instance = GetNew();
        if (instance != null)
            instance.StartCoroutine(StartGiveBack(instance, expiredTime));
        return instance;
    }


    IEnumerator StartGiveBack(T obj, float expiredtime)
    {
        yield return new WaitForSeconds(expiredtime);
        Free(obj); 
    }
    
    public void Extend(int count)
    {
        if (count < 1) return;

        int startIdx = instances.Count;
        for (int i = startIdx; i < startIdx + count; i++)
        {
            T instance = Object.Instantiate(m_prefab);
            instance.gameObject.SetActive(false);
            instance.poolID = i;
            instance.pool = this;
            instances.Add(instance);
            m_FreeIdx.Push(i);
        }
        
    }
    

    public void Free(T obj)
    {
        m_FreeIdx.Push(obj.poolID);
        instances[obj.poolID].gameObject.SetActive(false);
    }



}

public interface IPooled<T> where T : MonoBehaviour, IPooled<T>
{
    int poolID { get; set; }
    ObjectPooler<T> pool { get; set; }
} 
