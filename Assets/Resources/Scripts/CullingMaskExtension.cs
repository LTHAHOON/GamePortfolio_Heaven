using Cinemachine;
using UnityEngine;

public class CullingMaskExtension : CinemachineExtension
{
    [SerializeField]
    private LayerMask subCullingMask;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        var brain = CinemachineCore.Instance.GetActiveBrain(0);
        if ((object)brain.ActiveVirtualCamera != vcam)
            return;
        if (stage == CinemachineCore.Stage.Finalize)
        {
            if (brain.OutputCamera != null)
            {
                {
                    brain.OutputCamera.cullingMask = subCullingMask;
                }
            }
        }
    }

    public static void ChangeVirtualCamera(CinemachineVirtualCamera vcam, CinemachineVirtualCamera toVcam)
    {
        if(vcam && toVcam)
        {
            int tempPriority = toVcam.Priority;
            toVcam.Priority = vcam.Priority;
            vcam.Priority = tempPriority;
        }
    }
}