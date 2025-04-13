using UnityEngine;

public class StatueController : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public AudioSource moveSound;

    Renderer statueRenderer;
    Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        statueRenderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        if (!IsVisibleToCamera())
        {
            MoveTowardsPlayer();
            PlayMoveSound(true);
        }
        else
        {
            PlayMoveSound(false);
        }
    }

    bool IsVisibleToCamera()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        return GeometryUtility.TestPlanesAABB(planes, statueRenderer.bounds);
    }

    void MoveTowardsPlayer()
    {
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void PlayMoveSound(bool isMoving)
    {
        if (isMoving && !moveSound.isPlaying)
            moveSound.Play();
        else if (!isMoving && moveSound.isPlaying)
            moveSound.Stop();
    }
}

