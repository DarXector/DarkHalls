using UnityEngine;
using UnityEngine.UI;
using UI.Pagination;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class LevelControl : MonoBehaviour {

    public PagedRect Pages = null;
    public Button levelButton;
    public String gameScene;
    public String backScene;
    public String instructionsScene;

    public RectTransform levelList;
    public RectTransform backButton;
    public GameObject loadingText;

    public int columns = 4;
    public int itemsPerPage = 12;

    private List<LevelData> _levels;

    public AudioClip tapSFX;
    private AudioSource _audio;

    private List<Page> _pages;
    private bool _elementsAdded;
    // Use this for initialization
    public static LevelControl instance;

    private string _nextScene;
    private Page _currentPage;

    void Awake()
    {
        // setup reference to game manager
        if (instance == null)
            instance = this.GetComponent<LevelControl>();

        _audio = GetComponent<AudioSource>();

    }

    void Start ()
    {
        GameModel.Instance.navigated = true;

        LeanTween.moveY(levelList, -1280f, 0f);
        LeanTween.moveX(backButton, -600f, 0f);

        _elementsAdded = false;
        _levels = GameModel.Instance.gameData.levels;
        _pages = new List<Page>();

        int i = 0;

        foreach (LevelData level in _levels)
        {
            if (i % itemsPerPage == 0)
            {
                var page = Pages.AddPageUsingTemplate();
                page.gameObject.SetActive(true);
                page.PageTitle = "Page_" + Mathf.Round(i / itemsPerPage);
                _pages.Add(page);
            }

            i++;
        }

        LeanTween.moveY(levelList, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.2f);
        LeanTween.moveX(backButton, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.6f).onComplete += OnIntroComplete;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBack();
        }

        if (Screen.width <= 0f)
        {
            return;
        }

        RectTransform myRect = _pages[0].GetComponent<RectTransform>();

        if (!_elementsAdded && myRect.rect.width > 0)
        {
            //Debug.Log("Screen.width " + Screen.width);
            _elementsAdded = true;
            var scale = Screen.width / 800f;
            Debug.Log("myRect " + myRect.rect.height + " : " + myRect.rect.width + " scale: " + scale);
            var buttonWidth = (myRect.rect.width - 32 * scale) / (float)columns; 

            for(var i = 0; i < _pages.Count; i++)
            {
                GridLayoutGroup grid = _pages[i].GetComponent<GridLayoutGroup>();
                grid.cellSize = new Vector2(buttonWidth, buttonWidth);
                grid.spacing = new Vector2(8 * scale, 8 * scale);
            }

            int j = 0;
            foreach (LevelData level in _levels)
            {
                //Debug.Log("level " + level.index);
                var index = (int)Mathf.Floor(j / itemsPerPage);
                if (j % itemsPerPage == 0)
                { 
                    Pages.UpdatePagination();
                }

                var button = (Button)Instantiate(levelButton);
                button.transform.SetParent(_pages[index].transform, false);
                button.GetComponent<LevelButton>().level = level;

                j++;
            }
        }

        
    }

    void ToggleLayoutControls(Page page, bool enabled)
    {
        GridLayoutGroup grid = page.GetComponent<GridLayoutGroup>();
        grid.enabled = enabled;
        //LayoutElement element = page.GetComponent<LayoutElement>();
        //element.enabled = enabled;
    }

    void OnIntroComplete()
    {
        Debug.Log("OnIntroComplete");
        for (var i = 0; i < _pages.Count; i++)
        {
            ToggleLayoutControls(_pages[i], false);
        }

        _currentPage = Pages.GetCurrentPage();
    }

    public void OnPageChange(Vector2 vec)
    {
        //ToggleLayoutControls(_currentPage, false);
        _currentPage = Pages.GetCurrentPage();
        //ToggleLayoutControls(_currentPage, true);

    }

    void ChangeScene()
    {
        if(_nextScene == gameScene)
        {
            loadingText.SetActive(true);
        }

        SceneManager.LoadScene(_nextScene);
    }

    void AnimateOutro()
    {
        _audio.PlayOneShot(tapSFX);
        LeanTween.moveY(levelList, -1280f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.moveX(backButton, -600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.2f).onComplete += ChangeScene;
    }

    public void GoBack()
    {
        _nextScene = backScene;
        AnimateOutro();
    }

    internal void StartGame(LevelData level)
    {
        GameModel.Instance.currentLevel = level;
        _nextScene = GameModel.Instance.gameData.seenInstructions? gameScene : instructionsScene;
        if(_nextScene == instructionsScene)
        {
            GameModel.Instance.instructionsBeforePlay = true;
        }
        AnimateOutro();
    }
}
