using System.Collections;
using UnityEngine;
public class PlayerShieldShaderController : MonoBehaviour {
    public float alphaDuration;
    public float startFresnel = 0.5f;
    public Material shieldMaterial;
    private MeshRenderer meshRenderer;
    private Material[] materials;

    public void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        materials = meshRenderer.materials;
    }

    public void Reset() {
        materials = new Material[] { shieldMaterial };
        meshRenderer.materials = materials;
        materials[0].SetFloat("_FresnelPower", startFresnel);
    }

    public IEnumerator Break() {
        float elapsed = 0.0f;

        float value = startFresnel;
        materials = new Material[] { shieldMaterial };
        meshRenderer.materials = materials;
        materials[0].SetFloat("_FresnelPower", value);
        while (elapsed < alphaDuration) {
            value = -(startFresnel / alphaDuration) * elapsed + startFresnel;
            materials[0].SetFloat("_FresnelPower", value);
            meshRenderer.materials = materials;
            elapsed += Time.deltaTime;

            yield return null;
        }
        materials[0].SetFloat("_FresnelPower", 0.0f);
        meshRenderer.materials = materials;
        gameObject.SetActive(false);
        yield return null;
    }

}