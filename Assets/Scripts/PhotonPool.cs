using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PhotonPool : MonoBehaviour, IPunPrefabPool
{
    public readonly Dictionary<string, GameObject> ResourceCache = new Dictionary<string, GameObject>();
    private readonly Dictionary<string, List<GameObject>> ListCache = new Dictionary<string, List<GameObject>>();

    void OnEnable()
    {
        PhotonNetwork.PrefabPool = this;
    }

    void OnDisable()
    {
        PhotonNetwork.PrefabPool = default;
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        GameObject res = null;
        GameObject instance = null;

        #region Resource Caching
        bool cached = ResourceCache.TryGetValue(prefabId, out res);
        if (!cached)
        {
            res = Resources.Load<GameObject>(prefabId);
            if (res == null)
            {
                Debug.LogError("Not Found " + prefabId + "Check Pool.cs");
                return null;
            }
            else
            {
                this.ResourceCache.Add(prefabId, res);
            }
        }
        #endregion

        #region ListCaching
        List<GameObject> list = null;
        bool listCached = ListCache.TryGetValue(prefabId, out list);
        if (!listCached)
        {
            list = new List<GameObject>();
            ListCache.Add(prefabId, list);
        }
        #endregion

        if (list.Count == 0)
        {
            instance = GameObject.Instantiate(res, position, rotation) as GameObject;
        }
        else if (list.Count > 0)
        {
            instance = list[0];
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            list.RemoveAt(0);
        }

        if (instance != null)
        {
            instance.gameObject.SetActive(true);
            return instance;
        }
        else
        {
            Debug.LogError("Instance is Null Check Pool.cs");
            return null;
        }
    }

    public void Destroy(GameObject gameObject)
    {
        string prefabId = gameObject.name.Replace("(Clone)", "");

        List<GameObject> list = null;
        bool listCached = ListCache.TryGetValue(prefabId, out list);
        if (!listCached)
        {
            Debug.LogError("Not Found " + gameObject.name + " in ListCache");
            return;
        }

        gameObject.SetActive(false);
        list.Add(gameObject);
    }

}

/*
 * using UnityEngine;
using UnityEngine.Pool;

//
// T2 를 풀링하는 객체
//
public abstract class ICustomObjectPool<T1, T2> : MonoBehaviour where T1 : MonoBehaviour where T2 : MonoBehaviour
{
	public int DefaultPoolCapacity = 10;
	public int MaxPoolSize = 100;
	public bool CollectionCheck = true;

	// T2 를 관리 풀
	ObjectPool<T2> m_pool;
	public IObjectPool<T2> Pool
	{
		get
		{
			if (m_pool == null)
				m_pool = new ObjectPool<T2>(onCreateFunc, onGet, onRelease, onDestroy, CollectionCheck, DefaultPoolCapacity, MaxPoolSize);
			return m_pool;
		}
	}

	// 생성할 때는 호출되는 함수
	abstract protected T2 onCreateFunc();


	// T2 객체 한개를 활성화해서 전달해 준다.
	void onGet(T2 obj)
	{
		Debug.Log("## Pool - onGet");
		obj.gameObject.SetActive(true);
	}

	// T2 객체 한개를 비활성화해서 전달해 준다.
	void onRelease(T2 obj)
	{
		Debug.Log("## Pool - onRelease");
		obj.gameObject.SetActive(false);
	}

	// T2 객체 한개를 삭제
	void onDestroy(T2 obj)
	{
		Debug.Log("## Pool - onDestroy");
		Object.Destroy(obj.gameObject);
	}
}

public class MissilePool : ICustomObjectPool<MissilePool, Missile>
{
    [SerializeField] Missile prefabMissile = null;
	protected override Missile onCreateFunc()
	{
		Debug.Log("## Pool - onCreateFunc");
        Missile instMissile = Instantiate<Missile>(prefabMissile, Vector3.zero, Quaternion.identity);
		return instMissile;
	}


	public Missile Get()
	{
		Missile instMissile = Pool.Get();
		instMissile.callbackDestroy = onDestroy;
		return instMissile;
	}

	// 미사일이 충돌이 일어났을 때 호출
	void onDestroy(Missile missile)
	{
		Pool.Release(missile);
	}
}
*/

/*
 * 
 * public abstract class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    Queue<T> m_pool = null;

    public int Pool_Max_Size { get; set; } = 50;
    private int pool_count = 0;

    T tObj = null;

    protected ObjectPool()
    {
        m_pool = new Queue<T>();
    }

    abstract public T CreatePool();

    public T Get()
    {
        if (tObj == null)
        {
            tObj = CreatePool();
        }

        if (m_pool.Count == 0 && pool_count <= Pool_Max_Size)
        {
            pool_count++;
            T inst = Instantiate<T>(tObj);
            m_pool.Enqueue(inst);
            m_pool.Peek().gameObject.SetActive(false);
        }
        
        if(m_pool.Count > 0)
        {
            m_pool.Peek().gameObject.SetActive(true);
            return m_pool.Dequeue();
        }
        else
        {
            return null;
        }
    }

    public void Release(T recObj)
    {
        if(m_pool.Count == Pool_Max_Size)
        {
            Destroy(recObj);
        }
        else
        {
            recObj.gameObject.SetActive(false);
            m_pool.Enqueue(recObj);
        }
    }

    public void DestroyPool()
    {
        for(int i = 0; i < m_pool.Count; i++)
        {
            Destroy(m_pool.Dequeue());
        }
    }

}
*/

