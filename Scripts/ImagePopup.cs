using echo17.EndlessBook;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ImagePopup : MonoBehaviour
{
    [System.Serializable]
    public class TextView
    {
        public int PageNumber;
        public GameObject viewObject;
    }

    public int NowPageIdx = 0;

    public Transform PageViewText;
    public GameObject[] PageButton;

    [SerializeField]
    private List<TextView> PageTextList = new List<TextView>();


    private void Awake()
    {
        PageTextList.Clear();
        if (PageViewText != null)
        {
            foreach (Transform child in PageViewText)
            {
                TextView _temp = new TextView();

                string _strNum = Regex.Replace(child.name, @"\D", "");
                int _num = System.Convert.ToInt32(_strNum);

                _temp.PageNumber = _num;
                _temp.viewObject = child.gameObject;

                PageTextList.Add(_temp);
                child.gameObject.SetActive(false);
            }
        }

    }

    public void OnClick_OpenPopup(int leftPage, int rightPage)
    {
        bool DataCheck = false;
        gameObject.SetActive(true);
        for (int i = 0; i < PageTextList.Count; i++)
        {
            PageTextList[i].viewObject.SetActive(false);
        }


        int HaveData_Left = -1;
        int HaveData_Right = -1;

        PageButton[0].SetActive(false);
        PageButton[1].SetActive(false);

        //FindIndex 보다 for문이 빠르다
        //int get = GameInfo.ins.pageList.FindIndex(a => a.pageNumber == leftPage);
        //int get2 = GameInfo.ins.pageList.FindIndex(a => a.pageNumber == rightPage);

        for (int i = 0; i < GameInfo.ins.pageList.Count; i++)
        {
            if (GameInfo.ins.pageList[i].pageNumber == leftPage)
            {
                DataCheck = true;
                HaveData_Left = i;
            }
            else if (GameInfo.ins.pageList[i].pageNumber == rightPage)
            {
                HaveData_Right = i;
                DataCheck = true;
            }
        }

        if (HaveData_Left >= 0 && HaveData_Right >= 0)
        { //둘다 존재 할때 버튼 활성화
            StartCoroutine(PopupSetting(HaveData_Left));
        }
        else if (HaveData_Left < 0 && HaveData_Right >= 0)
        { //우측 페이지만 데이터 존재
            StartCoroutine(PopupSetting(HaveData_Right));
        }
        else if (HaveData_Right < 0 && HaveData_Left >= 0)
        { //좌측 페이지만 데이터 존재할때
            StartCoroutine(PopupSetting(HaveData_Left));
        }

        if (!DataCheck)
            gameObject.SetActive(false);

    }

    public void OnClick_ClosePopup()
    {
        gameObject.SetActive(false);
        //BookManager.OnTurnButtonClicked(NowPageIdx);
    }

    public void OnClick_ViewPage_Loading(int index)
    {
        NowPageIdx += index; 
        StartCoroutine(PopupSetting(NowPageIdx));
    }

    IEnumerator PopupSetting(int _idx)
    {
        NowPageIdx = _idx;

        yield return null;

        //Object 의 on off 로 변경
        for (int i = 0; i < PageTextList.Count; i++)
        {
            PageTextList[i].viewObject.SetActive(false);

            if (_idx + 1 == PageTextList[i].PageNumber)
            {
                PageTextList[i].viewObject.SetActive(true);
            }

            yield return null;
        }

        yield return null;

        PageButton[0].SetActive(true);
        PageButton[1].SetActive(true);

        if (_idx <= 0)
            PageButton[0].SetActive(false);
        if(_idx > PageTextList.Count - 2)
            PageButton[1].SetActive(false);
    }

    Vector2 SizeToParent(RawImage image, float spacing = 0, float padding = 0)
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
}
