using ElephantSDK;
using HolagoGames;
using Lofelt.NiceVibrations;
using System.Collections.Generic;
using UnityEngine;

public class LevelChecker : MonoBehaviour
{
    [SerializeField] List<GameObject> cars;
    [SerializeField] GameObject carsParent;

    private void Awake()
    {
        cars = new List<GameObject>();
        AddCars();
    }

    private void AddCars()
    {
        for(int i = 0; i < carsParent.transform.childCount; i++) 
        {
            cars.Add(carsParent.transform.GetChild(i).gameObject);
        }
    }

    public void RemoveCar(GameObject car)
    {
        if (cars.Remove(car))
        {
            Destroy(car);

            if (cars.Count <= 0)
            {
                LevelCompleted();
            }
        }
    }

    private void LevelCompleted()
    {
        Holago.SystemContainer.EventSystem.OnWinLevel.Invoke();
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
}