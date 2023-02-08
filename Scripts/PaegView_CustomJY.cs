using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using echo17.EndlessBook.Demo02;

public class PaegView_CustomJY : PageView
{
	[Header("페이지 사이즈")]
	/// <summary>
	/// 1 = 1page, 2 = 양쪽 페이지용
	/// </summary>
	public int Size = 1;

	[Header("원본 주소")]
    public string DataAdr = "";

	[SerializeField]
	public pageItem data = new pageItem();

	private void Start()
	{
		bool addCehck = true;
		string debugStr = "";
		string namePaser = gameObject.name;
		int nameIndex = namePaser.IndexOf("_");

		SpriteRenderer _spr = gameObject.GetComponentInChildren<SpriteRenderer>();
		if (_spr != null)
		{
			debugStr += "SpriteRenderer Not Null Check ---- \n";
			data.DrawType = 2;

			data.ImageSrc = _spr.sprite;
		}

		VideoPlayer _video = gameObject.GetComponentInChildren<VideoPlayer>();
		if (_video != null)
		{
			debugStr += "VideoPlayer Not Null Check ---- \n";
			data.DrawType = 1;
			data.videoSrc = _video.targetTexture;
		}

		Image _img = gameObject.GetComponentInChildren<Image>();
		if(_img != null)
		{
			debugStr += "Canvas Image Not Null Check ---- \n";
			data.DrawType = 2;
			data.ImageSrc = _img.sprite;
		}

		Text _txt = gameObject.GetComponentInChildren<Text>();

		if (DataAdr != null)
		{
		}

		//Debug.Log(nameIndex);

		if (nameIndex >= 0)
		{
			var _name = namePaser.Substring(nameIndex + 1);
			debugStr += namePaser + " : " +_name + " [";

			//_ 이라는 값이 있다면
			if (int.TryParse(_name, out int retrunNum))
			{
				data.pageNumber = retrunNum;
			}
			else if (namePaser.Equals("Front"))
			{
				data.pageNumber = 0;
			}

			debugStr += data.pageNumber + "] \n";
		}

		for(int i = 0; i < GameInfo.ins.pageList.Count; i++)
        {
			if(GameInfo.ins.pageList[i].pageNumber == data.pageNumber)
            {
				addCehck = false;
				debugStr += "중복 데이터 확인 \n";
				break;
            }
		}

        if (addCehck)
        {
			GameInfo.ins.pageList.Add(data);
		}
		//Debug.Log(debugStr);
	}
}
