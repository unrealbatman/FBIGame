using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    [SerializeField] private Image loadingBar;
    [SerializeField] private RectTransform FXHolder;
    [SerializeField] private TextMeshProUGUI TextProgress;

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
        loadingBar.GetComponentsInChildren<Image>()[1].fillAmount = progress;
        TextProgress.text = Mathf.Floor (progress * 100).ToString();
        FXHolder.rotation = Quaternion.Euler(new Vector3(0f, 0f, -progress * 360));
        loadingBar.gameObject.SetActive(true);
        TextProgress.gameObject.SetActive(true);
        FXHolder.gameObject.SetActive(true);

    }

    private void ShowExamineAnimation()
    {

        loadingBar.gameObject.SetActive(false);
        TextProgress.gameObject.SetActive(false);
        FXHolder.gameObject.SetActive(false);
        Debug.Log("Evidence Collected, Show Animation/ zoom here");
    }

    private void HideLoadingBar()
    {
        loadingBar.fillAmount= 0;
        loadingBar.gameObject.SetActive(false);
        TextProgress.gameObject.SetActive(false);
        FXHolder.gameObject.SetActive(false);


    }
}
