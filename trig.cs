using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trig : MonoBehaviour

{

    player myScript;//основной скрипт
    public GameObject particleEffect;//эффект удара
    public GameObject _camera;//основная камера
    public GameObject _line;//направляющая полоса
    public AudioClip _sound;//звук удара мяча
    public AudioClip _sound2;//звук входа в антигравитационную зону
    public AudioClip _sound3;//звук выхода из антигравитационной зоны
    private int inTerrain=0;
    private float   _timer;

    void Start(){
        myScript=GameObject.Find("Main Camera").GetComponent<player>();
    }

    void Update(){
        //создана, чтобы не было частых воспроизведений звука
        //в гравитационных поляъ
        if(inTerrain<0){
            if(_timer>=0.3f){
                inTerrain=0;
                _timer=0;
            }else if(_timer<0.3f){
                _timer+=Time.deltaTime;
            }
        }
    }

    void OnParticleCollision(GameObject other)
    {
        //попадание в мертвые зоны
        if(other.tag=="Damage"){
           myScript.Death();
        }
    }

    void OnCollisionEnter2D(Collision2D col){
        if(col.gameObject.tag=="terrain"){
            if(inTerrain==0){
                //при большой скорости и ударе об землю появляется эффект
                if(gameObject.GetComponent<Rigidbody2D>().velocity.magnitude>=5){
                    Instantiate(particleEffect, new Vector3(transform.position.x,transform.position.y,0), Quaternion.identity);
                }

                //воспроизведение звука удара мяча
                gameObject.GetComponent<AudioSource>().volume=gameObject.GetComponent<Rigidbody2D>().velocity.magnitude/15*PlayerPrefs.GetFloat("Sound");
                gameObject.GetComponent<AudioSource>().pitch=1f;
                gameObject.GetComponent<AudioSource>().PlayOneShot(_sound);
            }
            inTerrain=1;
            _timer=0;
        }
    }

    void OnCollisionExit2D(Collision2D col){
        //создана, чтобы не было частых воспроизведений звука
        //в гравитационных поляъ
        if(col.gameObject.tag=="terrain" && inTerrain>0){
            inTerrain=-1;
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col.tag=="Gravity"){//гравитационное поле
            gameObject.GetComponent<Rigidbody2D>().gravityScale=0;//убираем притяжение
            _camera=GameObject.Find("Main Camera");
            _camera.GetComponent<Camera>().backgroundColor=new Color(0.2f, 0.2f, 0.2f, 1);//меняем задний фон
            _line=GameObject.Find("Square");
            _line.GetComponent<SpriteRenderer>().color= new Color(1,1,1,1);//меняем цвет направляющей полосы
            gameObject.GetComponent<AudioReverbFilter>().enabled=true;//включаем
            _camera.GetComponent<AudioReverbFilter>().enabled=true;   //фильтры
        }else if(col.tag=="antiGravity"){//антигравитационное поле
            gameObject.GetComponent<Rigidbody2D>().gravityScale=-1;//делаем минусовую гравитацию
            gameObject.GetComponent<AudioSource>().volume=1f*PlayerPrefs.GetFloat("Sound");//подкручиваем громкость
            gameObject.GetComponent<AudioSource>().pitch=2f;//прибавляем скорость воспроизведения
            gameObject.GetComponent<AudioSource>().PlayOneShot(_sound2);//включаем звук
        }else if(col.tag=="Finish"){//финишная точка
            //Time.timeScale=0;//ставим игру на паузу
        }else if(col.tag=="Damage"){//зона смерти
            myScript.Death();
        }
    }

    void OnTriggerExit2D(Collider2D col){
        if(col.tag=="Gravity"){//покидаем гравитационное поле
            gameObject.GetComponent<Rigidbody2D>().gravityScale=1;//возвращаем стандартную гравитацию
            _line=GameObject.Find("Square");
            _line.GetComponent<SpriteRenderer>().color= new Color(0,0,0,1);//меняем цвет направляющей полосы
            _camera=GameObject.Find("Main Camera");
            _camera.GetComponent<Camera>().backgroundColor=new Color(0.6f, 0.6f, 0.6f, 1);//меняем задний фон
            gameObject.GetComponent<AudioReverbFilter>().enabled=false;//отключаем
            _camera.GetComponent<AudioReverbFilter>().enabled=false;   //фильтры
        }else if(col.tag=="antiGravity"){//покидаем антигравитационное поле
            gameObject.GetComponent<Rigidbody2D>().gravityScale=1;//возвращаем стандартную гравитацию
            gameObject.GetComponent<AudioSource>().pitch=2f;//ускоряем воспроизведение звука
            gameObject.GetComponent<AudioSource>().PlayOneShot(_sound3);//воспроизводим звук
        }
    }
}
