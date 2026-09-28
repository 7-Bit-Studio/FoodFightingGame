using Assets.Globals;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAttack : MonoBehaviour
{
    private VarTypes.Data data;
    private Vector3 AttackVel;
    private Transform AttackPos;
    private float deltaTime;
    private Vector2 startPos;
    public VarTypes.Data GetData() => data;
    private GameObject currentTarget;
    private int damage;
    private float speed;
    private Vector3 direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AttackPos = GetComponent<Transform>();
        AttackPos.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime = Time.deltaTime;
        UpdatePosition();
    }
    void UpdatePosition()
    {
        AttackPos.position += deltaTime * AttackVel;
        AttackPos.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(-AttackVel.x, AttackVel.y)));

        transform.SetPositionAndRotation(AttackPos.position, AttackPos.rotation);

        if (Mathf.Abs(AttackPos.position.x - startPos.x) > 10 || Mathf.Abs(AttackPos.position.y - startPos.y) > 10)
        {
            transform.gameObject.SetActive(false);
            AttackVel = Vector3.zero;
            AttackPos.position = startPos;
        }

        if (Vector3.Distance(AttackPos.position, currentTarget.transform.position) < 0.2f)
        {
            currentTarget.GetComponent<EnemyController>().OnHit(damage);
            transform.gameObject.SetActive(false);
        }
        if (transform.gameObject.activeSelf)
        {
            direction = currentTarget.transform.position - AttackPos.position;
            AttackVel = direction.normalized * speed;
        }
    }


    public void OnAttack(GameObject enemy, Vector3 start, float speed, int damage)
    {
        if (AttackPos.gameObject.activeSelf) return;
        Vector3 direction = enemy.transform.position - start;
        currentTarget = enemy;
        this.damage = damage;
        this.direction = direction;
        this.speed = speed;
        AttackVel = direction.normalized * speed;
        startPos = start;
        AttackPos.position = startPos;
        AttackPos.rotation = Functions.ToQuaternion(AttackVel.normalized);
        Debug.Log($"{AttackVel}");
        transform.gameObject.SetActive(true);
        UpdatePosition();
    }
}
