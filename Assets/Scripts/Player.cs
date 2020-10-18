using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public float restartLevelDelay = 1f;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public int wallDamage = 1;
    public Text foodText;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;
    private int food;

    protected override void Start()
    {
        this.animator = GetComponent<Animator>();
        this.food = GameManager.instance.playerFoodPoints;
        this.foodText.text = $"Food: {this.food}";

        base.Start();
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(this.wallDamage);
        this.animator.SetTrigger("playerChop");
    }

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = this.food;
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Exit":
                Invoke("Restart", this.restartLevelDelay);
                this.enabled = false;
                break;
            case "Food":
                this.food += pointsPerFood;
                this.foodText.text = $"+{this.pointsPerFood} Food: {this.food}";
                SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
                other.gameObject.SetActive(false);
                break;
            case "Soda":
                this.food += pointsPerSoda;
                this.foodText.text = $"+{this.pointsPerSoda} Food: {this.food}";
                SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
                other.gameObject.SetActive(false);
                break;
        }
    }

    public void LoseFood(int loss)
    {
        this.animator.SetTrigger("playerHit");
        this.food -= loss;
        this.foodText.text = $"-{loss} Food: {this.food}";
        this.CheckIfGameOver();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playersTurn) return;

        int horizontal = (int)Input.GetAxisRaw("Horizontal");
        int vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0) vertical = 0; // Disable diagonal movement
        if (horizontal != 0 || vertical != 0) this.AttemptMove<Wall>(horizontal, vertical);
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (this.Move(xDir, yDir, out RaycastHit2D hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
            this.food--;
            this.foodText.text = $"Food {this.food}";
            this.CheckIfGameOver();
        }
        else
        {
            T hitComponent = hit.transform?.GetComponent<T>();
            if (hitComponent != null) this.OnCantMove(hitComponent);
        }

        GameManager.instance.playersTurn = false;
    }

    private void CheckIfGameOver()
    {
        if (this.food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }
}
