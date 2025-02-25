using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleVisibility : MonoBehaviour
{
    private Animator animator;

    //Body
    public Transform pelvis;
    public Transform spine, chest, upperChest;
    public Transform head, neck;
    //Arms
    public Transform leftShoulder, leftUpperArm, leftLowerArm, leftHand;
    public Transform rightShoulder, rightUpperArm, rightLowerArm, rightHand;
    //Legs
    public Transform leftUpperLeg, leftLowerLeg, leftFoot, leftToes;
    public Transform rightUpperLeg, rightLowerLeg, rightFoot, rightToes;

    public string avatarMaskLayerName = "Avatar";
    // Start is called before the first frame update
    void Start()
    {

        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
            return;
        }


       /* // Assign bones dynamically
        hips = GetBone(HumanBodyBones.Hips);
        spine = GetBone(HumanBodyBones.Spine);
        chest = GetBone(HumanBodyBones.Chest);
        upperChest = GetBone(HumanBodyBones.UpperChest);
        head = GetBone(HumanBodyBones.Head);
        neck = GetBone(HumanBodyBones.Neck);

        leftShoulder = GetBone(HumanBodyBones.LeftShoulder);
        leftUpperArm = GetBone(HumanBodyBones.LeftUpperArm);
        leftLowerArm = GetBone(HumanBodyBones.LeftLowerArm);
        leftHand = GetBone(HumanBodyBones.LeftHand);

        rightShoulder = GetBone(HumanBodyBones.RightShoulder);
        rightUpperArm = GetBone(HumanBodyBones.RightUpperArm);
        rightLowerArm = GetBone(HumanBodyBones.RightLowerArm);
        rightHand = GetBone(HumanBodyBones.RightHand);

        leftUpperLeg = GetBone(HumanBodyBones.LeftUpperLeg);
        leftLowerLeg = GetBone(HumanBodyBones.LeftLowerLeg);
        leftFoot = GetBone(HumanBodyBones.LeftFoot);
        leftToes = GetBone(HumanBodyBones.LeftToes);

        rightUpperLeg = GetBone(HumanBodyBones.RightUpperLeg);
        rightLowerLeg = GetBone(HumanBodyBones.RightLowerLeg);
        rightFoot = GetBone(HumanBodyBones.RightFoot);
        rightToes = GetBone(HumanBodyBones.RightToes);*/

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Transform GetBone(HumanBodyBones bone)
    {
        Transform boneTransform = animator.GetBoneTransform(bone);
        if (boneTransform == null)
        {
            Debug.LogWarning(bone + " not found on " + gameObject.name);
        }
        return boneTransform;
    }

    public void toggleGameObjectVisibility(GameObject bodyPart) 
    {
        //bodyPart.gameObject.SetActive(false);
        //bodyPart.layer = LayerMask.NameToLayer(avatarMaskLayerName);

        SkinnedMeshRenderer renderer = bodyPart.GetComponent<SkinnedMeshRenderer>();
        if(renderer != null)
        {
            renderer.enabled = !renderer.enabled;
        }
        else
        {
            Debug.LogWarning("No Renderer found on" + bodyPart.name);
        }
    
    }


}
