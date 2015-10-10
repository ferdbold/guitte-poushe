﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Message;
using Tendresse.Data;

public class GameManager : MonoBehaviour {

    static public GameManager instance;
    public bool isFirst; //TODO :The server chooses a first and second player in the date. THIS DOES NOT CHANGE DURING THE DATE !

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
    }



//-----------------------------------------------------------------------------------------------------------------------
//////////////////////////////////////////////// SWITCH SCENE ///////////////////////////////////////////////////////////

    //VARIABLES
    Scenes currentScene;

    public enum Scenes {
        Menu,
        LoadingGame,
        Main
    }

    //FUNCTIONS

    /// <summary>
    /// Switch between scenes
    /// </summary>
    /// <param name="scene"></param>
    public void SwitchScene(Scenes scene) {
        OnSceneEnd(currentScene);
        currentScene = scene;
        OnSceneStartup(currentScene);
    }

    /// <summary>
    /// Event when the scene end
    /// </summary>
    /// <param name="scene"></param>
    void OnSceneEnd(Scenes scene) {
        switch (scene) {
            case Scenes.Menu:
                GameObject.FindWithTag("MainMenuRef").GetComponent<MainMenuRefUI>().mainMenu.DOFade(0, 0.75f);
                break;
            case Scenes.LoadingGame:
                CanvasGroup loading = GameObject.FindWithTag("MainMenuRef").GetComponent<MainMenuRefUI>().loading;
                loading.DOFade(0, 0.75f);
                loading.GetComponent<LoadingUI>().StopAllCoroutines();
                break;
            case Scenes.Main:
                break;
        }
    }

    /// <summary>
    /// Event on scene startup
    /// </summary>
    /// <param name="scene"></param>
    void OnSceneStartup(Scenes scene) {
        switch (scene) {
            case Scenes.Menu:
                Application.LoadLevel("Menu");
                break;
            case Scenes.LoadingGame:
                CanvasGroup loading = GameObject.FindWithTag("MainMenuRef").GetComponent<MainMenuRefUI>().loading;
                loading.DOFade(1, 0.75f);
                loading.GetComponent<LoadingUI>().StartAnim();
                message messa = new message("queueMatch");
                NetManager.instance.SendMessage(messa);
                break;
            case Scenes.Main:
                Application.LoadLevel("Main");
                break;
        }
    }

//-----------------------------------------------------------------------------------------------------------------------
////////////////////////////////////////////////              ///////////////////////////////////////////////////////////

    /// <summary>
    /// Event when the player finds a partner online
    /// </summary>
    public void Event_OnFindPartner(bool isFirst) {
        SwitchScene(Scenes.Main);
    }


   

    public void Event_OnSendImage(TendresseData tData) {
        Debug.Log("Beginning Send Message");
        message mes = NetManager.instance.MakeMessageFromImage(tData);
        Debug.Log("Created Message" + conversionTools.convertMessageToString(mes));
        NetManager.instance.SendMessage(mes);
        Debug.Log("Sent Message");
    }

    public void Event_OnReceiveImage(TendresseData tData) {
        Debug.Log("draw 1");
        DateManager.instance.DrawImageAt(tData, Vector3.zero, 1f);
        Debug.Log("draw 2");
    }

    

}
