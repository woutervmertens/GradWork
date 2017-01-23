using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpawnerPanel : MonoBehaviour
{
    private SpawnerNode _selected;

    private bool _spawnCar;
    private bool _spawnJeep;
    private bool _spawnBike;
    private bool _spawnTruck;

    private int _carPerc;
    private int _jeepPerc;
    private int _bikePerc;
    private int _truckPerc;

    public Toggle CarToggle;
    public Toggle JeepToggle;
    public Toggle BikesToggle;
    public Toggle TruckToggle;

    public Slider CarSlider;
    public Slider JeepSlider;
    public Slider BikeSlider;
    public Slider TruckSlider;

    public InputField RateIn;

    private float _generalSpawnRate;

    void Start()
    {
        _selected = MainManager.Main.LastSelectedGameObject.GetComponent<SpawnerNode>();
        _spawnCar = _selected.SpawnCar;
        _spawnJeep = _selected.SpawnJeep;
        _spawnBike = _selected.SpawnBike;
        _spawnTruck = _selected.SpawnTruck;
        _generalSpawnRate = _selected.GeneralSpawnRate;
        _carPerc = _selected.CarSpawnPerc;
        _jeepPerc = _selected.JeepSpawnPerc;
        _bikePerc = _selected.BikeSpawnPerc;
        _truckPerc = _selected.TruckSpawnPerc;
    }

    public void Show()
    {
        Start();
        CarSlider.value = _carPerc;
        JeepSlider.value = _jeepPerc;
        BikeSlider.value = _bikePerc;
        TruckSlider.value = _truckPerc;

        RateIn.text = _generalSpawnRate.ToString();
    }

    public void Close()
    {
        _selected.SpawnCar = _spawnCar;
        _selected.SpawnJeep = _spawnJeep;
        _selected.SpawnBike = _spawnBike;
        _selected.SpawnTruck = _spawnTruck;
        _selected.GeneralSpawnRate = _generalSpawnRate;
        _selected.CarSpawnPerc = _carPerc;
        _selected.JeepSpawnPerc = _jeepPerc;
        _selected.BikeSpawnPerc = _bikePerc;
        _selected.TruckSpawnPerc = _truckPerc;

        transform.parent.gameObject.SetActive(false);
    }

    public void SpawnCar(bool b)
    {
        b = CarToggle.isOn;
        _spawnCar = b;
        CarSlider.interactable = b;
    }

    public void SpawnJeep(bool b)
    {
        b = JeepToggle.isOn;
        _spawnJeep = b;
        JeepSlider.interactable = b;
    }

    public void SpawnBike(bool b)
    {
        b = BikesToggle.isOn;
        _spawnBike = b;
        BikeSlider.interactable = b;
    }

    public void SpawnTruck(bool b)
    {
        b = TruckToggle.isOn;
        _spawnTruck = b;
        TruckSlider.interactable = b;
    }

    public void CarPerc(int v)
    {
        _carPerc = (int)CarSlider.value;
    }
    public void JeepPerc(int v)
    {
        _jeepPerc = (int)JeepSlider.value;
    }
    public void BikePerc(int v)
    {
        _bikePerc = (int)BikeSlider.value;
    }
    public void TruckPerc(int v)
    {
        _truckPerc = (int)TruckSlider.value;
    }

    public void RateChange(string s)
    {
        s = RateIn.text;
        float b = StripNonFloats(s);
        if (b > 0) _generalSpawnRate = b;
    }

    private float StripNonFloats(string i)
    {
        string resString = "";
        bool hadDot = false;
        for (var j = 0; j < i.Length; j++)
        {
            var sub = i.Substring(j, 1);
            int tempInt;

            if (sub == "." && !hadDot)
            {
                hadDot = true;
                resString += sub;
            }
            else if (int.TryParse(sub, out tempInt))
            {
                resString += sub;
            }
        }
        return float.Parse(resString);
    }
}
