using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeMandibleBehavior : MonoBehaviour, IBulletHittable
{
    int mandibleHP = 6;
    EnemyHealth centipedeHP;

    SpriteRenderer spriteRenderer;
    int mandibleState = 0;

    [SerializeField] Sprite[] sprites;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        centipedeHP = GetComponentInParent<EnemyHealth>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBulletHit()
    {
        mandibleHP -= mandibleState;
        if (mandibleHP <= 0)
        {
            centipedeHP.TakeDamage(1);
            Destroy(gameObject);
        }
    }

    public void SetState(int val)
    {
        mandibleState = val;
        spriteRenderer.sprite = sprites[mandibleState];
    }
}