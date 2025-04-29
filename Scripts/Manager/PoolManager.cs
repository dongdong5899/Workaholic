using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    static public PoolManager Instance;

    private Dictionary<string, Stack<PoolableMono>> _pool = new Dictionary<string, Stack<PoolableMono>>();


    [SerializeField]
    private PoolListSO _poolListSO;

    private void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        PoolAdd(_poolListSO.poolingSettings);
    }

    private void PoolAdd(PoolSetting[] poolSetting)
    {
        foreach (PoolSetting ps in poolSetting)
        {
            _pool.Add(ps.poolingObjet.name, CreateList(ps.poolingObjet, ps.poolingCount));
        }
    }

    private Stack<PoolableMono> CreateList(PoolableMono prefab, int count)
    {
        Stack<PoolableMono> _poolList = new Stack<PoolableMono>();
        for (int i = 0; i < count; i++)
        {
            PoolableMono obj = Instantiate(prefab, transform);
            obj.gameObject.SetActive(false);
            obj.gameObject.name = obj.gameObject.name.Replace("(Clone)", "");
            _poolList.Push(obj);
        }

        return _poolList;
    }

    public PoolableMono Pop(string key, Vector3 pos, Vector3 scale, Vector3 rotate)
    {
        PoolableMono obj = _pool[key].Pop();
        obj.transform.position = pos;
        obj.GetComponent<ParticleController>()._follow = null;
        obj.transform.localScale = scale;
        obj.transform.eulerAngles = rotate;
        obj.gameObject.SetActive(true);
        obj.Init();

        return obj;
    }

    public PoolableMono Pop(string key, Vector3 pos, Vector3 scale, Vector3 rotate, GameObject follow)
    {
        PoolableMono obj = _pool[key].Pop();
        obj.GetComponent<ParticleController>()._pos = pos;
        obj.transform.position = follow.transform.position + pos;
        obj.GetComponent<ParticleController>()._follow = follow;
        obj.transform.localScale = scale;
        obj.transform.eulerAngles = rotate;
        obj.gameObject.SetActive(true);
        obj.Init();

        return obj;
    }

    public PoolableMono Pop(string key, Vector3 pos, Vector3 scale, Vector3 rotate, Vector2 dir, float force)
    {
        PoolableMono obj = _pool[key].Pop();
        obj.transform.position = pos;
        obj.GetComponent<BulletController>()._dir = dir * force;
        obj.transform.localScale = scale;
        obj.transform.eulerAngles = rotate;
        obj.gameObject.SetActive(true);
        obj.Init();

        return obj;
    }

    public void Push(PoolableMono obj)
    {
        obj.gameObject.SetActive(false);
        _pool[obj.name].Push(obj);
    }


    void Update()
    {
        
    }
}
