using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static ARSession aRSession;

    private void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackFromCurrentScene();
        }
#endif
    }

    public void BackFromCurrentScene()
    {
        if (GameInfo.ins.ScnenName.Equals(""))
        {
            if (Application.CanStreamedLevelBeLoaded("Main"))
            {
                PlaceOnPlane.isObjectPlaced = false;
                MultipleObjectPlacement.isObjectPlaced = false;
                Destroy(PlaceOnPlane.spawnedObject);
                //PrefabMaterialHandler.SpawningObjectMaterials = null;
                SceneManager.LoadScene("Main", LoadSceneMode.Single);
                LoaderUtility.Deinitialize();
                //StartCoroutine(LoadYourAsyncScene());
            }
        }
        else
        {
            if (Application.CanStreamedLevelBeLoaded(GameInfo.ins.ScnenName))
            {
                PlaceOnPlane.isObjectPlaced = false;
                MultipleObjectPlacement.isObjectPlaced = false;
                Destroy(PlaceOnPlane.spawnedObject);
                //PrefabMaterialHandler.SpawningObjectMaterials = null;
                SceneManager.LoadScene(GameInfo.ins.ScnenName);
                LoaderUtility.Deinitialize();
                //StartCoroutine(LoadYourAsyncScene());
            }
        }
    }

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
