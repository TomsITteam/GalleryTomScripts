using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArSceneManager : MonoBehaviour
{
	private void OnEnable()
	{
		Screen.orientation = ScreenOrientation.Portrait;
		//Invoke("autorotateSetting", 0.3f);
	}

	void autorotateSetting()
	{
		Screen.orientation = ScreenOrientation.AutoRotation;

		Screen.autorotateToPortrait = true;
		Screen.autorotateToPortraitUpsideDown = true;
		Screen.autorotateToLandscapeLeft = false;
		Screen.autorotateToLandscapeRight = false;
	}
}
