﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Message;
using Tendresse.Data;

public class GameManager : MonoBehaviour {

    static public GameManager instance;

    public GameObject DrawingObjectPrefab;

    private TouchDraw mainTouchDraw;
    private List<TouchDraw> tempDrawingList = new List<TouchDraw>();

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
        if (instance = this) {
            instance = null;
        }
    }

    void Update() {
        DebugMethods();
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
                mainTouchDraw = GameObject.FindGameObjectWithTag("MainDrawObject").GetComponent<TouchDraw>();
                break;
        }
    }

//-----------------------------------------------------------------------------------------------------------------------
////////////////////////////////////////////////              ///////////////////////////////////////////////////////////

    /// <summary>
    /// Event when the player finds a partner online
    /// </summary>
    public void Event_OnFindPartner() {
        SwitchScene(Scenes.Main);
    }


    /////////////////////////// MAKE DRAWINGS ////////////////////////

    /// <summary>
    /// Draw a temporary drawing into a newly created gameobject
    /// </summary>
    /// <param name="tData"></param>
    /// <param name="imagePosition"></param>
    /// <param name="imageScale"></param>
    public void DrawTempImageAt(TendresseData tData, Vector3 imagePosition, float imageScale) {
        GameObject go = (GameObject)Instantiate(DrawingObjectPrefab, imagePosition, Quaternion.identity);
        TouchDraw touchDraw = go.GetComponent<TouchDraw>();
        touchDraw.LoadTendresseData(tData, imagePosition, imageScale);

        tempDrawingList.Add(touchDraw);
    }

    /// <summary>
    /// Delete all temporary drawings 
    /// </summary>
    public void DeleteTempImage() {
        for(int i=0; i< tempDrawingList.Count; i++) {
            Destroy(tempDrawingList[i].gameObject);
        }
        tempDrawingList = new List<TouchDraw>();
    }

    /// <summary>
    /// Draw into the main image Draw
    /// </summary>
    /// <param name="tData"></param>
    /// <param name="imagePosition"></param>
    /// <param name="imageScale"></param>
    public void DrawImageAt(TendresseData tData, Vector3 imagePosition, float imageScale) {
        if (mainTouchDraw != null) {
            mainTouchDraw.LoadTendresseData(tData, imagePosition, imageScale);
        }
    }

    public void Event_OnSendImage(TendresseData tData) {
        message mes = NetManager.instance.MakeMessageFromImage(tData);
        NetManager.instance.SendMessage(mes);
    }

    public void Event_OnReceiveImage(TendresseData tData) {
        DrawImageAt(tData, Vector3.zero, 1f);
    }

    private void DebugMethods() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            Event_OnSendImage(mainTouchDraw.SaveCurrentData());
        }
        if (Input.GetKeyDown(KeyCode.X)) {

        }
    }

}