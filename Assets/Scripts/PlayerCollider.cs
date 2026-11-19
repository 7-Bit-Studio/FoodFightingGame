using Assets.Globals;
using JetBrains.Annotations;
using JetBrains.Rider.Unity.Editor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerCollider : MonoBehaviour
{
    private CircleCollider2D playerCollider;
    private PlayerController player;
    [SerializeField] private LayerMask wallLayer;
    private float radius;
    private static readonly Vector2[] dirs = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private readonly RaycastHit2D[] hits = new RaycastHit2D[8];
    private ContactFilter2D filter;

    void Start()
    {
        player = gameObject.GetComponent<PlayerController>();
        playerCollider = gameObject.GetComponent<CircleCollider2D>();
        radius = playerCollider.radius;
        filter = new ContactFilter2D();
        filter.SetLayerMask(wallLayer);
        filter.useTriggers = false;
    }

    void Update()
    {

        for (int d = 0; d < dirs.Length; d++)
        {
            int count = playerCollider.Raycast(dirs[d], filter, hits, radius);
            for (int i = 0; i < count; i++)
            {
                player.playerPos -= (Vector3)dirs[d] * (radius - hits[i].distance);
            }
        }
    }
}
