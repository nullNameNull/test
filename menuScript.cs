using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Monetization;

public class menuScript : MonoBehaviour
{
    public GameObject  levelsPanel,
                        playPanel,
                        shopPanel,
                        settingsPanel,
                        hightPanel,
                        loadingPanel,
                        exitPanel;
    
    private bool    _right=false;
                        
    public  Image   loadingImg;
    public  Text    loadingText;
    private int     sceneId;
    public Scrollbar skb;

    void Start()
    {
        skb.value=PlayerPrefs.GetFloat("Sound");
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(levelsPanel.activeSelf)
            {
                playPanel.SetActive(true);
                levelsPanel.SetActive(false);
            }else if(shopPanel.activeSelf)
            {
                playPanel.SetActive(true);
                shopPanel.SetActive(false);
            }else if(exitPanel.activeSelf)
            {
                exitPanel.SetActive(false);
            }else if(settingsPanel.activeSelf)
            {
                playPanel.SetActive(true);
                settingsPanel.SetActive(false);
                
                PlayerPrefs.SetFloat("Sound",skb.value);
            }else if(playPanel.activeSelf)
            {
                exitPanel.SetActive(true);
            }
        }

        

        if(sceneId>0)
        {
            StartCoroutine(AsyncLoad());
            sceneId=0;
        }

        if(skb.value>0)
        {
            gameObject.GetComponent<AudioSource>().volume=skb.value;
            if (!gameObject.GetComponent<AudioSource>().enabled)
            {
                gameObject.GetComponent<AudioSource>().enabled=true;
            }
        }
        else if (gameObject.GetComponent<AudioSource>().enabled)
        {
            gameObject.GetComponent<AudioSource>().enabled=false;
        }
        

    }

    public void OnClickPlay()
    {
        playPanel.SetActive(false);
        levelsPanel.SetActive(true);
    }

    public void OnClickExit()
    {
        exitPanel.SetActive(true);
    }

    public void ClickExit(){
        Application.Quit();
    }

    public void ClickCancel(){
        exitPanel.SetActive(false);
    }

    public void OnClickSettings()
    {
        settingsPanel.SetActive(true);
        playPanel.SetActive(false);
    }

    public void OnClickOkSettings(){
        settingsPanel.SetActive(false);
        playPanel.SetActive(true);

        PlayerPrefs.SetFloat("Sound",skb.value);
    }

    public void OnClickShop()
    {
        shopPanel.SetActive(true);
        playPanel.SetActive(false);
    }

    public void OnClickLevel(int levels)
    {
        sceneId=levels;
        hightPanel.SetActive(false);
        loadingPanel.SetActive(true);
    }

    IEnumerator AsyncLoad()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        while(!operation.isDone)
        {
            if(_right && loadingImg.fillAmount<1f)
            {
                loadingImg.fillAmount += Time.deltaTime;
            }else if(!_right && loadingImg.fillAmount>0f)
            {
                loadingImg.fillAmount -= Time.deltaTime;
            }else
            {
                _right=!_right;
                loadingImg.fillClockwise=_right;
            }

            loadingText.text=string.Format("{0:0}%",operation.progress*100);
            yield return null;
        }
    }
}
