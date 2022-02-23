
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestAddressablesMaster : MonoBehaviour
{

    //text
    public AssetReferenceTextAsset textAsset;
    public string textAssetString;
    public Text textLabel;

    //sound
    AudioSource audioSource;
    public AssetReferenceAudioClip clip;
    public string clipString;

    //object
    public AssetReferenceGameObject obj;
    public string objString;

    //scene
    public string sceneString;
    public AssetReferenceScene scene;


    //material
    public AssetReferenceMaterial material;
    public string materialString;


    //label
    public AssetLabelReference assetLabel;
    public string assetLabelString;

    AsyncOperationHandle loadHandler;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //// text
        Debug.Log($"text has loaded: {AddressableManager.isAssetLoaded(textAssetString)}");
        //load with adressable name async and do nothing
        AddressableManager.LoadAssetAsyncAutoRelease<TextAsset>(textAssetString, gameObject);

        ////// load with public AssetReferenceTextAsset
        textLabel.text = AddressableManager.LoadAssetAutoRelease<TextAsset>(textAsset, gameObject).text;
        ////// load with addressable name
        textLabel.text = AddressableManager.LoadAssetAutoRelease<TextAsset>(textAssetString, gameObject).text;
        ////// load with addressable name sync with action
        AddressableManager.LoadAssetAutoRelease<TextAsset>(textAssetString, gameObject, result =>
         {
             textLabel.text = result.text;
         });
        ////// load with addressable name async with action
        AddressableManager.LoadAssetAsyncAutoRelease<TextAsset>(textAssetString, gameObject).Completed += result =>
         {
             if (result.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
             {
                 textLabel.text = result.Result.text;
             }
         };
        //loadAsset and save the handler to release later;
        loadHandler = AddressableManager.LoadAsset<TextAsset>(textAssetString);

        //load asset and add auto release trigger later
        var handler = AddressableManager.LoadAsset<TextAsset>(textAssetString);
        AddressableManager.AddAutoReleaseAssetTrigger(handler, gameObject);


        Debug.Log($"text has loaded: {AddressableManager.isAssetLoaded(textAssetString)}");


        ////game object

        //// load asset and instantiate it
        var handleInstance = AddressableManager.InstantiateAsync(obj, transform);

        var prefab = AddressableManager.LoadAssetAutoRelease<GameObject>(objString, gameObject);
        var instanceObj = Instantiate(prefab);

        //// directly sync instantiate it 
        var instanceObj2 = AddressableManager.InstantiateSync(obj, new Vector3(1, 1, 1), Quaternion.identity);

        ////direct async instantiate it

        AsyncOperationHandle<GameObject> objHandler = AddressableManager.InstantiateAsync(obj, transform);
        objHandler.WaitForCompletion();
        ////AddressableManager.AddAutoReleaseAssetTrigger(clipString, audioSource.gameObject);
        ////material

        instanceObj.GetComponent<Renderer>().material = AddressableManager.LoadAssetAutoRelease<Material>(material, instanceObj);
        AddressableManager.LoadAssetAsyncAutoRelease<Material>(materialString, gameObject).Completed += result =>
         {
             if (result.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
             {
                 instanceObj2.GetComponent<Renderer>().material = result.Result;
                 objHandler.Result.GetComponent<Renderer>().material = result.Result;
             }
         };



        ////sound
        ///
        AddressableManager.LoadAssetAsyncAutoRelease<AudioClip>(clip, audioSource.gameObject).Completed += result =>
         {
             if (result.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
             {
                 audioSource.PlayOneShot(result.Result);
             }
         };
        var cc = AddressableManager.LoadAssetAutoRelease<AudioClip>(clipString, audioSource.gameObject);
        audioSource.PlayOneShot(cc);


        ////labels
        IList<GameObject> gos = AddressableManager.LoadAssetsAutoRelease<GameObject>(assetLabel, gameObject);
        foreach (var go in gos)
        {
            Instantiate(go);
        }


        AddressableManager.LoadAssetsAsyncAutoRelease<GameObject>(assetLabelString, gameObject).Completed += objects =>
            {
                if (objects.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {

                    foreach (var go in objects.Result)
                        Debug.Log($"Addressable Loaded: {go.name}");
                }
                else
                {
                    Debug.Log($"???");
                }
            };


    }



    public void gotoNextScene()
    {
        //scene
        AddressableManager.LoadSceneAsync("secondScene");
    }

    public void gotoAnotherScene()
    {
        //scene
        AddressableManager.LoadScene(scene);
    }

    private void OnDisable()
    {
        AddressableManager.Release(loadHandler);
        // clip.ReleaseAsset();
        // obj.ReleaseAsset();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
