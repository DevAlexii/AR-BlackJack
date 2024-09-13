using UnityEngine;

public class LightSwing : MonoBehaviour
{
    float alpha;
    [SerializeField]
    float speed;
    // Update is called once per frame
    void Update()
    {
        alpha += Time.deltaTime * speed;

        Vector3 rot = new Vector3(90 + Mathf.Sin(alpha) * 10, 90,0 );
        transform.rotation = Quaternion.Euler(rot);
    }
}
