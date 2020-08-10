﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string trackTest1;
    public FMOD.Studio.EventInstance track1;
    public TransformationPlayerStates myTfP;

    public static bool checkP1;
    public static bool checkP2;
    public float tempsAvantMusic;
    float tempsEcouleMusic;
    public bool isCheckMusic = false;

    [FMODUnity.EventRef]
    public string breathIntro;
    public FMOD.Studio.EventInstance bearthIntroEvent;

    public float volumeUp;
    public float volumeDown;
    // Start is called before the first frame update
    void Start()
    {

        isCheckMusic = false;
        checkP1 = false;
        checkP2 = false;
        tempsEcouleMusic = 0;
        track1 = FMODUnity.RuntimeManager.CreateInstance(trackTest1);
        bearthIntroEvent = FMODUnity.RuntimeManager.CreateInstance(breathIntro);
        bearthIntroEvent.start();
        track1.setParameterByName("TransiP1", 0F);
        track1.setParameterByName("TransiP2", 0F);
        track1.setVolume(volumeDown);
    }

    // Update is called once per frame
    void Update()
    {
        if (tempsEcouleMusic < tempsAvantMusic)
        {
            tempsEcouleMusic += Time.deltaTime;
        }
        if(StateOfGames.currentState == StateOfGames.StateOfGame.DefaultPlayable)
        {
            if (!isCheckMusic)
            {
                track1.start();
                isCheckMusic = true;
            }

        }
        if (checkP1)
        {
            track1.setParameterByName("TransiP1", 1F);
        }
        if (checkP2)
        {
            track1.setParameterByName("TransiP2", 1F);
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            track1.setVolume(volumeDown);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            track1.setVolume(volumeUp);
        }
    }
}
