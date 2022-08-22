using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPool:PoolManager
{
    
}
public class PoolManager : Singleton<PoolManager>
{
    /// <summary>
    /// 对象池
    /// </summary>
    public Dictionary<string, Queue<GameObject>> _poolDic = new Dictionary<string, Queue<GameObject>>();
    /// <summary>
    /// 预设体
    /// </summary>
    private Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

    class AwaitRecycle
    {
        public float AwaitTime = 0;
        public float StartTime = 0;
        public GameObject Pool;
        public AwaitRecycle(float time, float starttime, GameObject pool)
        {
            AwaitTime = time;
            StartTime = starttime;
            Pool = pool;
        }
    }
    /// <summary>
    /// 从对象池中获取对象
    /// </summary>
    /// <param name="objName"></param>
    /// <returns></returns>
    /// 

    public GameObject CreatePool(string prefabPath,Transform Parent=null)
    {
        //结果对象
        GameObject result = null;

        int lastIndex = prefabPath.LastIndexOf("/") + 1;
        string name = prefabPath.Substring(lastIndex, prefabPath.Length - lastIndex);


        //判断是否有该名字的对象池
        if (_poolDic.ContainsKey(name))
        {
            int poolCount = _poolDic[name].Count - 1;
            //对象池里有对象
            if (poolCount > 0)
            {
                int lastObjIndex = poolCount - 1;
                //获取结果
                result = _poolDic[name].Dequeue();
                if(Parent!=null)
                {
                    result.transform.SetParent(Parent);
                }
                //激活对象
                result.SetActive(true);
                //返回结果
                return result;
            }
        }
        //如果没有该名字的对象池或者该名字对象池没有对象

        GameObject prefab = null;
        //如果已经加载过该预设体
        if (_prefabs.ContainsKey(name))
        {
            prefab = _prefabs[name];
        }
        else     //如果没有加载过该预设体
        {
            //加载预设体
            prefab = Resources.Load<GameObject>(prefabPath);
            //更新字典
            _prefabs.Add(name, prefab);
        }

        if(prefab==null)
        {
            Debug.Log("prefabPath:"+ prefabPath+" is Null");
            return null;
        }

        //生成
        if(Parent!=null)
            result = Instantiate(prefab,Parent);
        else 
           result = Instantiate(prefab);
        //改名（去除 Clone）
        result.name = name;
        //返回
        return result;
    }

    /// <summary>
    /// 从对象池中获取对象
    /// </summary>
    /// <param name="prefabPath"></param>
    /// <param name="parentGo"></param>
    /// <returns></returns>
    public GameObject CreatePool(string prefabPath, GameObject parentGo)
    {
        var go = CreatePool(prefabPath, parentGo == null ? null : parentGo.transform);
        //go.transform.SetParent(parentGo.transform);
        return go;
    }

    /// <summary>
    /// 回收对象到对象池
    /// </summary>
    /// <param name="objName"></param>
    public void RecycleObj(GameObject obj)
    {
        //设置为非激活
        obj.SetActive(false);
        if (transform != null&& obj.transform!=null) 
        obj.transform.SetParent(transform);
        var name = obj.name;
        //判断是否有该对象的对象池
        if (_poolDic.ContainsKey(name))
        {
            //放置到该对象池
            _poolDic[name].Enqueue(obj);
        }
        else
        {
            //创建该类型的池子，并将对象放入
            var queue = new Queue<GameObject>() { };
            queue.Enqueue(obj);
            _poolDic.Add(name, queue);
        }
    }


    List<AwaitRecycle> _lis_AwaitRecycle = new List<AwaitRecycle>();
    List<int> _lis_cancellation = new List<int>();
    public void RecyclePool(float dtime, GameObject pool)
    {
        _lis_AwaitRecycle.Add(new AwaitRecycle(dtime, Time.time, pool));
        if (!IsInvoking("UpdateRecycleList"))
        {
            InvokeRepeating("UpdateRecycleList", 0, 0.1f);
        }
    }
    int _cancellation = 0;
    private void UpdateRecycleList()
    {
        _cancellation = _lis_AwaitRecycle.Count;
        for (int i = 0; i < _cancellation; i++)
        {
            if (Time.time - _lis_AwaitRecycle[i].StartTime > _lis_AwaitRecycle[i].AwaitTime)
            {
                RecycleObj(_lis_AwaitRecycle[i].Pool);
                _lis_AwaitRecycle.RemoveAt(i);
                i--;
                _cancellation--;
                if (_cancellation == 0)
                {
                    CancelInvoke("UpdateRecycleList");
                }
            }
        }
    }

    public void ClearPool(string objectName)
    {
        while (_poolDic.ContainsKey(objectName))
        {
            //对象池里有对象
            if (_poolDic[objectName].Count > 0)
            {
                //获取结果
                var result = _poolDic[objectName].Dequeue();
                //返回结果
                Destroy(result);
            }
        }
    }
    public void ClearPool()
    {
        var keys = _poolDic.Keys;

        foreach (var key in keys)
        {
            while (_poolDic[key].Count > 0)
            {
                //获取结果
                var result = _poolDic[key].Dequeue();
                //从池中移除该对象
                //返回结果
                Destroy(result);
            }
        }
        _poolDic.Clear();
    }
}
