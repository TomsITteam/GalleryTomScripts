using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using echo17.EndlessBook;
using echo17.EndlessBook.Demo02;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class BookManager : MonoBehaviour
{
	/// <summary>
	/// The scene camera used for ray casting
	/// </summary>
	public Camera sceneCamera;

	/// <summary>
	/// The book to control
	/// </summary>
	public EndlessBook book;

	/// <summary>
	/// The speed to play the page turn animation when the mouse is let go
	/// </summary>
	public float turnAnimTimeSpeed = 1;

	/// <summary>
	/// If this is turned on, then the page will reverse direction
	/// if the page is not past the midway point of the book.
	/// </summary>
	public bool reversePageIfNotMidway = true;

	protected BoxCollider boxCollider;
	protected bool isMouseDown;

	/// <summary>
	/// Whether pages are being flipped.
	/// 페이지가 뒤집혔는지 여부
	/// </summary>
	protected bool flipping = false;
	/// <summary>
	/// How fast the state change animations should be.
	/// 상태 변경 애니메이션의 속도
	/// </summary>
	public float openCloseTime = 0.3f;

	public ImagePopup imgPopup;

	[Header("Page Views 내부 파일 연결")]
	/// <summary>
	/// The mini-scenes that are rendered to textures for the book pages.
	/// 책 페이지의 텍스처로 렌더링되는 미니 장면
	/// </summary>
	public PageView[] pageViews;

	private int OpenLeftPageNumber = -1;
	private int OpenRightPageNumber = -1;
	private int TempLeftPage;
	private int TempRightPage;
	private Page.TurnDirectionEnum turnDirection;

	public GameObject IntroCamera;
	private Camera cameraIntro;
	private Animator introAni;
	public float IntroAniSpeed = 1;

	[Header("사운드 관련")]
	public GameObject SoundObject;
	public AudioClip Sound_Close;
	public AudioClip Sound_Open;
	public AudioClip Sound_Fliping;
	public AudioClip Sound_Turn;

	void Awake()
	{
		InitCamera();

		//추후 최초 로딩 창 등에서 초기화
		GameInfo.ins.pageList.Clear();

		// cache the box collider for faster referencing
		boxCollider = gameObject.GetComponent<BoxCollider>();
		imgPopup.gameObject.SetActive(false);
		TurnOffAllPageViews();
	}

    private void OnEnable()
	{
		Screen.orientation = ScreenOrientation.Landscape;
		//Invoke("autorotateSetting", 0.3f);
	}

	void autorotateSetting()
	{
		Screen.orientation = ScreenOrientation.AutoRotation;

		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
	}

    #region Book Demo Copy

    /// <summary>
    /// Fired when the mouse intersects with the collider box while mouse down occurs
    /// </summary>
    void OnMouseDown()
	{
		if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
			return;
		if (book.IsTurningPages || book.IsDraggingPage)
		{
			// exit if already turning
			return;
		}

		// get the normalized time based on the mouse position
		var normalizedTime = GetNormalizedTime();

		// calculate the direction of the page turn based on the mouse position
		var direction = normalizedTime > 0.5f ? Page.TurnDirectionEnum.TurnForward : Page.TurnDirectionEnum.TurnBackward;

		if (book.CurrentState == EndlessBook.StateEnum.OpenMiddle)
		{
			// tell the book to start turning a page manually
			book.TurnPageDragStart(direction);

            if (direction == Page.TurnDirectionEnum.TurnBackward)
            {
                TempLeftPage = book.CurrentLeftPageNumber - 2;
                TempRightPage = book.CurrentRightPageNumber - 2;
				//Debug.Log("Touch start Back Ward : " + book.CurrentLeftPageNumber + "[ "  + TempLeftPage  + " : "+ +TempRightPage + "]");
			}
            if (direction == Page.TurnDirectionEnum.TurnForward)
            {
				TempLeftPage = book.CurrentLeftPageNumber + 2;
				TempRightPage = book.CurrentRightPageNumber + 2;
				//Debug.Log("Touch start Forward : " + book.CurrentRightPageNumber + "[ " + TempLeftPage + " : " + +TempRightPage + "]");
			}
			MouseDownPageLoade();

		}
		turnDirection = direction;

		// the mosue is now currently down
		isMouseDown = true;
	}

	/// <summary>
	/// Fired when the mouse intersects with the collider box while dragging
	/// </summary>
	void OnMouseDrag()
	{
		if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
			return;
		if (book.IsTurningPages || !book.IsDraggingPage || !isMouseDown)
		{
			// if not turning or the mouse is not down, then exit
			return;
		}

		// get the normalized time based on the mouse position
		var normalizedTime = GetNormalizedTime();
		//Debug.Log(normalizedTime);
		if (normalizedTime < 0)
			normalizedTime = 0;
		if (normalizedTime > 1)
			normalizedTime = 1;

		// tell the book to move the manual page drag to the normalized time
		book.TurnPageDrag(normalizedTime);
	}

	/// <summary>
	/// Fired when the mouse intersects with the collider and the mouse up event occurs
	/// </summary>
	void OnMouseUp()
	{
		if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
			return;

		// mouse is no longer down, so we can turn a new page if the animation is also completed
		isMouseDown = false;

		switch (book.CurrentState)
		{
			case EndlessBook.StateEnum.ClosedFront:
				if (turnDirection == Page.TurnDirectionEnum.TurnForward)
				{
                    //onClick_OpenFront();

                    OnBookPage_Fist();
                    onClick_OpenMiddle();

					SoundPlay(Sound_Open);

					//OpenTurnPage();
				}
				return;
			case EndlessBook.StateEnum.OpenFront:
				if (turnDirection == Page.TurnDirectionEnum.TurnBackward)
				{
					onClick_ClosedFront();
				}
				if (turnDirection == Page.TurnDirectionEnum.TurnForward)
				{
					OnBookPage_Fist();
					onClick_OpenMiddle();
				}
				return;
			case EndlessBook.StateEnum.OpenMiddle:
				
				if (book.IsFirstPageGroup)
				{
					if (turnDirection == Page.TurnDirectionEnum.TurnBackward)
					{
						//onClick_OpenFront();
						onClick_ClosedFront();
						SoundPlay(Sound_Close);
						return;
					}
				}
				if (book.IsLastPageGroup)
				{
					if (turnDirection == Page.TurnDirectionEnum.TurnForward)
					{
						//onClick_OpenBack();
						onClick_ClosedBack();
						SoundPlay(Sound_Close);
						return;
					}
				}
				//페이지가 돌아가고 있습니까? || 페이지를 수동으로 드래그하고 있습니까?
				if (book.IsTurningPages || !book.IsDraggingPage)
				{
					// if not turning then exit
					return;
				}
				// 책에 수동 회전을 중지하도록 지시합니다.
				// reversePageIfNotMidway가 켜져 있으면 중간 지점을 지나쳤는지 확인합니다.
				// 그렇지 않으면 페이지를 뒤집습니다.
				float normalizedTime = book.TurnPageDragNormalizedTime;
				if (normalizedTime > 1)
					normalizedTime = 1;
				book.TurnPageDragStop(turnAnimTimeSpeed, PageTurnCompleted, reverse: reversePageIfNotMidway ? (normalizedTime < 0.5f) : false);
				//Debug.Log(reversePageIfNotMidway + " :: " + book.TurnPageDragNormalizedTime);
				if(book.TurnPageDragNormalizedTime < 0.5f)
                {
					MouseUpPageClose();
				}
				SoundPlay(Sound_Turn);
				break;
			case EndlessBook.StateEnum.OpenBack:
				if (turnDirection == Page.TurnDirectionEnum.TurnBackward)
				{
					OnBookPage_Last();
					onClick_OpenMiddle();
				}
				if (turnDirection == Page.TurnDirectionEnum.TurnForward)
				{
					onClick_ClosedBack();
				}
				break;
			case EndlessBook.StateEnum.ClosedBack:
				if (turnDirection == Page.TurnDirectionEnum.TurnBackward)
				{
					OnBookPage_Last();
					onClick_OpenMiddle();

					Com.ins.SoundPlay(Sound_Open);
					//onClick_OpenBack();
				}
				break;
		}
	}

	/// <summary>
	/// 마우스 위치를 기반으로 정규화된 시간을 계산합니다.
	/// </summary>
	protected virtual float GetNormalizedTime()
	{
		// get the ray from the camera to the screen
		var ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		// cast a ray and see where it hits
		if (Physics.Raycast(ray, out hit))
		{
			// return the position of the ray cast in terms of the normalized position of the collider box
			return (hit.point.x + (boxCollider.size.x / 2.0f)) / boxCollider.size.x;
		}

		// if we didn't hit the collider, then check to see if we are on the
		// left or right side of the screen and calculate the normalized time appropriately
		var viewportPoint = sceneCamera.ScreenToViewportPoint(Input.mousePosition);
		return (viewportPoint.x >= 0.5f) ? 1 : 0;
	}

	private void PageTurnCompleted(int leftPageNumber, int rightPageNumber)
	{
		//페이지가 수동 회전을 완료할 때 호출됩니다.
		//Debug.Log(leftPageNumber + " : " + rightPageNumber);
		OnBookPage_Open(leftPageNumber, rightPageNumber);
	}

	/// <summary>
	/// Deactivates all the page mini-scenes
	/// </summary>
	protected virtual void TurnOffAllPageViews()
	{
		for (var i = 0; i < pageViews.Length; i++)
		{
			if (pageViews[i] != null)
			{
				pageViews[i].Deactivate();
			}
		}
	}
	
	/// <summary>
	/// Gets the page view mini-scene of a page number
	/// </summary>
	/// <param name="pageNumber">The page number</param>
	/// <returns></returns>
	private PageView GetPageView(int pageNumber)
	{
		// search for a page view.
		// 0 = front page,
		// 999 = back page
		return pageViews.Where(x => x.name == string.Format("PageView_{0}", (pageNumber == 0 ? "Front" : (pageNumber == 999 ? "Back" : pageNumber.ToString("00"))))).FirstOrDefault();
	}

	/// <summary>
	/// Turns a page mini-scene on or off
	/// </summary>
	/// <param name="pageNumber">The page number</param>
	/// <param name="on">Whether the mini-scene is on or off</param>
	protected virtual void TogglePageView(int pageNumber, bool on)
	{
		var pageView = GetPageView(pageNumber);

		if (pageView != null)
		{
			if (pageView != null)
			{
				if (on)
				{
					pageView.Activate();
				}
				else
				{
					pageView.Deactivate();
				}
			}
		}
	}

	/// <summary>
	/// Set the state
	/// </summary>
	/// <param name="state">The state to set to</param>
	protected virtual void SetState(EndlessBook.StateEnum state)
	{
		// set the state
		book.SetState(state, openCloseTime, OnBookStateChanged);
	}

	/// <summary>
	/// Set the ClosedFront state
	/// </summary>
	public void onClick_ClosedFront()
	{
		SetState(EndlessBook.StateEnum.ClosedFront);
	}

	/// <summary>
	/// Set the OpenFront state
	/// </summary>
	public void onClick_OpenFront()
	{
		// toggle the front page view
		OnBookPage_Fist();

		SetState(EndlessBook.StateEnum.OpenFront);
	}

	/// <summary>
	/// Set the OpenMiddle state
	/// </summary>
	public void onClick_OpenMiddle()
	{
        // toggle the left and right page views
        TogglePageView(book.CurrentLeftPageNumber, true);
        TogglePageView(book.CurrentRightPageNumber, true);

        SetState(EndlessBook.StateEnum.OpenMiddle);
	}

	/// <summary>
	/// Set the OpenBack state
	/// </summary>
	public void onClick_OpenBack()
	{
		TurnOffAllPageViews();
		// toggle the back page view
		TogglePageView(999, true);

		SetState(EndlessBook.StateEnum.OpenBack);
	}

	/// <summary>
	/// Set the ClosedBack state
	/// </summary>
	public void onClick_ClosedBack()
	{
		SetState(EndlessBook.StateEnum.ClosedBack);
	}

	/// <summary>
	/// Called when the book's state changes
	/// </summary>
	/// <param name="fromState">Previous state</param>
	/// <param name="toState">Current state</param>
	/// <param name="pageNumber">Current page number</param>
	protected virtual void OnBookStateChanged(EndlessBook.StateEnum fromState, EndlessBook.StateEnum toState, int pageNumber)
	{
		switch (toState)
		{
			case EndlessBook.StateEnum.ClosedFront:
			case EndlessBook.StateEnum.ClosedBack:
				// play the closed sound

				// turn off page mini-scenes
				TurnOffAllPageViews();
				break;

			case EndlessBook.StateEnum.OpenMiddle:
				if (fromState != EndlessBook.StateEnum.OpenMiddle)
				{
					// play open sound
					//SoundPlay(Sound_Turn);
				}
				else
				{
					// stop the flipping sound
					flipping = false;
					Destroy(SoundObject);
					//pagesFlippingSound.Stop();
				}
				// turn off the front and back page mini-scenes
				TogglePageView(0, false);
				TogglePageView(999, false);
				break;

			case EndlessBook.StateEnum.OpenFront:
				TogglePageView(0, true);
				break;
			case EndlessBook.StateEnum.OpenBack:
				// play the open sound
				//bookOpenSound.Play();
				break;
		}
	}

	public virtual void OnPageButtonClicked(int pageNumber)
	{
		var newLeftPageNumber = pageNumber % 2 == 0 ? pageNumber - 1 : pageNumber;

		// play the flipping sound if more than a single page is turning
		if (Mathf.Abs(newLeftPageNumber - book.CurrentLeftPageNumber) > 2)
		{
			SoundPlay(Sound_Fliping);
		}

		book.TurnToPage(pageNumber == 999 ? book.LastPageNumber : pageNumber,
			EndlessBook.PageTurnTimeTypeEnum.TotalTurnTime,
			turnAnimTimeSpeed,
					onCompleted: OnBookStateChanged,
					onPageTurnStart: OnPageTurnStart,
					onPageTurnEnd: OnPageTurnEnd
			);
	}

	public virtual void OnTurnButtonClicked(int direction)
	{
		int pageNum = book.CurrentPageNumber + direction;
        //Debug.Log(pageNum + " ... LastPage : " + book.LastPageNumber);
        int LastPage_sniffling = 1;

        if (book.LastPageNumber % 2 == 0)
            LastPage_sniffling = 2;
        else
            LastPage_sniffling = 1;

        if (direction == -1)
		{
			if (pageNum <= 0)
			{
				onClick_ClosedFront();
				SoundPlay(Sound_Close);
			}
			else if (pageNum >= book.LastPageNumber - LastPage_sniffling)
			{
				OnBookPage_Last();
				onClick_OpenMiddle();

				SoundPlay(Sound_Open);
			}
			else
				SoundPlay(Sound_Turn);
			book.TurnBackward(turnAnimTimeSpeed, onCompleted: OnBookStateChanged, onPageTurnStart: OnPageTurnStart, onPageTurnEnd: OnPageTurnEnd);
		}
		else
		{
			if (pageNum <= 2)
			{
				OnBookPage_Fist();
				onClick_OpenMiddle();

				Com.ins.SoundPlay(Sound_Open);
			}
			else if (pageNum >= book.LastPageNumber)
			{
				onClick_ClosedBack();
				Com.ins.SoundPlay(Sound_Close);
			}
			else
				SoundPlay(Sound_Turn);
			book.TurnForward(turnAnimTimeSpeed, onCompleted: OnBookStateChanged, onPageTurnStart: OnPageTurnStart, onPageTurnEnd: OnPageTurnEnd);
		}
	}

	protected virtual void OnPageTurnStart(Page page, int pageNumberFront, int pageNumberBack, int pageNumberFirstVisible, int pageNumberLastVisible, Page.TurnDirectionEnum turnDirection)
	{
		//Debug.Log("OnPageTurnStart: front [" + pageNumberFront + "] back [" + pageNumberBack + "] fv [" + pageNumberFirstVisible + "] lv [" + pageNumberLastVisible + "] dir [" + turnDirection + "]");
		// turn on the front and back page views of the page if necessary
		TogglePageView(pageNumberFront, true);
		TogglePageView(pageNumberBack, true);

		switch (turnDirection)
		{
			case Page.TurnDirectionEnum.TurnForward:
				// turn on the last visible page view if necessary
				TogglePageView(pageNumberLastVisible, true);
				break;
			case Page.TurnDirectionEnum.TurnBackward:
				// turn on the first visible page view if necessary
				TogglePageView(pageNumberFirstVisible, true);
				break;
		}
	}

	protected virtual void OnPageTurnEnd(Page page, int pageNumberFront, int pageNumberBack, int pageNumberFirstVisible, int pageNumberLastVisible, Page.TurnDirectionEnum turnDirection)
	{
		//Debug.Log("OnPageTurnEnd: front [" + pageNumberFront + "] back [" + pageNumberBack + "] fv [" + pageNumberFirstVisible + "] lv [" + pageNumberLastVisible + "] dir [" + turnDirection + "]");
		switch (turnDirection)
		{
			case Page.TurnDirectionEnum.TurnForward:
				// turn off the two pages that are now hidden by this page
				TogglePageView(pageNumberFirstVisible - 1, false);
				TogglePageView(pageNumberFirstVisible - 2, false);
				break;

			case Page.TurnDirectionEnum.TurnBackward:
				// turn off the two pages that are now hidden by this page
				TogglePageView(pageNumberLastVisible + 1, false);
				TogglePageView(pageNumberLastVisible + 2, false);
				break;
		}
	}
	#endregion

	#region 정연 scripts
	void OnBookPage_Fist()
	{
		TurnOffAllPageViews();

		TogglePageView(1, true);
		TogglePageView(2, true);
	}

	void OnBookPage_Last()
	{
		TurnOffAllPageViews();

		TogglePageView(book.CurrentLeftPageNumber, true);
		TogglePageView(book.CurrentRightPageNumber, true);
	}

	public void OnClick_BookLastPageOpen()
	{
		TurnOffAllPageViews();

		string debugMsg = "---------- OnClick BookLast PageOpen ----------\n";

		debugMsg += "Current Page Number [" + book.CurrentPageNumber + "] \n";
		debugMsg += "Current Left Page Number [" + book.CurrentLeftPageNumber + "] \n";
		debugMsg += "Current Right Page Number [" + book.CurrentRightPageNumber + "] \n";
		debugMsg += "Is First Page Group [" + book.IsFirstPageGroup + "] \n";
		debugMsg += "Is Last Page Group [" + book.IsLastPageGroup + "] \n";
		debugMsg += "Is Last Page Group [" + book.IsLastPageGroup + "] \n";
		debugMsg += "Last Page Number [" + book.LastPageNumber + "] \n";
		debugMsg += "Max Pages Turning Count [" + book.MaxPagesTurningCount + "] \n";
		debugMsg += "Is Turning Pages [" + book.IsTurningPages + "] \n";
		debugMsg += "Is Dragging Page [" + book.IsDraggingPage + "] \n";

		//Debug.Log(debugMsg);

		TogglePageView(book.CurrentLeftPageNumber, true);
		TogglePageView(book.CurrentRightPageNumber, true);
	}

	void MouseDownPageLoade()
	{
		TogglePageView(TempLeftPage, true);
		TogglePageView(TempRightPage, true);
	}

	void MouseUpPageClose()
	{
		TogglePageView(TempLeftPage, false);
		TogglePageView(TempRightPage, false);
	}

    void OnBookPage_Open(int _left, int _right)
	{
		if (OpenLeftPageNumber != _left && OpenRightPageNumber != _right)
		{
			if(OpenLeftPageNumber >= 0)
				TogglePageView(OpenLeftPageNumber, false);
			if (OpenRightPageNumber >= 0)
				TogglePageView(OpenRightPageNumber, false);

			OpenLeftPageNumber = _left;
			OpenRightPageNumber = _right;
			
			TogglePageView(OpenLeftPageNumber, true);
			TogglePageView(OpenRightPageNumber, true);
		}
	}
	public void OnClick_Left_Random_23Page()
	{
		int ran = Random.Range(4, 8);
		int sum;
		sum = book.CurrentPageNumber - ran;
		if (sum < 1)
		{
			sum = 1;
		}
		OnPageButtonClicked(sum);
	}

	public void OnClick_Right_Random_23Page()
	{
		int ran = Random.Range(4, 8);
		int sum;
		sum = book.CurrentPageNumber + ran;
		if (sum >= book.LastPageNumber)
		{
			sum = book.LastPageNumber - 1;
		}
		OnPageButtonClicked(sum);
	}

	public void OnClick_ImagePopupOpen()
    {
		imgPopup.OnClick_OpenPopup(book.CurrentLeftPageNumber, book.CurrentRightPageNumber);
	}

	public void OnClick_ImagePopupClose()
	{
		OnPageButtonClicked(imgPopup.NowPageIdx + 1);
		imgPopup.OnClick_ClosePopup();
	}

	public PaegView_CustomJY CustomScript(int num)
    {
		var temp = GetPageView(num);
		if(temp == null)
			return null;

		if (temp.TryGetComponent(out PaegView_CustomJY _custom))
		{
			return _custom;
		}
		return null;
    }

	public void OnClick_ChangeArScene(string sceneName = "Nft_Ar")
    {
		GameInfo.ins.BackSceneCheck = false;
		GameInfo.ins.ScnenName = SceneManager.GetActiveScene().name;

		LoaderUtility.Initialize();
		SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}

	void InitCamera()
	{
		if(sceneCamera.TryGetComponent(out Animator _anim)){
			introAni = _anim;
			if (GameInfo.ins.CameraState == 0)
				GameInfo.ins.CameraState = 2;
			else
			{
				GameInfo.ins.CameraState = 98;
				introAni.enabled = false;
			}
			return;
		}
		if(introAni == null && IntroCamera == null)
		{
			GameInfo.ins.CameraState = 98;
			return;
        }
		cameraIntro = IntroCamera.GetComponent<Camera>();
		introAni = IntroCamera.GetComponent<Animator>();

		cameraIntro.targetDisplay = 0;
		sceneCamera.targetDisplay = 0;

		cameraIntro.gameObject.SetActive(false);
		sceneCamera.gameObject.SetActive(false);
		GameInfo.ins.CameraState = 1;
	}

	void CameraStateCheck()
	{
		switch (GameInfo.ins.CameraState)
		{
			case 1:
				cameraIntro.gameObject.SetActive(true);
				sceneCamera.gameObject.SetActive(false);
				introAni.speed = IntroAniSpeed;
				introAni.Play(0);
				GameInfo.ins.CameraState = 10;
				GameInfo.ins.OldCameraState = 1;
				break;
			case 2:
				introAni.enabled = true;
				introAni.speed = IntroAniSpeed;
				introAni.Play(0);
				GameInfo.ins.CameraState = 10;
				GameInfo.ins.OldCameraState = 2;
				break;

			case 10:
				//introAni Play 중일때
				AnimatorStateInfo stateinfo = introAni.GetCurrentAnimatorStateInfo(0);

				if (stateinfo.normalizedTime > 0.98f)
				{
					switch (GameInfo.ins.OldCameraState)
					{
						case 1: GameInfo.ins.CameraState = 99; break;
						case 2: GameInfo.ins.CameraState = 98; break;
					}
				}
				break;

			case 98:
				if(introAni != null)
					introAni.enabled = false;
				GameInfo.ins.CameraState = 100;
				break;
			case 99:
				sceneCamera.gameObject.SetActive(true);
				cameraIntro.gameObject.SetActive(false);
				GameInfo.ins.CameraState = 100;
				break;
		}
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
			GameInfo.ins.CameraState = 0;
		}

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    GameInfo.ins.CameraState = 98;
        //}
		CameraStateCheck();
	}

	public void SoundPlay(AudioClip SoundSrc, float _volume = 1, bool Sound3D = false, bool Loop = false)
	{
		if (SoundSrc == null)
			return;

		if (SoundObject != null)
			Destroy(SoundObject);

		SoundObject = new GameObject("sound");
		DontDestroyOnLoad(SoundObject);

		SoundObject.transform.position = Vector3.zero;
		SoundObject.transform.SetParent(transform);
		AudioSource _audioSource = SoundObject.AddComponent<AudioSource>();
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
			Destroy(SoundObject, SoundSrc.length);
	}
	#endregion
}
