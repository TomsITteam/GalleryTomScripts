using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameInfo : Single<GameInfo>
{
	[SerializeField]
	public List<pageItem> pageList = new List<pageItem>();
    [SerializeField]
    public List<nftData> nftList = new List<nftData>();

    public bool BackSceneCheck = true;
    public string ScnenName = "";

    public int OldCameraState = 0;
    public int CameraState = 0;

    public GameObject ArObjectGroup;
    public GameObject LastObject;

    public List<GameObject> ArObjList = new List<GameObject>();
    public GameObject GetArObjectGroup()
    {
        if(ArObjectGroup == null)
        {
            ArObjectGroup = GameObject.Find("ArObjectGroup");
            if(ArObjectGroup == null)
            {
                ArObjectGroup = new GameObject("ArObjectGroup");
            }
        }
        return ArObjectGroup;
    }

    private void Awake()
    {
        nftList.Clear();
        ArObjList.Clear();

        CameraState = 0;
        //StartCoroutine(Read_NFT_Data());
    }
    IEnumerator Read_NFT_Data()
    {
        yield return null;

        List<Dictionary<string, object>> data = CSVReader.Read("nftData");

        yield return null;

        for (var i = 0; i < data.Count; i++)
        {
            nftData temp = new nftData();

            //Debug.Log("index " + (i).ToString() + " : " + data[i]["Name"] + " " + data[i]["Age"]);
            temp.AppID = System.Convert.ToInt32(data[i]["AppID"]);
            temp.Name = System.Convert.ToString(data[i]["Name"]);
            temp.Title = System.Convert.ToString(data[i]["Title"]);
            temp.Description = System.Convert.ToString(data[i]["Description"]);
            temp.srcAdr = System.Convert.ToString(data[i]["srcAdr"]);
            if(!temp.srcAdr.Equals("") && temp.srcAdr != null)
            {
                Debug.Log(temp.srcAdr);
                var _filePath = Path.GetExtension(temp.srcAdr);
                Debug.Log(_filePath);
            }


            if (data[i]["pageNumber"] != null)
            {
                if (!data[i]["pageNumber"].Equals(""))
                    temp.PageNumber = System.Convert.ToInt32(data[i]["pageNumber"]);
                else
                    temp.PageNumber = -2;
            }

            nftList.Add(temp);

            yield return null;
        }

        yield return null;
    }

    public void ListCheck()
    {
        int LastIndex;
        //list null object delet
        ArObjList.RemoveAll(_obj => _obj == null);

        LastIndex = ArObjList.Count - 1;
        if (LastIndex >= 0)
            LastObject = ArObjList[LastIndex];
        else
            ArObjList.Clear();
    }
}

[System.Serializable]
public class pageItem
{
	public int pageNumber = -1;
	/// <summary>
	/// 1 : Video, 2 : Image
	/// </summary>
	public int DrawType = 0;
	[Header("데이터 추출")]
	public string strTitle = "";
	public string strDescription = "";
	public RenderTexture videoSrc = null;
	public Sprite ImageSrc = null;
    public Image CavasImageSrc = null;

    [Header("텍스트 매쉬")]
    public TextMesh viewTitle;
    public TextMesh viewDescription;
    public Text ViewCanvasText;
}
[System.Serializable]
public class nftData
{
	public int AppID;
	public int PageNumber;
	public string Name;
	public string Title;
	public string Description;
    public string srcAdr;

}
