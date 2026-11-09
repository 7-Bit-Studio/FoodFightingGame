using UnityEngine;
using Assets.Globals;
using Unity.VisualScripting;

public class PlayerAttackMelee : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] public float swingSpeed = 20;
    [SerializeField] private float range = 5;
    [SerializeField] public float swingAngle = 90;
    [SerializeField] private bool doesStun = false;
    private float deltaTime;
    private float startRot;
    private bool isAttacking;
    private float AttackAngle;
    private VarTypes.Data playerData;
    void Awake()
    {
        //startPos = transform.position;
        transform.gameObject.SetActive(false);
        startRot = transform.rotation.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime = Time.deltaTime;
        playerData = Functions.GetSiblingGameObject("Player").GetComponent<PlayerController>().GetData();
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
                return;
            }
            else
            {
                playerData = Functions.GetSiblingGameObject("Player").GetComponent<PlayerController>().GetData();
                AttackAngle += swingSpeed * deltaTime;
                transform.position = new Vector3(Mathf.Sin(AttackAngle * Mathf.Deg2Rad) * 0.5f + playerData.position.x, -Mathf.Cos(AttackAngle * Mathf.Deg2Rad) * 0.5f + playerData.position.y, 0);
                transform.localRotation = Quaternion.Euler(0, 0, AttackAngle);
                Debug.Log($"Attacking. Angle: {startRot - AttackAngle}");
                Debug.Log($"{startRot} vs {AttackAngle}");
            }
        }
    }


    public void OnAttack(GameObject enemy, Vector3 start)
    {
        // if targeted enemy is not active, return
        if (!enemy.activeSelf) return;
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
            return;
        }

        isAttacking = true;
        Vector3 direction = start - data.position;
        startRot = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg - swingAngle / 2;
        AttackAngle = startRot;
        transform.rotation = Quaternion.Euler(0, 0, startRot);
        UpdatePosition();

        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, 0.5f))
        {
            if (collider.gameObject == enemy)
            {
                collider.GetComponent<EnemyController>().OnHit(damage, doesStun);
                Debug.Log($"Hit enemy {enemy.name} for {damage} damage.");
            }
        }

        // activate attack so it can move and be seen
        transform.gameObject.SetActive(true);
    }
}
