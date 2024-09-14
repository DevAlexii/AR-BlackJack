using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private void OnEnable()
    {
        Canvas canvas  = GetComponent<Canvas>();
        transform.LookAt(Camera.main.transform);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
    }
  
}