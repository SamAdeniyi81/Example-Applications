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
    }

    public BodyPart[] bodyParts;

    void Start()
    {
        // Turn off only the MeshRenderers of the anatomy meshes, keeping them active
        foreach (var part in bodyParts)
        {
            if (part.anatomyMesh != null)
            {
                MeshRenderer[] renderers = part.anatomyMesh.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer renderer in renderers)
                {
                    renderer.enabled = false; // Hides the anatomy mesh
                }
                //Debug.Log($"Disabled anatomy mesh renderers for {part.anatomyMesh.name}");

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

                // Toggle only the MeshRenderers (so objects remain active)
                foreach (MeshRenderer renderer in part.anatomyMesh.GetComponentsInChildren<MeshRenderer>())
                {
                    renderer.enabled = !isShowingAnatomy;
                }

                foreach (SkinnedMeshRenderer renderer in part.avatarMesh.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    renderer.enabled = isShowingAnatomy;
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
        }
        Debug.Log("All anatomy meshes hidden!");
    }
}