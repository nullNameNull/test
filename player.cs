using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Monetization;


public class player : MonoBehaviour
{
    public  GameObject  ball2,
                        ball,
                        plane,
                        _panel,
                        _StartPos;
    public  Text    message,
                    loadingText;
            
    private bool    startBol=false,
                    _gravity,
                    _click=false,
                    _right=false,
                    _exit=false;
    private double  lenghtB,
                    lenghtC,
                    cosY;
    private float   d,
                    _sound;
            
    [SerializeField]
    private int _raz,
                _raz2,
                money;


    public GameObject   panelPause,
                        panelExit,
                        panelSettings,
                        headPanel,
                        loadPanel,
                        moneyText;
    public Image        loadingImg;
    public Scrollbar skb;
    


    void Start(){
        //отключаем физику мяча
        Instantiate(ball2,new Vector3(_StartPos.transform.position.x,_StartPos.transform.position.y,0),Quaternion.identity);
        ball=GameObject.Find("ball(Clone)");
        plane=GameObject.Find("ball(Clone)/Square");
        _raz2=_raz;
        gameObject.GetComponent<Camera>().backgroundColor=new Color(0.6f, 0.6f, 0.6f, 1);

        _sound=PlayerPrefs.GetFloat("Sound");

        PlayerPrefs.SetInt("Money", 100);

        money=PlayerPrefs.GetInt("Money");
        moneyText.GetComponent<Text>().text=money.ToString();
    }
    
    void Update()
    {
        
        if(_exit)
        {
            StartCoroutine(AsyncLoad());
            _exit=false;
        }


        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(panelPause.activeSelf)
            {
               headPanel.SetActive(true);
               panelPause.SetActive(false); 
               Time.timeScale=1;
            }else if(panelExit.activeSelf)
            {
               panelExit.SetActive(false);
               panelPause.SetActive(true);
            }else if(panelSettings.activeSelf)
            {
                panelSettings.SetActive(false);
                panelPause.SetActive(true);

                PlayerPrefs.SetFloat("Sound", skb.value);
            }else
            {
                OnClickPause();
            }
        }

        _Camera();//движение камеры

        if(_raz>0 && headPanel.activeSelf){       
            //запускаем кручение мяча
            if(Input.GetMouseButtonDown(0)){
                //кликаем рядом с мячем или на мячь
                d=(float)Math.Sqrt((Camera.main.ScreenToWorldPoint(Input.mousePosition).x-ball.transform.position.x)*(Camera.main.ScreenToWorldPoint(Input.mousePosition).x-ball.transform.position.x)
                    +(Camera.main.ScreenToWorldPoint(Input.mousePosition).y-ball.transform.position.y)*(Camera.main.ScreenToWorldPoint(Input.mousePosition).y-ball.transform.position.y));
                //если попадаем то...
                if(d<ball.transform.localScale.x * 10){
                    _click=true;
                    startBol=true;
                    plane.GetComponent<SpriteRenderer>().enabled=true;
                }      
            }else if(Input.GetMouseButtonUp(0) && _click){
                //удаление линии направления
                plane.GetComponent<SpriteRenderer>().enabled=false;

                //удаление сообщения
                if(_panel){
                    _panel.SetActive(false);
                }

                //выключаем кручение мяча
                startBol=false;

                
                //включаем физику и бросаем мячик
                ball.GetComponent<Rigidbody2D>().simulated=true;
                ball.GetComponent<Rigidbody2D>().AddForce(Vector2.up * (Camera.main.ScreenToWorldPoint(Input.mousePosition).y-ball.transform.position.y)*-500);
                ball.GetComponent<Rigidbody2D>().AddForce(Vector2.left * (Camera.main.ScreenToWorldPoint(Input.mousePosition).x-ball.transform.position.x)*500);
                _raz-=1; 
                _click=false;
            }
            
            //поворот мяча
            if(startBol && _click){
                _rotate();
            }
        }

        
    }

    //уничтожение мяча
    public void Death(){
        ball.name="1";
        Destroy(ball);
        Instantiate(ball2,new Vector3(_StartPos.transform.position.x,_StartPos.transform.position.y,0),Quaternion.identity);
        plane=GameObject.Find("ball(Clone)/Square");
        ball=GameObject.Find("ball(Clone)");
        _raz=_raz2;
    }

    void _Camera(){
        //движение камеры
        if (transform.position.x< ball.transform.position.x) {
                transform.Translate(Vector2.right * (ball.transform.position.x - transform.position.x)*2.5f * Time.deltaTime);
        }else if (transform.position.x > ball.transform.position.x)
        {
            transform.Translate(Vector2.right * (ball.transform.position.x - transform.position.x)*2.5f * Time.deltaTime);
        }
        if (transform.position.y < ball.transform.position.y)
        {
            transform.Translate(Vector2.up * (ball.transform.position.y - transform.position.y)*2.5f*Time.deltaTime);
        }
        else if (transform.position.y > ball.transform.position.y)
        {
            transform.Translate(Vector2.up * (ball.transform.position.y - transform.position.y)*2.5f * Time.deltaTime);
        }
    }

    private void _rotate(){
        //ИЗВРАЩЕННАЯ ФОРМУЛА ИЗ МОЕЙ БОШКИ
        //чертим в уме треугольник. Сторона А статично = 100
        //сторону B и C мы ищем по формулам
        lenghtB=(Camera.main.ScreenToWorldPoint(Input.mousePosition).x-ball.transform.position.x)*(Camera.main.ScreenToWorldPoint(Input.mousePosition).x-ball.transform.position.x)
            +(Camera.main.ScreenToWorldPoint(Input.mousePosition).y-ball.transform.position.y)*(Camera.main.ScreenToWorldPoint(Input.mousePosition).y-ball.transform.position.y);
        lenghtB=Math.Sqrt(lenghtB);//находим длину стороны B

        lenghtC=(Camera.main.ScreenToWorldPoint(Input.mousePosition).x-ball.transform.position.x)*(Camera.main.ScreenToWorldPoint(Input.mousePosition).x-ball.transform.position.x)
            +(Camera.main.ScreenToWorldPoint(Input.mousePosition).y-ball.transform.position.y+100)*(Camera.main.ScreenToWorldPoint(Input.mousePosition).y+100-ball.transform.position.y);
        lenghtC=Math.Sqrt(lenghtC);//находим длину стороны C

        //находим угол треугольника между мячем и указателем
        cosY=Math.Acos((lenghtB*lenghtB+lenghtC*lenghtC-100*100)/(2*lenghtC*lenghtB))/Math.PI*180;

        //крутим мячик противоположно курсору
        if(Camera.main.ScreenToWorldPoint(Input.mousePosition).x>ball.transform.position.x){
            ball.transform.rotation = Quaternion.Euler(0,0,180-Convert.ToSingle(cosY));
        }else{
            ball.transform.rotation = Quaternion.Euler(0,0,Convert.ToSingle(cosY)-180);
        }
    }

    public void OnClickPause()
    {
        headPanel.SetActive(false);
        panelPause.SetActive(true);
        Time.timeScale=0;//ставим игру на паузу
    }

    public void OnClickPauseResume()
    {
        headPanel.SetActive(true);
        panelPause.SetActive(false);
        Time.timeScale=1;
    }

    public void OnClickExit()
    {
        panelExit.SetActive(true);
        panelPause.SetActive(false);
    }

    public void OnClickExitOk()
    {
        Time.timeScale=1;
        _exit=true;
        loadPanel.SetActive(true);
    }

    public void OnClickExitCancel()
    {
        panelExit.SetActive(false);
        panelPause.SetActive(true);
    }

    public void OnClickSettings()
    {
        panelSettings.SetActive(true);
        panelPause.SetActive(false);

        skb.value=PlayerPrefs.GetFloat("Sound");
    }

    public void OnClickSettingsOk()
    {
        panelSettings.SetActive(false);
        panelPause.SetActive(true);

        PlayerPrefs.SetFloat("Sound", skb.value);
    }

    //загрузочный экран
    IEnumerator AsyncLoad()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(0);
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

