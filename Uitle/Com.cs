using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Com : Single<Com>
{
    // 카메라
    // Camera = "android.permission.CAMERA";
    // 마이크
    //Microphone = "android.permission.RECORD_AUDIO";
    // 정확한 위치(GPS, 네트워크 모두 사용)
    // FineLocation = "android.permission.ACCESS_FINE_LOCATION";
    // 부정확한 위치(네트워크만 사용)
    // CoarseLocation = "android.permission.ACCESS_COARSE_LOCATION";
    // 외부 저장소에서 읽기
    // ExternalStorageRead = "android.permission.READ_EXTERNAL_STORAGE";
    // 외부 저장소에 쓰기
    // ExternalStorageWrite = "android.permission.WRITE_EXTERNAL_STORAGE";

    GameObject BgmSound = null;

    ///카메라 스케일 관련
    public void CameraScaler(float widethSize, float heightSize,Camera gCamera = null)
    {
        Camera mCamera = null;
        if (gCamera != null)
            mCamera = gCamera;
        else
            mCamera = Camera.main;

        if (mCamera == null)
            return;

        Rect rect = mCamera.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / (widethSize / heightSize); // (가로 / 세로)
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }
        mCamera.rect = rect;
    }

    public Vector2 ScreenWorldPosition(Camera CanvasCam, RectTransform CanvasRTrans, Vector3 TargetPos)
    {
        Vector2 ViewportPosition = CanvasCam.WorldToViewportPoint(TargetPos);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRTrans.sizeDelta.x) - (CanvasRTrans.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRTrans.sizeDelta.y) - (CanvasRTrans.sizeDelta.y * 0.5f)));

        return WorldObject_ScreenPosition;
    }

    /// <summary>
    /// 베지어 곡선의 포인트를 찾는다
    /// </summary>
    public Vector3 GetPointOnBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1f - t;
        float t2 = t * t;
        float u2 = u * u;
        float u3 = u2 * u;
        float t3 = t2 * t;

        Vector3 result =
            (u3) * p0 +
            (3f * u2 * t) * p1 +
            (3f * u * t2) * p2 +
            (t3) * p3;

        return result;
    }

    /// <summary>
    /// 전체값에서 일부값은 몇 퍼센트
    /// </summary>
    public int percentageOfTotal(int now, int max){
        //전체값에서 일부값은 몇 퍼센트
        //(전체값 X 100) ÷ 일부값
        return ((max*100)/now);
    }
    public float percentageOfTotal(float now, float max){
        return ((max*100)/now);
    }

    /// <summary>
    ///숫자를 몇 퍼센트 증가시키는 공식
    /// </summary>
    public int percentIncrease(int num, int percent){
        //숫자를 몇 퍼센트 증가시키는 공식
        //숫자 X (1 + 퍼센트 ÷ 100)
        return ((num * (1+percent)) / 100);
    }
    public float percentIncrease(float num, float percent){
        return ((num * (1+percent)) / 100);
    }
    
    ///<summary>
    ///숫자를 몇 퍼센트 감소하는 공식
    ///</summary>
    public int percentReduce(int num, int percent){
        //숫자를 몇 퍼센트 감소하는 공식.
        //숫자 X (1 - 퍼센트 ÷ 100)
        return ((num * (1-percent)) / 100);
    }
    public float percentReduce(float num, float percent){
        return ((num * (1-percent)) / 100);
    }

    public void ShuffleArray<T>(T[] array)
    {
        int random1;
        int random2;

        T tmp;

        for (int index = 0; index < array.Length; ++index)
        {
            random1 = UnityEngine.Random.Range(0, array.Length);
            random2 = UnityEngine.Random.Range(0, array.Length);

            tmp = array[random1];
            array[random1] = array[random2];
            array[random2] = tmp;
        }
    }

    public void ShuffleList<T>(List<T> list)
    {
        int random1;
        int random2;

        T tmp;

        for (int index = 0; index < list.Count; ++index)
        {
            random1 = UnityEngine.Random.Range(0, list.Count);
            random2 = UnityEngine.Random.Range(0, list.Count);

            tmp = list[random1];
            list[random1] = list[random2];
            list[random2] = tmp;
        }
    }

    public bool TargetRay2D_Bool(Camera mCam, string TagName)
    {
        Vector2 wp = mCam.ScreenToWorldPoint(Input.mousePosition);
        Ray2D ray = new Ray2D(wp, Vector2.zero);
        RaycastHit2D hitCheck = Physics2D.Raycast(ray.origin, ray.direction);
        if (hitCheck.transform != null)
        {
            if (hitCheck.transform.tag.Equals(TagName))
            {
                return true;
            }
        }
        return false;
    }

    private Transform FindChildByName(string ThisName,Transform ThisGObj ) 
    {
        Transform ReturnObj;
       /*- ---->WHAT IS HE DOING HERE?<------------ */
        if(ThisGObj.name==ThisName )
            return ThisGObj.transform;
         /*- ---->WHAT IS HE DOING HERE?<------------ */
        foreach(Transform child in ThisGObj )
        {
            ReturnObj = FindChildByName(ThisName, child );
            if(ReturnObj )
                return ReturnObj;
        }
        return null;
    }

    public void AniSetInt(Animator ani, string IntName, int value)
    {
        if (ani.GetInteger(IntName) != value)
        {
            //Debug.Log(IntName + " [" + value + "]");
            ani.SetInteger(IntName, value);
        }
    }

    public Vector2 SizeToParent(RawImage image, float spacing = 0, float padding = 0)
    {
        float w = 0, h = 0;
        var parent = image.transform.parent.GetComponent<RectTransform>();
        var imageTransform = image.GetComponent<RectTransform>();

        // check if there is something to do
        if (image.texture != null)
        {
            if (!parent) { return imageTransform.sizeDelta; } //if we don't have a parent, just return our current width;
            padding = 1 - padding;
            float ratio = image.texture.width / (float)image.texture.height;
            var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
            if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
            {
                //Invert the bounds if the image is rotated
                bounds.size = new Vector2(bounds.height, bounds.width);
            }
            //Size by height first
            h = bounds.height * padding;
            w = h * ratio;
            if (w > bounds.width * padding)
            { //If it doesn't fit, fallback to width;
                w = bounds.width * padding;
                h = w / ratio;
            }

            h -= spacing;
            w -= spacing;
        }
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        return imageTransform.sizeDelta;
    }
    public void BgmSoundPlay(AudioClip SoundSrc, bool Loop = true)
    {
        if (SoundSrc == null)
        {
            return;
        }

        if (BgmSound != null)
        {
            return;
        }

        BgmSound = new GameObject("Bgm");

        DontDestroyOnLoad(BgmSound);

        BgmSound.transform.position = Vector3.zero;
        BgmSound.transform.SetParent(transform);
        AudioSource BgmSource = BgmSound.AddComponent<AudioSource>();
        BgmSource.clip = SoundSrc;
        BgmSource.minDistance = 10.0f;
        BgmSource.maxDistance = 30.0f;
        BgmSource.volume = 1;

        BgmSource.spatialBlend = 0;

        BgmSource.loop = Loop;
        BgmSource.Play();

        //if (!Loop)
        //    Destroy(BgmSource, SoundSrc.length);
    }

    public void BgmStop()
    {
        Destroy(BgmSound);
    }

    public void SoundPlay(AudioClip SoundSrc,float _volume = 1, bool Sound3D = false, bool Loop = false)
    {
        if (SoundSrc == null)
            return;

        GameObject soundObj = new GameObject("Sound");

        DontDestroyOnLoad(soundObj);

        soundObj.transform.position = Vector3.zero;
        soundObj.transform.SetParent(transform);
        AudioSource _audioSource = soundObj.AddComponent<AudioSource>();
        _audioSource.clip = SoundSrc;
        _audioSource.minDistance = 10.0f;
        _audioSource.maxDistance = 30.0f;
        _audioSource.volume = _volume;

        if (!Sound3D)
            _audioSource.spatialBlend = 0;
        else
            _audioSource.spatialBlend = 1;
        
        _audioSource.loop = Loop;
        _audioSource.Play();

        if (!Loop)
            Destroy(soundObj, SoundSrc.length);
    }

    public void SoundPlay(AudioClip SoundSrc, Vector3 Pos, float _volume = 1, bool Loop = false)
    {
        if (SoundSrc == null)
            return;

        GameObject soundObj = new GameObject("Sound");

        DontDestroyOnLoad(soundObj);

        soundObj.transform.position = Pos;
        soundObj.transform.SetParent(transform);
        AudioSource _audioSource = soundObj.AddComponent<AudioSource>();
        _audioSource.clip = SoundSrc;
        _audioSource.minDistance = 10.0f;
        _audioSource.maxDistance = 30.0f;
        _audioSource.volume = _volume;
        _audioSource.spatialBlend = 1;
        _audioSource.loop = Loop;

        _audioSource.Play();
        if (!Loop)
            Destroy(soundObj, SoundSrc.length);
    }

    public void SoundPlay(AudioClip SoundSrc, Vector3 Pos, Transform ObjectTrans, float _volume = 1, bool Loop = false)
    {
        if (SoundSrc == null)
            return;

        GameObject soundObj = new GameObject("Sound");

        DontDestroyOnLoad(soundObj);

        soundObj.transform.position = Pos;
        soundObj.transform.SetParent(ObjectTrans);
        AudioSource _audioSource = soundObj.AddComponent<AudioSource>();
        _audioSource.clip = SoundSrc;
        _audioSource.minDistance = 10.0f;
        _audioSource.maxDistance = 30.0f;
        _audioSource.volume = _volume;
        _audioSource.spatialBlend = 1;
        _audioSource.loop = Loop;

        _audioSource.Play();
        if (!Loop)
            Destroy(soundObj, SoundSrc.length);
    }
}
