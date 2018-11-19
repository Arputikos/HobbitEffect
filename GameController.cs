using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameController _instance = null;
    public static GameController i { get { return _instance; } }//Instance

    public GameObject levelBoundsObject;
    public Bounds levelBounds;

    public GameObject ZombiePrefab;

    private float levelTimer;

    public Text levelTimerText;
    static int LEVEL_TIME = 180;//time in seconds for the level

    public GameObject player { private set; get; }

    public bool gameOver { private set; get; }
    public bool win { private set; get; }

    public GameObject winnerImage;

    bool positiveWorld;//which is now

    float spawnTimer;

    private Transform finish;

    public Image healthBar;
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        //DontDestroyOnLoad(this.gameObject);

        Init();
    }
    void Init()
    {
        positiveWorld = true;
        Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("POSITIVE");
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("NEGATIVE"));
        winnerImage.SetActive(false);

        finish = GameObject.FindGameObjectWithTag("finish").transform;

        levelTimerText.text = "";
        gameOver = false;
        win = false;
        player = GameObject.FindGameObjectWithTag("Player");
        //read level area
        levelBounds = levelBoundsObject.GetComponent<MeshRenderer>().bounds;
        levelBoundsObject.SetActive(false);

        healthBar.fillAmount = 1.0f;

        levelTimer = -4.0f;//time - prepare

        spawnTimer = 0;
        SpawnEnemies(10);
    }

    void Update()
    {
        if (gameOver || win)
        {
            levelTimer += Time.deltaTime;
            if(levelTimer >= 3.0f)
            {
                Time.timeScale = 1.0f;
                //reload level
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            return;
        }

        spawnTimer += Time.deltaTime;
        if(spawnTimer >= 30.0f)//co 30 sec spawnuje
        {
            SpawnEnemies(10);
            spawnTimer = 0.0f;
        }

        if (player.GetComponent<PlayerHealth>().isDead())
        {
            GameOver();
            levelTimerText.text = "GAME OVER";
            return;
        }


        UpdateTimer();
        //UpdateUI()

        if (levelTimer > 0)
        {
            levelTimer -= Time.deltaTime;
            if(levelTimer <= 0)
            {
                //check if won or not
                if ((player.transform.position - finish.position).magnitude <= 5.0f)
                    Win();
                else
                    GameOver();
                
            }
        } else if (levelTimer < 0)
        {
            levelTimer += Time.deltaTime;
            if (levelTimer > 0)
            {
                levelTimer = LEVEL_TIME;
            }
        }

        if ((player.transform.position - finish.position).magnitude <= 5.0f)//player reached finish
        {
            Win();
        }
    }

    void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(ZombiePrefab, AICore.GetRandomMapPosition(), Quaternion.identity, GameObject.FindGameObjectWithTag("ENEMY_CONTAINER").transform);
        }
    }
    public void UpdateTimer()
    {
        string min = Mathf.FloorToInt(levelTimer / 60).ToString("##");
        if (min.Length == 1) min = "0" + min;
        else if (min.Length == 0) min = "00";
        string sec = Mathf.FloorToInt(levelTimer % 60).ToString("##");
        if (sec.Length == 1) sec = "0" + sec;
        else if (sec.Length == 0) sec = "00";
        if (levelTimer >= 0)
            levelTimerText.text = min + ":" + sec;
        else if (levelTimer < -3)
            levelTimerText.text = "...GET READY...";
        else if (levelTimer < -2)
            levelTimerText.text = "3...";
        else if (levelTimer < -1)
            levelTimerText.text = "2...";
        else if (levelTimer < 0)
            levelTimerText.text = "1...";
    }
    public void UpdateUI()
    {
        healthBar.fillAmount = (float)player.GetComponent<PlayerHealth>()._currentHealth / (float)player.GetComponent<PlayerHealth>()._maxHealth;
    }
    void GameOver()
    {
        levelTimerText.text = "GAME OVER";
        gameOver = true;
        Time.timeScale = 0.15f;
        levelTimer = 0;
        win = false;
    }
    public void ChangeWorlds()
    {
        positiveWorld = !positiveWorld;
        if (positiveWorld)
        {
            Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("POSITIVE");
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("NEGATIVE"));
        }
        else
        {
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("POSITIVE"));
            Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("NEGATIVE");
        }
    }

    public void Win()
    {
        levelTimerText.text = "YOU WON !";
        winnerImage.SetActive(true);
        gameOver = false;
        Time.timeScale = 0.15f;
        levelTimer = 0;
        win = true;
    }

    // Turn on the bit using an OR operation:
    //private void Show()
    //{
    //    Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("SomeLayer");
    //}

    //// Turn off the bit using an AND operation with the complement of the shifted int:
    //private void Hide()
    //{
    //    Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("SomeLayer"));
    //}

    //// Toggle the bit using a XOR operation:
    //private void Toggle()
    //{
    //    Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("SomeLayer");
    //}
}
