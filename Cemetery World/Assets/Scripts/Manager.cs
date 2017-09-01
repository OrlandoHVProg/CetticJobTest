using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//struct para recoger la informacion de login del usuario
[System.Serializable]
public struct Error
{
	public string error;
}
[System.Serializable]
public struct User
{
	public int id;
	public string name;
	public string email;
	public string created_at;
	public string updated_at;
	public int score;
}

[System.Serializable]
public struct Parameters
{
	public int id;
	public int velocidad_personaje;
	public int tiempo;
	public int puntos_items;
	public int puntos_enemigos;
	public int cantidad_vida;
}

//Desde aqui se controlaran todas las variables y condiciones de la partida.Asi como la conexion a la web
public class Manager : MonoBehaviour {

	public int Lives=3;
	public float Speed=5;
	public int TimeInSeconds=30;
	public int ValorGemas=100;
	public int ValorKills=50;
	public string URL="192.168.100.13";
	public GameObject [] spawnPlaces;
	public GameObject [] monsters;
	public PlayerController player;
	int gemas=0;
	int kills=0;
	int score=0;
	public float gameTime = 30f;
	public GravityAttractor world;
	public Text time;
	public Text livesui;
	public Text scoreui;
	public Text gemsui;
	public Text gameover;
	public Text survive;
	public Text finalscore;
	public Text highscore;
	public Image resume;
	public Image restart;
	public Image close;
	bool gameoverc=false;
	public GameObject LoginUI;
	public GameObject gameUI;
	public GameObject registerUI;
	public InputField loginuser;
	public InputField loginpass;
	public InputField reguser;
	public InputField regpass;
	public InputField regpassconf;
	public InputField regemail;
	public Text log;
	public Text regLog;
	User myObject2;
	// Use this for initialization
	void Start () {
		if(PlayerPrefs.HasKey("id"))
		{
			myObject2.id=PlayerPrefs.GetInt("id");
			myObject2.score=PlayerPrefs.GetInt("score");
			myObject2.name=PlayerPrefs.GetString("name");
			GetParameter();
			Time.timeScale=1;
			StartCoroutine(InvokeMonsters());
			LoginUI.SetActive(false);
			gameUI.SetActive(true);
		}
		else
		Time.timeScale=0;
	}
	//realiza el pedido POST para el login en la base de datos 
	public void Login()
	{
		WWWForm form=new WWWForm();
		form.AddField("name",loginuser.text); 
		form.AddField("password",loginpass.text);
		WWW www=new WWW(URL+"/game-login",form);
		StartCoroutine(WaitforLogin(www));
	}
    //realiza el pedido POST para el registro en la base de datos
    public void Register()
	{
		if(regpass.text!=regpassconf.text)
		{
			regLog.text="Password confirmation does not match";
			regLog.color=Color.red;
		}
		else if(regpass.text.Length<6)
		{
			regLog.text="Password must be at least 6 characters";
			regLog.color=Color.red;
		}
		else
		{
			WWWForm form=new WWWForm();
			form.AddField("name",reguser.text);
			form.AddField("email",regemail.text);
			form.AddField("password",regpass.text);
			form.AddField("password_confirmation",regpassconf.text);
			WWW www=new WWW(URL+"/game-register",form);
			StartCoroutine(WaitforReg(www));
		}

	}
	//Aqui se espera el response del pedido para registro
	IEnumerator WaitforReg(WWW www)
	{
		yield return www;
		if(www.error==null)
		{
			Error myObject=JsonUtility.FromJson<Error>(www.text);
			if(myObject.error!=null)
			{
				regLog.text=myObject.error;
				regLog.color=Color.red;
			}
			else
			{
				regLog.text="Succesfull Registration please back and do login";
				regLog.color=Color.green;
			}

		}
		else
		{
			Debug.Log(www.error);
		}
	}
    //Aqui se espera el response del pedido para login
    IEnumerator WaitforLogin(WWW www)
	{
		yield return www;
		if(www.error==null)
		{
			Error myObject=JsonUtility.FromJson<Error>(www.text);
			if(myObject.error==null)
			{
				myObject2=JsonUtility.FromJson<User>(www.text);
				log.text="Correct Login!!!";
				log.color=Color.green;
				GetParameter();
				Time.timeScale=1;
				StartCoroutine(InvokeMonsters());
				LoginUI.SetActive(false);
				gameUI.SetActive(true);
			}
			else
			{
				log.text=myObject.error;
				log.color=Color.red;
				myObject.error=null;
			}
				
		}
		else
		{
			Debug.Log(www.error);
		}
	}
		
	IEnumerator WaitforParam(WWW www)
	{
		yield return www;

		if(www.error==null)
		{
			Debug.Log(www.text);
			Parameters myObject2=JsonUtility.FromJson<Parameters>(www.text);
			Lives=myObject2.cantidad_vida;
			Speed=myObject2.velocidad_personaje;
			player.moveSpeed=myObject2.velocidad_personaje;
			ValorGemas=myObject2.puntos_items;
			ValorKills=myObject2.puntos_enemigos;
			TimeInSeconds=myObject2.tiempo;
			gameTime=myObject2.tiempo;
			time.text = (Mathf.Round(gameTime)).ToString();
			livesui.text=Lives.ToString();
		}
		else
		{
			Debug.Log(www.error);
		}
	}

	IEnumerator WaitforScore(WWW www)
	{
		yield return www;

		if(www.error==null)
		{
			Debug.Log(www.text);
		}
		else
		{
			Debug.Log(www.error);
		}
	}

	public void UploadScore(int score)
	{
		if(myObject2.score<score || myObject2.score==null)
		{
			WWWForm form=new WWWForm();
			form.AddField("user_id",myObject2.id); 
			form.AddField("score",score);
			WWW www=new WWW(URL+"/save-score",form);
			highscore.text="new highscore!!!";
			highscore.color=Color.green;
			StartCoroutine(WaitforScore(www));
		}
		else
		{
			highscore.text="no new highscore";
			highscore.color=Color.red;
		}
	}

	void GetParameter()
	{
		WWW www=new WWW(URL+"/get-parametros");
		StartCoroutine(WaitforParam(www));
	}

	// Aqui se controla el momento de final de partida tras terminarse el tiempo
	void Update () {
		if(Lives>0)
		{
			gameTime -= Time.deltaTime;
			if(gameTime<6)
				time.color=Color.red;
			time.text = (Mathf.Round(gameTime)).ToString();
		}
		if(gameTime < 0 && !gameoverc)
		{
			gameoverc=true;
			Time.timeScale=0;
			survive.enabled=true;
			score+=Lives*100;
			finalscore.text="Final score:"+score;
			UploadScore(score);
			finalscore.enabled=true;
			highscore.enabled=true;
			restart.enabled=true;
			close.enabled=true;
		}
	}
    //Invocacion periodica de los enemigos en los puntos seleccionados del mundo
	IEnumerator InvokeMonsters()
	{
		int placeR=Random.Range(0,12);
		int monsterR=Random.Range(0,11);
		GameObject invMons=Instantiate(monsters[monsterR],spawnPlaces[placeR].transform.position,spawnPlaces[placeR].transform.rotation);
		invMons.GetComponent<GravityBody>().attractor=world;
		yield return new WaitForSeconds(1.5f);
		if(gameTime>0)
			StartCoroutine(InvokeMonsters());
	}
    //controlar la perdida de vidas del jugador y el estado de game over al perder todas estas
	public void LiveLost()
	{
		Lives--;
		if(Lives<=1)
			livesui.color=Color.red;
		livesui.text=Lives.ToString();;
		if(Lives==0)
		{
            StartCoroutine(WaitForDie());
			
		}
	}
    //dar tiempo a la animacion de cuando se muere antes de mostrar la ui de game over
    IEnumerator WaitForDie()
    {
        yield return new WaitForSeconds(2);
        Time.timeScale = 0;
        gameover.enabled = true;
        score += Lives * 100;
        finalscore.text = "Final score:" + score;
		UploadScore(score);
        finalscore.enabled = true;
		highscore.enabled=true;
        restart.enabled = true;
        close.enabled = true;
    }
    //recoleccion de gemas y su efecto en el score
	public void Collection()
	{
		gemas++;
		score+=ValorGemas;
		gemsui.text=gemas.ToString();
		scoreui.text=score.ToString();
	}
    //eliminacion de enemigo y su efecto en el score
	public void EnemyKill()
	{
		kills++;
		score+=ValorKills;
		scoreui.text=score.ToString();
	}
    //A continuacion botones de navegacion y pausa
	public void Pausa()
	{
		Time.timeScale=0;
		resume.enabled=true;
		restart.enabled=true;
		close.enabled=true;
	}

	public void Resume()
	{
		Time.timeScale=1;
		resume.enabled=false;
		restart.enabled=false;
		close.enabled=false;
	}

	public void Restart()
	{
		PlayerPrefs.SetInt("id",myObject2.id);
		PlayerPrefs.SetInt("score",myObject2.score);
		PlayerPrefs.SetString("name",myObject2.name);
		SceneManager.LoadScene(0);

	}

	public void Close()
	{
		PlayerPrefs.DeleteAll();
		Application.Quit();
	}

    public void GoToReg()
    {
        LoginUI.SetActive(false);
        registerUI.SetActive(true);
    }

    public void BackToLogin()
    {
        LoginUI.SetActive(true);
        registerUI.SetActive(false);
    }
}
