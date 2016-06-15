﻿using UnityEngine;
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
    
    public int columns = 4;

    private List<LevelData> _levels;

    public AudioClip tapSFX;
    private AudioSource _audio;

    private List<Page> _pages;
    private bool _elementsAdded;
    // Use this for initialization
    public static LevelControl instance;

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

        _elementsAdded = false;
        _levels = GameModel.Instance.levelsData.levels;
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
        if(Screen.width <= 0f)
        {
            return;
        }

        RectTransform myRect = _pages[0].GetComponent<RectTransform>();

        if (!_elementsAdded && myRect.rect.width > 0)
        {
            //Debug.Log("Screen.width " + Screen.width);
            _elementsAdded = true;
            var scale = Screen.width / 800f;
            //Debug.Log("myRect " + myRect.rect.height + " : " + myRect.rect.width + " scale: " + scale);
            var buttonWidth = (myRect.rect.width - 32 * scale) / (float)columns; 

            int i = 0;
            foreach (LevelData level in _levels)
            {
                var index = (int)Mathf.Floor(i / 16);
                if (i % 16 == 0)
                {
                    GridLayoutGroup grid = _pages[index].GetComponent<GridLayoutGroup>();
                    grid.cellSize = new Vector2(buttonWidth, buttonWidth);
                    grid.spacing = new Vector2(8 * scale, 8 * scale);
                    Pages.UpdatePagination();
                }

                var button = (Button)Instantiate(levelButton);
                button.transform.SetParent(_pages[index].transform, false);
                button.GetComponent<LevelButton>().level = level;

                i++;
            }
        }
        
    }

    public void GoBack()
    {
        _audio.PlayOneShot(tapSFX);
        SceneManager.LoadScene(backScene);
    }

    internal void StartGame(LevelData level)
    {
        _audio.PlayOneShot(tapSFX);
        GameModel.Instance.currentLevel = level;
        SceneManager.LoadScene(gameScene);
    }
}
