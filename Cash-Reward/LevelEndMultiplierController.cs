using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace HolagoGames
{
    public class LevelEndMultiplierController : MonoBehaviour
    {
        [SerializeField]
        private HolagoCurrencyManager currencyManager;
        [SerializeField]
        private RectTransform arrowPanel;
        [SerializeField]
        private RectTransform normalCashButton;
        [SerializeField]
        private TextMeshProUGUI multiplyText;
        [SerializeField]
        private TextMeshProUGUI claimText;
        [SerializeField]
        private TextMeshProUGUI normalCashText;

        [SerializeField]
        private Transform buttonsPanel;

        [SerializeField]
        private GameObject multiplierButton, multiplierIcon;

        [SerializeField]
        private GameObject multiplierPanel;


        private EventSystem eventSystem;


        private int cashCount;
        private bool doArrowAnim;
        private float currentArrowAngle;
        private bool pressed = false;

        private void Awake()
        {
            eventSystem = Holago.SystemContainer.EventSystem;
        }

        private void OnEnable()
        {
            normalCashButton.gameObject.SetActive(false);
            //StartCoroutine(OpenNormalButton());
            StartCoroutine(InitCor());
            pressed = false;
            doArrowAnim = true;
            buttonsPanel.localScale = Vector3.zero;
        }
        private void OnDisable()
        {
            StopAllCoroutines();
            multiplierPanel.SetActive(false);
        }

        private IEnumerator InitCor()
        {
            var wait = new WaitForFixedUpdate();
            yield return wait;
            yield return wait;
            yield return wait;
            cashCount = currencyManager.GetEarnedCash();

            var levelSystem = Holago.SystemContainer.LevelSystem;
            var currentLevel = levelSystem.CurrentLevel;

            buttonsPanel.DOScale(Vector3.one, .4f).SetEase(Ease.OutBack);
            normalCashText.text = cashCount.ToString();
            normalCashButton.gameObject.SetActive(true);

            if (currentLevel > 100)
            {
                arrowPanel.gameObject.SetActive(true);
                multiplierButton.SetActive(true);
                multiplierIcon.SetActive(true);
            }
            else
            {
                arrowPanel.gameObject.SetActive(false);
                multiplierButton.SetActive(false);
                multiplierIcon.SetActive(false);
            }
            multiplierPanel.SetActive(true);
        }

        public void OnNormalButtonPressed()
        {
            if (pressed)
                return;

            doArrowAnim = false;

            eventSystem.OnCashAnimRequested.Invoke((1, true));
            currencyManager.SetEarnedCurrency(cashCount);

            pressed = true;

            
        }
        public void OnRWButtonPressed()
        {
            if (pressed)
                return;

            doArrowAnim = false;
            OnRWCompleted();
        }

        public void OnRWCompleted()
        {
            int cash;

            if (Mathf.Abs(currentArrowAngle) < 8f)
            {
                cash = cashCount * 5;
                eventSystem.OnCashAnimRequested.Invoke((5, true));
            }
            else if (Mathf.Abs(currentArrowAngle) < 20f)
            {
                cash = cashCount * 4;
                eventSystem.OnCashAnimRequested.Invoke((4, true));
            }
            else if (Mathf.Abs(currentArrowAngle) < 30f)
            {
                cash = cashCount * 3;
                eventSystem.OnCashAnimRequested.Invoke((3, true));
            }
            else
            {
                cash = cashCount * 2;
                eventSystem.OnCashAnimRequested.Invoke((2, true));
            }

            currencyManager.SetEarnedCurrency(cash);
            pressed = true;
        }
        public void OnRWFailed()
        {
            doArrowAnim = true;
        }

        private void FixedUpdate()
        {
            if (doArrowAnim)
            {
                float sinVal = Mathf.Sin(Time.time * 2.5f);
                currentArrowAngle = sinVal * 35;

                arrowPanel.rotation = Quaternion.Euler(currentArrowAngle * Vector3.forward);

                if (Mathf.Abs(currentArrowAngle) < 8f)
                {
                    multiplyText.text = (cashCount * 5).ToString();
                    claimText.text = "GET" + " " + "5X";
                }
                else if (Mathf.Abs(currentArrowAngle) < 20f)
                {
                    multiplyText.text = (cashCount * 4).ToString();
                    claimText.text = "GET" + " " + "4X";
                }
                else if (Mathf.Abs(currentArrowAngle) < 30f)
                {
                    multiplyText.text = (cashCount * 3).ToString();
                    claimText.text = "GET" + " " + "3X";
                }
                else
                {
                    multiplyText.text = (cashCount * 2).ToString();
                    claimText.text = "GET" + " " + "2X";
                }
            }
        }

    }
}