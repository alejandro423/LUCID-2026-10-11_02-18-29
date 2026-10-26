using UnityEngine;

public class SeguirCamara : MonoBehaviour
{
    [SerializeField]
    private Transform objetivo;

    [SerializeField]
    private float suavizado = 5f;

    private void LateUpdate()
    {
        Vector3 posicionDeseada = new Vector3(
            objetivo.position.x,
            objetivo.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            posicionDeseada,
            suavizado * Time.deltaTime
        );
    }
}