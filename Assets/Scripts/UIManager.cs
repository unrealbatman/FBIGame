using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image loadingBar;
    [SerializeField] private RectTransform fxHolder;
    [SerializeField] private TextMeshProUGUI textProgress;
    [SerializeField] private GameObject crosshair;

    [Header("FX Settings")]
    [SerializeField] private ParticleSystem loadingParticle;

    private RawImage crosshairImage;


    private void Awake()
    {
        // Cache the RawImage component of the crosshair
        crosshairImage = crosshair.GetComponent<RawImage>();
        if (!crosshairImage)
        {
            Debug.LogWarning("Crosshair does not have a RawImage component!");
        }
    }

    private void OnEnable()
    {
        PlayerController.OnLoadingProgress  += UpdateLoadingBar;
        PlayerController.OnLoadingComplete  += ShowExamineAnimation;
        PlayerController.OnLoadingCancelled += HideLoadingBar;
    }

    private void OnDisable()
    {
        PlayerController.OnLoadingProgress  -= UpdateLoadingBar;
        PlayerController.OnLoadingComplete  -= ShowExamineAnimation;
        PlayerController.OnLoadingCancelled -= HideLoadingBar;
    }



    private void UpdateLoadingBar(float progress)
    {
        // Update loading bar fill
        loadingBar.GetComponentsInChildren<Image>()[1].fillAmount = progress;

        // Update progress text
        textProgress.text = Mathf.Floor (progress * 100).ToString();

        // Activate UI elements if not already active
        ActivateLoadingUI(true);

        // Rotate FX Holder
        fxHolder.rotation = Quaternion.Euler(new Vector3(0f, 0f, -progress * 360));

        // Play loading particle if it's not already playing
        if (!loadingParticle.isPlaying)
        {
            loadingParticle.Play();
        }

        // Adjust crosshair transparency
        SetCrosshairAlpha(0f);
        
    }

    private void ShowExamineAnimation()
    {
        // Hide loading UI elements
        ActivateLoadingUI(false);

        // Stop the loading particle
        loadingParticle.Stop();

        //Display extracted info

        Debug.Log("Evidence Collected, Show Animation/ zoom here");
    }

    private void HideLoadingBar()
    {
        // Reset loading bar fill
        loadingBar.fillAmount= 0;

        // Hide loading UI elements
        ActivateLoadingUI(false);

        // Reset crosshair transparency
        SetCrosshairAlpha(255f);

    }


    private void ActivateLoadingUI(bool isActive)
    {
        loadingBar.gameObject.SetActive(isActive);
        textProgress.gameObject.SetActive(isActive);
        fxHolder.gameObject.SetActive(isActive);
    }

    private void SetCrosshairAlpha(float alpha)
    {
        if (crosshairImage)
        {

            Color color = crosshairImage.color;
            color.a = alpha;
            crosshairImage.color = color;

        }
        
    }
}
