﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControls : MonoBehaviour {

    public Button pauseResume;
    public Sprite pauseSprite, resumeSprite;
    private bool isPaused = false;

    public bool IsPaused
    {
        get
        {
            return isPaused;
        }

        set
        {
            isPaused = value;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPauseResumeClick()
    {
        SetPaused(!isPaused);
    }

    private void SetPaused(bool isPaused_)
    {
        isPaused = isPaused_;
        if (isPaused)
        {
            pauseResume.image.sprite = resumeSprite;
        }
        else
        {
            pauseResume.image.sprite = pauseSprite;
            // resume movescripts
            Resume();
        }
    }

    private void Resume()
    {
		MapScript[] mapScripts = FindObjectsOfType<MapScript>();
		foreach (MapScript map in mapScripts)
		{
			if (map == null) continue;

			map.ResumeVisualization();
		}
	}

    public void Begin()
    {
		if (!isPaused) return;

		MapScript[] mapScripts = FindObjectsOfType<MapScript>();
		foreach (MapScript map in mapScripts)
		{
			if (map == null) continue;

			map.StepBegin();
		}
	}

    public void End()
    {
		if (!isPaused) return;

		MapScript[] mapScripts = FindObjectsOfType<MapScript>();
		foreach (MapScript map in mapScripts)
		{
			if (map == null) continue;

			map.StepEnd();
		}
	}

    public void Next()
    {
        if (!isPaused) return;

		MapScript[] mapScripts = FindObjectsOfType<MapScript>();
		foreach(MapScript map in mapScripts)
		{
			if (map == null) continue;

			map.StepForward();
		}
    }

    public void Previous()
    {
		if (!isPaused) return;

		MapScript[] mapScripts = FindObjectsOfType<MapScript>();
		foreach (MapScript map in mapScripts)
		{
			if (map == null) continue;

			map.StepBackwards();
		}
	}
}
