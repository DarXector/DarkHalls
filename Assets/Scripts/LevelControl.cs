using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UI.Pagination;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class LevelControl : MonoBehaviour {

    public PagedRect Pages = null;
    public Button levelButton;
    public String gameScene;
    
    public int columns = 4;

    private List<LevelData> _levels;

    internal void StartGame(LevelData level)
    {
        GameModel.Instance.currentLevel = level;
        SceneManager.LoadScene(gameScene);
    }

    private List<Page> _pages;
    private bool _elementsAdded;
    // Use this for initialization
    public static LevelControl instance;

    void Awake()
    {
        // setup reference to game manager
        if (instance == null)
            instance = this.GetComponent<LevelControl>();

    }

    void Start ()
    {
        _elementsAdded = false;
        _levels = GameModel.Instance.levels.levels;
        _pages = new List<Page>();

        int i = 0;

        foreach (LevelData level in _levels)
        {
            if (i % 16 == 0)
            {
                var page = Pages.AddPageUsingTemplate();
                page.gameObject.SetActive(true);
                page.PageTitle = "Page_" + Mathf.Round(i / 16);
                _pages.Add(page);
            }

            i++;
        }
    }

    void Update()
    {
        RectTransform myRect = _pages[0].GetComponent<RectTransform>();

        if (!_elementsAdded && myRect.rect.width > 0)
        {
            _elementsAdded = true;
            Debug.Log("myRect " + myRect.rect.height + " : " + myRect.rect.width);
            var buttonWidth = (myRect.rect.width - 30) / (float)columns;            //Change

            int i = 0;
            foreach (LevelData level in _levels)
            {
                var index = (int)Mathf.Floor(i / 16);
                if (i % 16 == 0)
                {
                    GridLayoutGroup grid = _pages[index].GetComponent<GridLayoutGroup>();
                    grid.cellSize = new Vector2(buttonWidth, buttonWidth);
                    Pages.UpdatePagination();
                }

                var button = (Button)Instantiate(levelButton);
                button.transform.SetParent(_pages[index].transform, false);
                button.GetComponent<LevelButton>().level = level;

                i++;
            }
        }
        
    }
}
