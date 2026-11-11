using UnityEngine;
using Assets.Globals;
using Unity.VisualScripting;

public class PlayerAttackMelee : MonoBehaviour
{
    public int damage = 10;
    public float swingSpeed = 20;
    public float range = 5;
    public float swingAngle = 90;
    public bool doesStun = false;
    private float deltaTime;
    private float startRot;
    private bool isAttacking;
    private float AttackAngle;
    private PlayerController player;
    private VarTypes.Data playerData;
    private Functions funcs = new();
    private FunctionsInstance funcInst = new();
    void Awake()
    {
        //startPos = transform.position;
        player = funcInst.GetSiblingGameObject("Player").GetComponent<PlayerController>();
        transform.gameObject.SetActive(false);
        startRot = transform.rotation.eulerAngles.z;
        transform.localScale = transform.localScale * range;
        playerData = player.GetData();
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime = Time.deltaTime;
        player = funcInst.GetSiblingGameObject("Player").GetComponent<PlayerController>();
        playerData = player.GetData();
        UpdatePosition();
    }
    void UpdatePosition()
    {
        if (isAttacking)
        {
            if (Mathf.Abs(startRot - AttackAngle) > swingAngle)
            {
                Debug.Log($"Attack finished. Angle: {startRot - AttackAngle}");
                Destroy(transform.gameObject);
            }
            else
            {
                playerData = player.GetData();
                AttackAngle += swingSpeed * deltaTime;
                transform.position = new Vector3(Mathf.Sin(AttackAngle * Mathf.Deg2Rad) * 0.5f * range + playerData.position.x, -Mathf.Cos(AttackAngle * Mathf.Deg2Rad) * 0.5f * range + playerData.position.y, 0);
                transform.localRotation = Quaternion.Euler(0, 0, AttackAngle);
                Debug.Log($"Attacking. Angle: {startRot - AttackAngle}");
                Debug.Log($"{startRot} vs {AttackAngle}");
            }
        }
    }


    public void OnAttack(GameObject enemy, Vector3 start)
    {
        // if targeted enemy is not active, return
        if (!enemy.activeSelf)
        {
            funcInst.Kill(transform.gameObject);
        }
        if(transform.gameObject.activeSelf)
        {
            funcInst.Kill(transform.gameObject);
        }
        //startTime = Time.time;

        // set targeted enemy
        //currentTarget = enemy;

        // find distance to targeted enemy
        float distance = Vector3.Distance(enemy.transform.position, start);

        //get general enemy data
        VarTypes.Data data = enemy.GetComponent<EnemyController>().GetData();

        if(distance > range)
        {
            Debug.Log($"Enemy is out of range. Distance: {distance}, Range: {range}");
            Destroy(transform.gameObject);
            return;
        }

        isAttacking = true;
        Vector3 direction = start - data.position;
        startRot = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg - swingAngle / 2;
        AttackAngle = startRot;
        transform.rotation = Quaternion.Euler(0, 0, startRot);
        transform.localScale *= range;
        UpdatePosition();

        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, range))
        {
            if (!collider.gameObject.CompareTag("Enemy")) continue;

            collider.GetComponent<EnemyController>().OnHit(damage, doesStun);
            Debug.Log($"Hit enemy {enemy.name} for {damage} damage.");
        }

        // activate attack so it can move and be seen
        transform.gameObject.SetActive(true);
    }
}
