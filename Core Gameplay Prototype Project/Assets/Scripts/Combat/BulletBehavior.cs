using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    Player,
    Enemy,
}

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] TargetType targetType;

    SpriteRenderer spriteRenderer;

    delegate void BulletDespawner(GameObject bullet);
    BulletDespawner despawn;

    bool isPlayerBullet { get { return targetType == TargetType.Enemy; } }
    bool partOfPool = true;
    bool punchThrough = false;
    int damage = 1;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(CombatStageManager.Instance != null){
            if(partOfPool){
                despawn = isPlayerBullet ? CombatStageManager.Instance.ReturnPlayerProjectile : CombatStageManager.Instance.ReturnEnemyProjectile;
            }
            else{
                despawn = Destroy;
            }
        }
        else{
            despawn = isPlayerBullet ? TutorialRoomManager.Instance.ReturnPlayerProjectile : TutorialRoomManager.Instance.ReturnEnemyProjectile;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // Note: by default, Translate translates the transform relative to itself, so Vector3.up
        // is like 1 unit up the y-axis relative to the transform (after rotation)
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (!spriteRenderer.isVisible)
        {
            despawn(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == targetType.ToString())
        {
            if (isPlayerBullet)
            {
                GameObject particles;
                if(CombatStageManager.Instance != null){
                    particles = CombatStageManager.Instance.GetEnemyHitParticles();
                }
                else{
                    particles = TutorialRoomManager.Instance.GetEnemyHitParticles();
                }
                particles.transform.position = transform.position;
                particles.GetComponent<ParticleSystem>().Emit(10);
            }

            IBulletHittable bulletHitDetector = collision.GetComponent<IBulletHittable>();
            if (bulletHitDetector != null)
            {
                bulletHitDetector.OnBulletHit(damage);
            }

            if(!punchThrough){
                despawn(gameObject);
            }
            
        }
    }

    // For use in cases where the projectile speed is different from the default
    public void SetSpeed(float val)
    {
        speed = val;
    }

    public void AllowPunchThrough(){
        punchThrough = true;
    }

    public void MarkNotPool(){
        partOfPool = false;
    }

    public void SetDamage(int val){
        damage = val;
    }
}
