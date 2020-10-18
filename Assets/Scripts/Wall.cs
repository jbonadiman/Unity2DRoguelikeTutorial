using UnityEngine;

public class Wall : MonoBehaviour
{
    public Sprite dmgSprite;
    public int hp = 4;

    public AudioClip chopSound1;
    public AudioClip chopSound2;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall(int loss)
    {
        SoundManager.instance.RandomizeSfx(this.chopSound1, this.chopSound2);
        this.spriteRenderer.sprite = this.dmgSprite;
        this.hp -= loss;

        if (this.hp <= 0) this.gameObject.SetActive(false);
    }
}
