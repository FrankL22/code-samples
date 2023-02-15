using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private static HUD mCurrent;
    public static HUD Current
    {
        get { return mCurrent; }
    }

    private const float ARROW_OFFSET = 20f;

    private bool mPlayingTimerAnim = false;

    private Slider mResourceBar;
    private Text mResourceText;
    private Text mSprayText;
    private Text mEnemySprayText;
    private Text mTimerText;
    private CountDown mCountDown;

    private Dictionary<Transform, GameObject> mEnemyIndicators = new Dictionary<Transform, GameObject>();

    private void Awake()
    {
        mCurrent = this;

        mTimerText = transform.Find("Timer").GetComponent<Text>();
        mResourceBar = transform.Find("ResourceBar").GetComponent<Slider>();
        mResourceBar.maxValue = Utilities.RESOURCE_DISPLAY_MAX;
        mResourceText = mResourceBar.transform.Find("ResourceText").GetComponent<Text>();
        var spray = transform.Find("SprayPercentage");
        mSprayText = spray.Find("SprayText").GetComponent<Text>();
        mEnemySprayText = spray.Find("EnemySprayText").GetComponent<Text>();
        mCountDown = transform.Find("Countdown").GetComponent<CountDown>();
    }

    private void OnDestroy()
    {
        if (mCurrent == this) mCurrent = null;
    }

    private void FixedUpdate()
    {
        var playerPos = PlayerControl.Current.Position;

        foreach (var enemy in mEnemyIndicators)
        {
            var enemyPos = enemy.Key.position;
            var arrow = enemy.Value;
            if (arrow.activeSelf && CheckOnScreen(enemyPos))
            {
                arrow.SetActive(false);
                continue;
            }
            else if (!arrow.activeSelf && !CheckOnScreen(enemyPos))
            {
                arrow.SetActive(true);
            }
            arrow.transform.position = ClampToScreenPos(enemyPos);
            arrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, enemyPos - playerPos);
        }
    }

    public void StartGameDelayed(int pDelay)
    {
        mCountDown.SetTime(pDelay);
    }

    public void ToggleShowHUD(bool pShow)
    {
        GetComponent<Canvas>().enabled = pShow;
    }

    public void UpdateTimer(float pTime)
    {
        if (pTime < 0) pTime = 0;

        if (!mPlayingTimerAnim && pTime < Utilities.COUNTDOWN_THRESHOLD)
        {
            ToggleTimerAnimation(true);
        }

        mTimerText.text = pTime.ToString("F1") + "s";
    }

    public void UpdateResourceBar(float pAmount)
    {
        mResourceBar.value = pAmount;
        mResourceText.text = ((int)pAmount).ToString();
    }

    public void UpdateSprayPercentage(float pPlayer, float pEnemy)
    {
        mSprayText.text = pPlayer.ToString("P1");
        mEnemySprayText.text = pEnemy.ToString("P1");
    }

    public void RegisterEnemy(Transform pEnemy)
    {
        if (mEnemyIndicators.ContainsKey(pEnemy)) return;
        var arrowPrefab = PrefabManager.Instance.GetPrefab(Utilities.ENEMY_ARROW_NAME);
        var arrow = GameObject.Instantiate(arrowPrefab);
        mEnemyIndicators.Add(pEnemy, arrow);
    }

    public void RemoveEnemy(Transform pEnemy)
    {
        if (!mEnemyIndicators.ContainsKey(pEnemy)) return;
        mEnemyIndicators.Remove(pEnemy);
    }

    public void ToggleTimerAnimation(bool pActive)
    {
        mPlayingTimerAnim = pActive;
        mTimerText.gameObject.GetComponent<Animator>().enabled = pActive;
    }

    private Vector3 ClampToScreenPos(Vector3 pPos)
    {
        pPos = Camera.main.WorldToScreenPoint(pPos);

        if (pPos.x < ARROW_OFFSET) pPos.x = ARROW_OFFSET;
        else if (pPos.x > Screen.width - ARROW_OFFSET) pPos.x = Screen.width - ARROW_OFFSET;

        if (pPos.y < ARROW_OFFSET) pPos.y = ARROW_OFFSET;
        else if (pPos.y > Screen.height - ARROW_OFFSET) pPos.y = Screen.height - ARROW_OFFSET;

        pPos =  Camera.main.ScreenToWorldPoint(pPos);
        pPos.z = 0;
        return pPos;
    }

    private bool CheckOnScreen(Vector3 pPos)
    {
        pPos = Camera.main.WorldToScreenPoint(pPos);
        if (pPos.x < 0 || pPos.x > Screen.width) return false;
        if (pPos.y < 0 || pPos.y > Screen.height) return false;
        return true;
    }
}
