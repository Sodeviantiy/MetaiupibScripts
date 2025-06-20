using UnityEngine;
using Michsky.MUIP;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class QualityController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private CustomDropdown qualityDropdown;
    [SerializeField] private CustomDropdown upscalingDropdown;

    private UniversalRenderPipelineAsset currentURPAsset;
    private readonly string[] upscalingOptions = { "Linear", "Point", "FSR", "STP" };

    void Start()
    {
        InitializeQualityDropdown();
        InitializeUpscalingDropdown();
        UpdateCurrentURPAsset();
    }

    void InitializeQualityDropdown()
    {
        
        string[] qualityNames = QualitySettings.names;

        foreach (string qualityName in qualityNames)
        {
            qualityDropdown.CreateNewItem(qualityName, null, false);
        }

        qualityDropdown.SetupDropdown();
        qualityDropdown.ChangeDropdownInfo(QualitySettings.GetQualityLevel());
        qualityDropdown.onValueChanged.AddListener(ChangeQualityLevel);
    }

    void InitializeUpscalingDropdown()
    {
        

        foreach (string option in upscalingOptions)
        {
            upscalingDropdown.CreateNewItem(option, null, false);
        }

        upscalingDropdown.SetupDropdown();
        upscalingDropdown.onValueChanged.AddListener(ChangeUpscalingMethod);
        UpdateUpscalingDropdownState();
    }

    void UpdateCurrentURPAsset()
    {
        currentURPAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
    }

    void UpdateUpscalingDropdownState()
    {
        if (currentURPAsset != null)
        {
            int index = GetCurrentUpscalingIndex();
            upscalingDropdown.ChangeDropdownInfo(index);
        }
    }

    int GetCurrentUpscalingIndex()
    {
        if (currentURPAsset == null) return 0;

        return currentURPAsset.upscalingFilter switch
        {
            UpscalingFilterSelection.Linear => 0,
            UpscalingFilterSelection.Point => 1,
            UpscalingFilterSelection.FSR => 2,
            UpscalingFilterSelection.STP => 3,
            _ => 0
        };
    }

    public void ChangeQualityLevel(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        qualityDropdown.Animate();
        UpdateCurrentURPAsset();
        UpdateUpscalingDropdownState();
    }

    private void ChangeUpscalingMethod(int index)
    {
        if (currentURPAsset == null) return;

        currentURPAsset.upscalingFilter = index switch
        {
            0 => UpscalingFilterSelection.Linear,
            1 => UpscalingFilterSelection.Point,
            2 => UpscalingFilterSelection.FSR,
            3 => UpscalingFilterSelection.STP,
            _ => UpscalingFilterSelection.Linear
        };

        QualitySettings.renderPipeline = currentURPAsset;
        upscalingDropdown.Animate();
    }
}