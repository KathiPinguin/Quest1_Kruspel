using Codice.Client.Common.GameUI;
using PlasticPipe.PlasticProtocol.Messages;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinCounterText;
    [SerializeField] private PlayerMovement PlayerMovement;
    //[SerializeField] private Character character;
    [SerializeField] private Image Healthbar;
    [SerializeField] private CanvasGroup hudCanvasGroup;
    [SerializeField] private CanvasGroup gameOverCanvaseGroup;
    [SerializeField] private CanvasGroup victoryCanvasGroup;
    [SerializeField] private float fadingTime = 2.0f;
    private bool isFadingInGameOver = false;
    
    public IEnumerator FadeInVictory()
    {
        PlayerMovement.stopPlayer();
        unlockMouse();
        this.isFadingInGameOver = true;
        float timer = 0.0f;
        while (timer < this.fadingTime)
        {
            float percent = timer / this.fadingTime;
            this.hudCanvasGroup.alpha = 1.0f - percent;
            this.victoryCanvasGroup.alpha = percent;
            yield return null;
            timer += Time.deltaTime;
        }
        this.hudCanvasGroup.alpha = 0.0f;
        this.victoryCanvasGroup.alpha = 1.0f;
        this.isFadingInGameOver = false;

    }

    public void victroy()
    {
        unlockMouse();
        this.StartCoroutine(this.FadeInVictory());
    }

    private IEnumerator FadeInGameOver()
    {
        unlockMouse();
        this.isFadingInGameOver = true;
        float timer = 0.0f;
        while (timer < this.fadingTime)
        {
            float percent = timer / this.fadingTime;
            this.hudCanvasGroup.alpha = 1.0f - percent;
            this.gameOverCanvaseGroup.alpha = percent;
            yield return null;
            timer += Time.deltaTime;
        }
        this.hudCanvasGroup.alpha = 0.0f;
        this.gameOverCanvaseGroup.alpha = 1.0f;
        

    }

    public IEnumerator FadeOutGameOver()
    {
        unlockMouse();
        this.isFadingInGameOver = true;
        float timer = 0.0f;
        while (timer < this.fadingTime)
        {
            float percent = timer / this.fadingTime;
            this.hudCanvasGroup.alpha = percent;
            this.gameOverCanvaseGroup.alpha = 1.0f - percent;
            yield return null;
            timer += Time.deltaTime;
        }
        this.hudCanvasGroup.alpha = 1.0f ;
        this.gameOverCanvaseGroup.alpha = 0.0f;
        this.isFadingInGameOver = false;



    }

    private void Update()
    {
        float percent = this.PlayerMovement.GetCurrentHealth() / this.PlayerMovement.GetmaxHealth();
        this.Healthbar.fillAmount = percent;
        if (percent <= 0.0f && !this.isFadingInGameOver)
        {
            this.StartCoroutine(this.FadeInGameOver());
        }
    }

    public void lockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void unlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    private static UIManager instance = null;
    public static UIManager Instance => instance;
    private class PlayerStatistics
    {
        public int coinCounter = 0;
        //... add more statistics ^^ (e.g. enemies jumped on etc.)
    }
    private PlayerStatistics statistics;
    private void Awake()
    {
        instance = this;
        this.statistics = new PlayerStatistics() { coinCounter = 0 };
    }

    private void Start()
    {
        this.lockMouse();
    }

    public void CollectCoin()
    {
        this.statistics.coinCounter++;
        string coinText = $" {this.statistics.coinCounter} ";
        this.coinCounterText.text = coinText;
    }

    public void looseCoin()
    {
        statistics.coinCounter = 0;
        string coinText = $" {this.statistics.coinCounter} ";
        this.coinCounterText.text = coinText;
    }

    public void quit()
    {
        UnityEngine.Application.Quit();
    }

}