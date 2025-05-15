using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartToggle : MonoBehaviour
{
    [System.Serializable]
    public class BodyPart
    {
        public GameObject avatarMesh;  // Avatar's SkinnedMeshRenderer parent
        public GameObject anatomyMesh; // Anatomy MeshRenderer parent

        [HideInInspector] public Material[] originalMaterials; // Cache original avatar materials

    }

    public BodyPart[] bodyParts;
    public Material anatomyMaterial; // Material to use when anatomy is shown


    void Start()
    {
        // Turn off only the MeshRenderers of the anatomy meshes, keeping them active
        foreach (var part in bodyParts)
        {
            var smr = part.avatarMesh.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
            {
                part.originalMaterials = smr.materials;
            }

            // Disable anatomy renderers
            if (part.anatomyMesh != null)
            {
                foreach (MeshRenderer renderer in part.anatomyMesh.GetComponentsInChildren<MeshRenderer>())
                {
                    renderer.enabled = false;
                    //
                }
            }
        }
    }

    public void ToggleBodyPart(GameObject partObject)
    {
        foreach (var part in bodyParts)
        {
            if (part.avatarMesh == partObject || part.anatomyMesh == partObject)
            {
                bool isShowingAnatomy = part.anatomyMesh.GetComponentInChildren<MeshRenderer>().enabled;

                // Toggle anatomy visibility
                foreach (MeshRenderer renderer in part.anatomyMesh.GetComponentsInChildren<MeshRenderer>())
                {
                    renderer.enabled = !isShowingAnatomy;
                }

                // Change avatar material
                var smr = part.avatarMesh.GetComponentInChildren<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    if (!isShowingAnatomy)
                    {
                        // Swap all materials to anatomyMaterial
                        Material[] anatomyMats = new Material[smr.materials.Length];
                        for (int i = 0; i < anatomyMats.Length; i++)
                        {
                            anatomyMats[i] = anatomyMaterial;
                        }
                        smr.materials = anatomyMats;
                    }
                    else
                    {
                        // Restore original materials
                        smr.materials = part.originalMaterials;
                    }
                }

                return;
            }
        }

        Debug.LogWarning("Body part not found!");
    }

    public void HideAllAnatomyMeshes()
    {

        foreach (var part in bodyParts)
        {
            if (part.anatomyMesh != null)
            {
                foreach (MeshRenderer renderer in part.anatomyMesh.GetComponentsInChildren<MeshRenderer>())
                {
                    renderer.enabled = false;
                }
            }

            // Restore avatar materials
            var smr = part.avatarMesh.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null && part.originalMaterials != null)
            {
                smr.materials = part.originalMaterials;
            }
        }

        Debug.Log("All anatomy meshes hidden!");
    }
}