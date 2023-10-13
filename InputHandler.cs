using UnityEngine;
using UnityEngine.EventSystems;
using HolagoGames;

public class InputHandler : MonoBehaviour
{
    private const float treshold = 0.25f;
    private Vector2 startPos;
    private Vector2 deltaPos;

    float RAYLENGHT = 200f;
    Ray ray;
    RaycastHit hit;

    private void OnEnable()
    {
        Holago.SystemContainer.EventSystem.OnPointerDown.Register(OnPointerDown);
        Holago.SystemContainer.EventSystem.OnPointerUp.Register(OnPointerUp);
        Holago.SystemContainer.EventSystem.OnDrag.Register(OnDrag);
    }

    private void OnDisable()
    {
        Holago.SystemContainer.EventSystem.OnPointerDown.UnRegister(OnPointerDown);
        Holago.SystemContainer.EventSystem.OnPointerUp.UnRegister(OnPointerUp);
        Holago.SystemContainer.EventSystem.OnDrag.UnRegister(OnDrag);
    }

    private void OnPointerDown(PointerEventData data)
    {
        ray = Holago.SystemContainer.CameraSystem.MainCamera.ScreenPointToRay(data.position);
        Debug.DrawRay(ray.origin, ray.direction * RAYLENGHT, Color.red, 5, true);

        // Debug.Log("OnPointerDown: " + data.position);
        startPos = data.position;
    }
    private void OnPointerUp(PointerEventData data)
    {
        // Debug.Log("OnPointerUp: " + data.position);
    }

    private void OnDrag(PointerEventData data)
    {
        deltaPos = data.position - startPos;

        if (deltaPos.magnitude >= treshold && Physics.Raycast(ray, out hit, RAYLENGHT))
        {
            CarMovement carMovement = hit.collider.GetComponent<CarMovement>();

            if (carMovement != null)
            {
                Vector3 dragDirection = deltaPos.normalized;
                carMovement.HandleDirection(dragDirection);
            }
        }
        // Debug.Log("OnDrag: " + data.position);       
    }
}