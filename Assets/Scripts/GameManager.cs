using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public int playerFoodPoints = 100;
    public static GameManager instance = null;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private BoardManager boardScript;
    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        this.doingSetup = true;

        this.levelImage = GameObject.Find("LevelImage");
        this.levelText = GameObject.Find("LevelText").GetComponent<Text>();
        this.levelText.text = $"Day {this.level}";
        this.levelImage.SetActive(true);
        Invoke("HideLevelImage", this.levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        this.levelImage.SetActive(false);
        this.doingSetup = false;
    }

    //void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    //{
    //    this.level++;
    //    this.Awake();
    //}

    //void OnEnable()
    //{
    //    SceneManager.sceneLoaded += this.OnLevelFinishedLoading;
    //}

    //void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= this.OnLevelFinishedLoading;
    //}

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.level++;
        instance.Awake();
    }

    public void GameOver()
    {
        this.levelText.text = $"After {this.level} days, you starved.";
        this.levelImage.SetActive(true);
        enabled = false;
    }

    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup) return;

        StartCoroutine("MoveEnemies");
    }

    public void AddEnemyToList(Enemy script)
    {
        this.enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        this.enemiesMoving = true;
        yield return new WaitForSeconds(this.turnDelay);

        if (this.enemies.Count == 0)
        {
            yield return new WaitForSeconds(this.turnDelay);
        }

        foreach (Enemy enemy in this.enemies)
        {
            enemy.MoveEnemy();
            yield return new WaitForSeconds(enemy.moveTime);
        }

        this.playersTurn = true;
        this.enemiesMoving = false;
    }
}
