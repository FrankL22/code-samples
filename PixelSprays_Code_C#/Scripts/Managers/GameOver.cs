using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject mPauseMenu;
    private Text mResultText;

    private void Awake()
    {
        mResultText = transform.Find("ResultText").GetComponent<Text>();
        gameObject.SetActive(false);
        GameManager.Instance.mGameOver = this;
    }

    public void Show(int pResult, float pPlayer, float pEnemy)
    {
        float playerCountTime = Utilities.PERCENT_COUNT_TIME_BASE;
        float enemyCountTime = Utilities.PERCENT_COUNT_TIME_BASE;
        GameObject image = null;
        switch (pResult)
        {
            case 1:
                // ”Æ
                mResultText.text = $"You've won!";
                playerCountTime = Utilities.PERCENT_COUNT_TIME_BASE * (pEnemy == 0 ? 1 : pPlayer / pEnemy);
                image = transform.Find("PlayerWon").gameObject;
                break;
            case 0:
                // ∆Ω
                mResultText.text = $"Draw game";
                image = transform.Find("PlayerDraw").gameObject;
                break;
            case -1:
                //  ‰
                mResultText.text = $"You've lost...";
                enemyCountTime = Utilities.PERCENT_COUNT_TIME_BASE * (pPlayer == 0 ? 1 : pEnemy / pPlayer);
                image = transform.Find("PlayerLost").gameObject;
                break;
        }

        playerCountTime = Mathf.Min(playerCountTime, Utilities.PERCENT_COUNT_TIME_CAP);
        enemyCountTime = Mathf.Min(enemyCountTime, Utilities.PERCENT_COUNT_TIME_CAP);

        transform.Find("PlayerPercent").GetComponent<PercentCountUp>().CountTo(pPlayer, playerCountTime);
        transform.Find("EnemyPercent").GetComponent<PercentCountUp>().CountTo(pEnemy, enemyCountTime);

        gameObject.SetActive(true);
        StartCoroutine(DelayShowResult(Mathf.Max(playerCountTime, enemyCountTime), image));
    }

    private IEnumerator DelayShowResult(float pDelay, GameObject pImage)
    {
        yield return new WaitForSeconds(pDelay);
        mResultText.gameObject.SetActive(true);
        pImage?.SetActive(true);
        transform.Find("RestartText").gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        GameManager.Instance.StartGame();
    }

    public void PauseGame() => GameManager.Instance.PauseGame(mPauseMenu);
    public void ResumeGame() => GameManager.Instance.ResumeGame(mPauseMenu);

    public void QuitGame() => GameManager.Instance.BackToTitle();
}
