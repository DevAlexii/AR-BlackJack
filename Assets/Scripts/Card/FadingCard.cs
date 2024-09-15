using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
public class FadingCard : MonoBehaviour
{
    private MeshRenderer _renderer;
    [SerializeField]
    private Color emissiveColor = Color.blue;
    [SerializeField]
    private float frequency = 5.0f;
    [SerializeField]
    private float maxIntensity = 1.0f;
    [SerializeField]
    private float minIntensity = 0.0f;

    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
    }
    private void Update()
    {
        // Calculate the emission intensity by sin function.
        // Adding 1 to sin value and dividing by 2 normalizes it to a 0 to 1 range.
        float emissionIntensity = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(Time.time * frequency) + 1.0f) / 2.0f);
        _renderer.materials[0].SetColor("_EmissionColor", emissiveColor * emissionIntensity);
    }
}
