using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Lofelt.NiceVibrations;

namespace HolagoGames
{
    public class CashAnimationController : MonoBehaviour
    {
        private const int minMultiplier = 1;
        private const int maxMultiplier = 5;

        //60 tane olmalı
        [SerializeField]
        private List<RectTransform> singleCashes;
        [SerializeField]
        private RectTransform rewardedMoneyStartPoint;
        [SerializeField]
        private RectTransform normalMoneyStartPoint;
        [SerializeField]
        private RectTransform moneyIcon;

        private EventSystem eventSystem;

        private void Awake()
        {
            eventSystem = Holago.SystemContainer.EventSystem;
        }
        private void OnEnable()
        {
            eventSystem.OnCashAnimRequested.Register(StartAnim);
            //eventSystem.OnShopPanelClosed.Register(ShopPanelClosed);
        }

        private void OnDisable()
        {
            eventSystem.OnCashAnimRequested.UnRegister(StartAnim);
            //eventSystem.OnShopPanelClosed.UnRegister(ShopPanelClosed);
        }

        public void StartAnim((int multiplier, bool levelEnd) args)
        {
            StartCoroutine(CashAnim(args.multiplier, args.levelEnd));
        }
        private IEnumerator CashAnim(int multiplier, bool levelEnd)
        {
            var wait = new WaitForFixedUpdate();

            Holago.SystemContainer.EventSystem.PlayCoinsSFX.Invoke();

            int cashCount = singleCashes.Count / ((maxMultiplier + minMultiplier) - Mathf.Clamp(multiplier, minMultiplier, maxMultiplier));
            Vector3 targetPos;

            if (multiplier == 1)
                targetPos = normalMoneyStartPoint.position;
            else
                targetPos = rewardedMoneyStartPoint.position;

            for (int i = 0; i < cashCount; i++)
            {
                RectTransform singleCash = singleCashes[i];
                singleCash.position = targetPos;
                singleCash.rotation = Quaternion.Euler(Random.Range(-179.5f, 179.5f) * Vector3.forward);
                singleCash.gameObject.SetActive(true);

                const float randomSeed = 300f;
                float randomX = Random.Range(-randomSeed, randomSeed);
                float randomY = Random.Range(-randomSeed, randomSeed);
                singleCash.DOMove(targetPos + Vector3.right * randomX + Vector3.up * randomY, .35f).SetId(singleCash.transform.GetInstanceID());

                if (i % 3 == 0)
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
                yield return wait;
                //yield return wait;
            }

            yield return new WaitForSeconds(.2f);

            Invoke(nameof(UIHaptic), .15f);

            RectTransform targetMoneyRect = moneyIcon;
            for (int i = 0; i < cashCount; i++)
            {
                RectTransform singleCash = singleCashes[i];

                DOTween.Kill(singleCash.transform.GetInstanceID());

                var seq = DOTween.Sequence();
                seq.Append(singleCash.DOMove(targetMoneyRect.position, 0.8f).SetEase(Ease.InCubic));
                seq.OnComplete(() =>
                {
                    singleCash.gameObject.SetActive(false);
                    // haptic vs
                });

                // if (i % 3 == 0)
                //     HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
                if (i % multiplier == 0)
                    yield return wait;
                //yield return wait;
            }
            Holago.SystemContainer.EventSystem.OnStartCashCounter.Invoke();


            yield return new WaitForSeconds(1.2f);

            Holago.SystemContainer.EventSystem.OnCashAnimCompleted.Invoke(levelEnd);
        }

        private void UIHaptic() => HapticPatterns.PlayConstant(0.4f, 0.05f, 0.7f);
        private void ShopPanelClosed()
        {
            for (int i = 0; i < singleCashes.Count; i++)
            {
                RectTransform singleCash = singleCashes[i];
                DOTween.Kill(singleCash.transform.GetInstanceID());
                singleCash.gameObject.SetActive(false);
            }
        }
    }
}