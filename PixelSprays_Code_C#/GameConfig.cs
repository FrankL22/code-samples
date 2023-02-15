using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameConfig : MonoBehaviour
{
    [SerializeField] private Slider mEnemyNum;
    private Text mEnemyNumText;
    [SerializeField] private Slider mGameTime;
    private Text mGameTimeText;

    private void Awake()
    {
        mEnemyNumText = mEnemyNum.transform.Find("Num").GetComponent<Text>();
        mGameTimeText = mGameTime.transform.Find("Num").GetComponent<Text>();
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        GameManager.Instance.UpdateGameConfig((int)mEnemyNum.value, mGameTime.value);
        gameObject.SetActive(false);
    }

    public void OnUpdateValue()
    {
        mEnemyNumText.text = mEnemyNum.value.ToString();
        mGameTimeText.text = mGameTime.value.ToString();
    }
}
