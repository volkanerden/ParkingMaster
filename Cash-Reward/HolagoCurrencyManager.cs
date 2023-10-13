using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Lofelt.NiceVibrations;

namespace HolagoGames
{
    public class HolagoCurrencyManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text[] currencyTexts;

        private CurrencySystem currencySystem;
        private float preCashCount;
        private float timer;

        private int earnedCurrencyInLevel;
        private bool setCashCounter = true;

        private void OnEnable()
        {
            setCashCounter = true;
            earnedCurrencyInLevel = 0;

            currencySystem = Holago.SystemContainer.CurrencySystem;
            Holago.SystemContainer.EventSystem.OnLevelLoaded.Register(OnLevelLoaded);
            Holago.SystemContainer.EventSystem.OnWinLevel.Register(OnWinLevel);

            preCashCount = currencySystem.CashCount;
            SetCurrencyTexts(preCashCount.ToString("0"));
            timer = Time.time;

            Holago.SystemContainer.EventSystem.OnStartCashCounter.Register(StartCount);
        }

        private void StartCount()
        {
            setCashCounter = true;
        }

        private void OnDisable()
        {
            Holago.SystemContainer.EventSystem.OnLevelLoaded.UnRegister(OnLevelLoaded);
            Holago.SystemContainer.EventSystem.OnWinLevel.UnRegister(OnWinLevel);
            Holago.SystemContainer.EventSystem.OnStartCashCounter.UnRegister(StartCount);
        }

        private void OnLevelLoaded()
        {
            setCashCounter = true;
            timer = Time.time;
            earnedCurrencyInLevel = 0;
        }
        private void OnWinLevel()
        {
            //setCashCounter = false;

            float finishTime = Time.time - timer;
            float clampedFinishTime = Mathf.Clamp(finishTime, 10, 50);

            earnedCurrencyInLevel = 20;
        }

        public int GetEarnedCash()
        {
            return earnedCurrencyInLevel;
        }
        public void SetEarnedCurrency(int cash)
        {
            earnedCurrencyInLevel = cash;
            currencySystem.AddCurrency(Mathf.FloorToInt(earnedCurrencyInLevel), CurrencyType.Cash);
            earnedCurrencyInLevel = 0;
        }

        private void SetCurrencyTexts(string cashText)
        {
            foreach (TMP_Text currencyText in currencyTexts)
            {
                currencyText.text = cashText;
            }
        }

        private void FixedUpdate()
        {
            if (setCashCounter == true)
            {
                float currentCashCount = currencySystem.CashCount;

                if (preCashCount != currentCashCount)
                {
                    preCashCount = Mathf.Lerp(preCashCount, currentCashCount, .1f);
                    SetCurrencyTexts(preCashCount.ToString("0"));

                    if (Mathf.Abs(currentCashCount - preCashCount) < 1)
                    {
                        preCashCount = currentCashCount;
                    }
                }

                if (Mathf.Abs(currentCashCount - preCashCount) < 1)
                {
                    preCashCount = currentCashCount;
                    SetCurrencyTexts(currentCashCount.ToString("0"));
                    setCashCounter = false;
                }
            }
        }
    }
}