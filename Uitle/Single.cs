using UnityEngine;
using System.Collections;

public class Single<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T _instance = null;
    private static bool applicationIsQuitting = false;
    private static object _lock = new object();


    [System.Obsolete("instance is deprectaed, plase use Instance instaed")]
    public static T instance
    {
        get
        {
            return ins;
        }
    }


    public static T ins
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("<color=blue>[Singleton]</color> Instance '" + typeof(T) + "' already destroyed on application quit. Won't create again - returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("<color=blue>[Singleton]</color> Something went really wrong  - there should never be more than 1 singleton! Reopenning the scene might fix it.");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singletonParnet = GameObject.Find("Singleton");
                        if (null == singletonParnet)
                        {
                            singletonParnet = new GameObject();
                            singletonParnet.name = "Singleton";
                            DontDestroyOnLoad(singletonParnet);
                        }

                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).ToString();

                        DontDestroyOnLoad(singleton);

                        singleton.transform.parent = singletonParnet.transform;

                        //Debug.Log("<color=blue>[Singleton]</color> An instance of " + typeof(T) + " is needed in the scene, so '" + singleton + "' was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        //Debug.Log("<color=blue>[Singleton]</color> Using instance already created: " + _instance.gameObject.name);
                    }
                }

                return _instance;
            }
        }
    }

    public static bool HasInstance
    {
        get
        {
            return !IsDestroyed;
        }
    }

    public static bool IsDestroyed
    {
        get
        {
            if (_instance == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }



    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    protected virtual void OnDestroy()
    {
        _instance = null;
        applicationIsQuitting = true;
        //Debug.Log(typeof(T) + " [Mog.Singleton] instance destroyed with the OnDestroy event");
    }

    protected virtual void OnApplicationQuit()
    {
        _instance = null;
        applicationIsQuitting = true;
        //Debug.Log(typeof(T) + " [Mog.Singleton] instance destroyed with the OnApplicationQuit event");
    }
}