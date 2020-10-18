using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;

    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    private Animator animator;
    private Transform target;
    private bool skipMove;

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        this.animator = GetComponent<Animator>();
        this.target = GameObject.FindGameObjectWithTag("Player").transform;

        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (this.skipMove)
        {
            this.skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);
        this.skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        this.AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        hitPlayer.LoseFood(this.playerDamage);

        this.animator.SetTrigger("enemyAttack");
        SoundManager.instance.RandomizeSfx(this.enemyAttack1, this.enemyAttack2);
    }
}
