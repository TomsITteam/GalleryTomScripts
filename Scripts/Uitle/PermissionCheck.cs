using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class PermissionCheck : MonoBehaviour
{
    public GameObject Popup;
    public Text Title;
    public Text Message;
    public Button Yes;
    public Button No;

    [Tooltip("카메라")]
    public bool permission_Camera;
    [Tooltip("마이크")]
    public bool permission_Microphone;
    [Tooltip("부정확한 위치(네트워크만 사용)")]
    public bool permission_FineLocation;
    [Tooltip("외부 저장소에서 읽기")]
    public bool permission_ExternalStorageRead;
    [Tooltip("외부 저장소에 쓰기")]
    public bool permission_ExternalStorageWrite;

    private void Start()
    {
        
    }
}
