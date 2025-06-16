using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    //public Text LoadingPercentage;
    //public Image LoadingProgressBar;

    private static SceneTransition instance;
    private static bool shouldPlayOpeningAnimation = false;

    private Animator componentAnimator;
    private AsyncOperation loadingSceneOperation;

    public TextMeshProUGUI textClosing;
    public TextMeshProUGUI textOpening;

    public static void SwitchToScene(string sceneName)
    {
        if (instance == null)
        {
            Debug.LogError("SceneTransition instance not found!");
            return;
        }

        // Меняем надписи в зависимости от целевой сцены
        if (sceneName == "Game")
        {
            instance.textClosing.text = "Loading scene...";
            instance.textOpening.text = "Creating Map...";
        }
        else if (sceneName == "Menu")
        {
            instance.textClosing.text = "Cleaning up...";
            instance.textOpening.text = "Returning to menu...";
        }

        instance.componentAnimator.SetTrigger("sceneClosing");

        instance.loadingSceneOperation = SceneManager.LoadSceneAsync(sceneName);
        instance.loadingSceneOperation.allowSceneActivation = false;
    }

    private void Start()
    {
        instance = this;

        componentAnimator = GetComponent<Animator>();

        if (shouldPlayOpeningAnimation)
        {
            componentAnimator.SetTrigger("sceneOpening");
            //instance.LoadingProgressBar.fillAmount = 1;

            // Чтобы если следующий переход будет обычным SceneManager.LoadScene, не проигрывать анимацию opening:
            shouldPlayOpeningAnimation = false;
        }
    }

    private void Update()
    {
        //if (loadingSceneOperation != null)
        //{
        //    LoadingPercentage.text = Mathf.RoundToInt(loadingSceneOperation.progress * 100) + "%";

        //    // Просто присвоить прогресс:
        //    //LoadingProgressBar.fillAmount = loadingSceneOperation.progress; 

        //    // Присвоить прогресс с быстрой анимацией, чтобы ощущалось плавнее:
        //    LoadingProgressBar.fillAmount = Mathf.Lerp(LoadingProgressBar.fillAmount, loadingSceneOperation.progress,
        //        Time.deltaTime * 5);
        //}
    }

    public void OnAnimationOver()
    {
        // Чтобы при открытии сцены, куда мы переключаемся, проигралась анимация opening:
        shouldPlayOpeningAnimation = true;

        loadingSceneOperation.allowSceneActivation = true;
    }
}
