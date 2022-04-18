using System.Collections;
using UnityEngine;
public class DissolveShaderController : MonoBehaviour {
    public float dissolveTime;
    public Material dissolveMaterial;
    public bool terminateOnDissolve;
    private MeshRenderer meshRenderer;
    private Material[] materials;

    public void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        materials = meshRenderer.materials;
    }

    public IEnumerator Dissolve() {
        float elapsed = 0.0f;

        float value = 0.0f;
        materials = new Material[] { dissolveMaterial };
        meshRenderer.materials = materials;
        materials[0].SetFloat("_Cutoff", value);
        while (elapsed < dissolveTime) {
            value = elapsed / dissolveTime;
            materials[0].SetFloat("_Cutoff", value);
            meshRenderer.materials = materials;
            elapsed += Time.deltaTime;

            yield return null;
        }
        materials[0].SetFloat("_Cutoff", 1.0f);
        meshRenderer.materials = materials;
        if (terminateOnDissolve) {
            Destroy(gameObject);
        } else {
            gameObject.SetActive(false);
        }
        yield return null;
    }

}