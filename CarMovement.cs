using DG.Tweening;
using HolagoGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    Rigidbody rb;
    RoadPiece currentRoad;
    [SerializeField] RoadManager roadManager;
    [SerializeField] LevelChecker levelEndChecker;
    ParticleSystem hitParticle;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 5f;
    Vector3 moveDir;
    Vector3 orientation;
    int currentRoadID;
    int unit = 1;
    float step;

    bool isOnRoad = false;
    bool hasEngineSFXPlayed = false;

    float movementTimestamp;

    private void Awake()
    {
        enabled = false;
        rb = GetComponent<Rigidbody>();
        hitParticle = GetComponentInChildren<ParticleSystem>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        step = moveSpeed * Time.fixedDeltaTime;
        moveDir = transform.position + (unit * orientation) * step;
        Vector3 currentPos = Vector3.MoveTowards(transform.position, moveDir, step);
        rb.MovePosition(currentPos);
    }

    public void HandleDirection(Vector2 direction)
    {
        enabled = true;
        movementTimestamp = Time.time;

        if (direction.x != 0)
        {
            unit = (int)Mathf.Sign(direction.x);
            orientation = transform.forward;
        }
        else if (direction.y != 0)
        {
            unit = (int)Mathf.Sign(direction.y);
            orientation = Vector3.forward;
        }
    }

    IEnumerator FollowRoadCO()
    {
        float initY = transform.position.y;

        int currentIndex = roadManager.roadPieces.FindIndex(road => road.roadID == currentRoadID);
        int numRoadPieces = roadManager.roadPieces.Count;

        Vector3[] waypoints = new Vector3[numRoadPieces - currentIndex];

        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = roadManager.roadPieces[currentIndex + i].transform.position;
            waypoints[i].y = initY;
        }

        if (currentRoadID == roadManager.roadPieces.Count - 1)
        {
            levelEndChecker.RemoveCar(gameObject);
        }

        yield return transform.DOPath(waypoints, moveSpeed, PathType.CatmullRom, PathMode.Full3D)
            .SetLookAt(0.05f, Vector3.forward)
            .SetSpeedBased()
            .WaitForCompletion();
    }


    //IEnumerator FollowRoadCO()
    //SET ROAD COLLIDERS' SIZE TO : 5 - 2 - 5
    //{
    //    int currentIndex = roadManager.roadPieces.FindIndex(road => road.roadID == currentRoadID);

    //    //SET NEXT WAYPOINT
    //    while (currentIndex < roadManager.roadPieces.Count)
    //    {
    //        int nextRoadID = roadManager.roadPieces[currentIndex].nextRoadID;
    //        int nextIndex = roadManager.roadPieces.FindIndex(road => road.roadID == nextRoadID);

    //        currentRoad = roadManager.roadPieces[nextIndex];

    //        Vector3 targetPos = roadManager.roadPieces[nextIndex].transform.position;
    //        targetPos.y = transform.position.y;

    //        //SET DIRECTION
    //        Vector3 moveDirection = (targetPos - transform.position).normalized;
    //        while (Vector3.Distance(transform.position, targetPos) > 3f)
    //        {
    //            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed / 4f * Time.deltaTime);
    //            //SET ROTATION
    //            if (moveDirection != Vector3.zero)
    //            {
    //                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
    //                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    //            }

    //            yield return null;
    //        }
    //        currentIndex = nextIndex;
    //        currentRoadID = nextRoadID;

    //        //DESTROY
    //        if (currentRoadID == roadManager.roadPieces.Count - 1)
    //        {
    //            levelEndChecker.RemoveCar(gameObject);
    //        }
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherGameObject = other.gameObject;  

        if (otherGameObject.CompareTag("Road"))
        {
            isOnRoad = true;

            currentRoad = other.GetComponent<RoadPiece>();
            currentRoadID = currentRoad.roadID;

            if (hasEngineSFXPlayed == false)
            {
                Holago.SystemContainer.EventSystem.PlayEngineSFX.Invoke();
            }
            hasEngineSFXPlayed = true;

            StartCoroutine(FollowRoadCO());
        }

        else if (other.CompareTag("Car"))
        {
            enabled = false;

            CarMovement otherCar = other.GetComponent<CarMovement>();

            if (otherCar == null)
                return;

            if (movementTimestamp > otherCar.GetMovementTimestamp())
            {
                HitAnimation();
                hitParticle.Play();
            }
        }

        else if (otherGameObject.CompareTag("Obstacle"))
        {
            if (isOnRoad)
                return;

            enabled = false;

            HitAnimation();
            hitParticle.Play();
        }

        else if (otherGameObject.CompareTag("Gate"))
        {
            other.GetComponent<Animator>().SetTrigger("open");
            Holago.SystemContainer.EventSystem.PlaySuccessSFX.Invoke();
        }
    }
    public float GetMovementTimestamp()
    {
        return movementTimestamp;
    }

    private void HitAnimation()
    {
        Holago.SystemContainer.EventSystem.PlayCrashSFX.Invoke();

        Vector3 originalPosition = transform.position - transform.forward * unit;
        Vector3 collisionPosition = transform.position + transform.forward * 0.1f;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(collisionPosition, 0.1f));
        sequence.Append(transform.DOMove(originalPosition, 0.1f));

    }
}