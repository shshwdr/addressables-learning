using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using static UnityEngine.AddressableAssets.Addressables;

[Serializable]
public class AssetReferenceMaterial : AssetReferenceT<Material>
{
    public AssetReferenceMaterial(string guid) : base(guid) { }
}

public class AssetReferenceScene : AssetReferenceT<SceneAsset>
{
    public AssetReferenceScene(string guid) : base(guid) { }
}


[Serializable]
public class AssetReferenceTextAsset : AssetReferenceT<TextAsset>
{
    public AssetReferenceTextAsset(string guid) : base(guid) { }
}

[Serializable]
public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
{
    public AssetReferenceAudioClip(string guid) : base(guid) { }
}



public static class AddressableManager
{
    //this is for scene
    static Dictionary<object, AsyncOperationHandle> obejctToHandler = new Dictionary<object, AsyncOperationHandle>();

    public static bool isAssetLoaded(object key)
    {
        var handler = Addressables.GetDownloadSizeAsync(key);
        bool result = handler.IsDone;
        Addressables.Release(handler);
        return result;
    }
    #region Load
    //load
    //load async
    public static AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(object key)
    {
        return Addressables.LoadAssetAsync<TObject>(key);
    }
    //load async auto release
    public static AsyncOperationHandle<TObject> LoadAssetAsyncAutoRelease<TObject>(object key, GameObject ob)
    {
        var handle = Addressables.LoadAssetAsync<TObject>(key);
        AddAutoReleaseAssetTrigger(handle, ob);
        return handle;
    }
    //load sync
    public static AsyncOperationHandle<TObject> LoadAsset<TObject>(object key, Action<TObject> onComplete = null)
    {
        //Addressables.LoadAssetAsync("test");
        var handle = Addressables.LoadAssetAsync<TObject>(key);
        TObject obj = handle.WaitForCompletion();
        if (onComplete != null)
        {
            onComplete(handle.Result);

        }
        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            return handle;
        }
        else
        {
            Debug.LogError($"failed to fetch key {key}");
            return default(AsyncOperationHandle<TObject>);
        }
    }
    //load sync auto release
    public static TObject LoadAssetAutoRelease<TObject>(object key, GameObject ob, Action<TObject> onComplete = null)
    {
        var handle = LoadAssetAsyncAutoRelease<TObject>(key, ob);
        TObject obj = handle.WaitForCompletion();
        if (onComplete != null)
        {
            onComplete(handle.Result);
        }
        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            return obj;
        }
        else
        {
            Debug.LogError($"failed to fetch key {key}");
            return default(TObject);
        }
    }

    //load scene
    //load scene async
    public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(object key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
    {
        var handler = Addressables.LoadSceneAsync(key, loadMode, activateOnLoad, priority);
        obejctToHandler[key] = handler;
        return handler;
    }
    //load scene sync
    public static SceneInstance LoadScene(object key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
    {
        var handle = Addressables.LoadSceneAsync(key, loadMode, activateOnLoad, priority);
        obejctToHandler[key] = handle;
        var obj = handle.WaitForCompletion();
        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            return obj;
        }
        else
        {
            Debug.LogError($"failed to fetch key {key}");
            return default(SceneInstance);
        }
    }
    //load assets by label async
    public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(object key, Action<TObject> callback = null)
    {
        return Addressables.LoadAssetsAsync<TObject>(key, callback);
    }
    // load assets by label async auto release
    public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsyncAutoRelease<TObject>(object key, GameObject ob, Action<TObject> callback = null)
    {
        var handler = Addressables.LoadAssetsAsync<TObject>(key, callback);
        AddAutoReleaseAssetTrigger(handler, ob);
        return handler;
    }
    //load assets by label sync
    public static AsyncOperationHandle<IList<TObject>> LoadAssets<TObject>(object key, Action<TObject> callback = null)
    {
        var handle = LoadAssetsAsync<TObject>(key, callback);
        List<TObject> objs = handle.WaitForCompletion() as List<TObject>;
        return handle;
    }
    //load assets by label sync auto release
    public static IList<TObject> LoadAssetsAutoRelease<TObject>(object key, GameObject ob, Action<TObject> callback = null)
    {

        var handle = LoadAssetsAsync<TObject>(key, callback);
        AddAutoReleaseAssetTrigger(handle, ob);
        IList<TObject> objs = handle.WaitForCompletion();
        return objs;
    }
    #endregion

    #region instantiate

    //instantiate
    //instantiate async
    public static AsyncOperationHandle<GameObject> InstantiateAsync(object key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
    {
        return Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
    }
    public static AsyncOperationHandle<GameObject> InstantiateAsync(object key, Vector3 position, Quaternion rotation, Transform parent = null, bool trackHandle = true)
    {
        return Addressables.InstantiateAsync(key, position, rotation, parent, trackHandle);
    }
    //instantiate sync
    public static GameObject InstantiateSync(object key, Transform parent = null, bool instantiateInWorldSpace = false)
    {
        var handle = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace);
        var obj = handle.WaitForCompletion();
        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            return obj;
        }
        else
        {
            Debug.LogError($"failed to fetch key {key}");
            return null;
        }
    }
    public static GameObject InstantiateSync(object key, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        var handle = Addressables.InstantiateAsync(key, position, rotation, parent);
        var obj = handle.WaitForCompletion();
        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            return obj;
        }
        else
        {
            Debug.LogError($"failed to fetch key {key}");
            return null;
        }
    }
    #endregion

    #region unload

    //unload
    public static void Release(AsyncOperationHandle assetReference)
    {
        Addressables.Release(assetReference);
    }
    public static void Release(object assetReference)
    {
        Addressables.Release(assetReference);
    }
    public static void Release(AssetReference assetReference)
    {
        Addressables.Release(assetReference);
    }
    public static void Release(string assetReference)
    {
        Addressables.Release(assetReference);
    }


    public static AsyncOperationHandle<SceneInstance> UnloadSceneAsync(object key, bool autoReleaseHandle = true)
    {
        var handler = obejctToHandler[key];
        obejctToHandler.Remove(key);
        return Addressables.UnloadSceneAsync(handler, autoReleaseHandle);
    }

    #endregion



    #region Lifetime Management

    public static void AddAutoReleaseAssetTrigger(AsyncOperationHandle assetReference, GameObject targetGO)
    {
        targetGO.AddComponent<ReleaseHandleOnDestroy>().OnDestroyEvent +=
            () => {
                Addressables.Release(assetReference);
            };
    }
    public static void AddAutoReleaseAssetTrigger(object assetReference, GameObject targetGO)
    {
        targetGO.AddComponent<ReleaseHandleOnDestroy>().OnDestroyEvent +=
            () => {
                Addressables.Release(assetReference);
            };
    }


    public static void AddAutoReleaseAssetTrigger(AssetReference assetReference, GameObject targetGO)
    {
        targetGO.AddComponent<ReleaseHandleOnDestroy>().OnDestroyEvent +=
            () => {
                Addressables.Release(assetReference);
            };
    }
    public static void AddAutoReleaseAssetTrigger(string key, GameObject targetGO)
    {
        targetGO.AddComponent<ReleaseHandleOnDestroy>().OnDestroyEvent +=
            () => { Addressables.Release(key); };
    }
    #endregion
}
