using Assets.Globals;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float damageArea = 0.5f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float speed = 20;
    private VarTypes.Data data;
    private Vector3 AttackVel;
    private Transform AttackPos;
    private float deltaTime;
    private Vector2 startPos;
    public VarTypes.Data GetData() => data;
    private GameObject currentTarget;
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

        if (Vector3.Distance(AttackPos.position, currentTarget.transform.position) < 0.5f)
        {
            currentTarget.GetComponent<EnemyController>().OnHit(damage);
            transform.gameObject.SetActive(false);
        }
    }


    public void OnAttack(GameObject enemy, Vector3 start)
    {
        if (AttackPos.gameObject.activeSelf) return;
        Vector3 direction;
        currentTarget = enemy;
        float distance = Vector3.Distance(enemy.transform.position, start);
        VarTypes.Data data = enemy.GetComponent<EnemyController>().GetData();

        direction = (distance/speed * data.velocity) + data.position.position - start;
        Debug.Log($"Speed: {speed}");
        Debug.Log($"{distance / speed * data.velocity}");

        AttackVel = direction.normalized * speed;
        startPos = start;
        AttackPos.position = startPos;
        AttackPos.rotation = Functions.ToQuaternion(AttackVel.normalized);
        Debug.Log($"{Time.time}: {AttackVel}");
        transform.gameObject.SetActive(true);
        UpdatePosition();
    }
}
