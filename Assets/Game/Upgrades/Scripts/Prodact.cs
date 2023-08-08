using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Prodact : MonoBehaviour
{
    public int Price = 000;
    // Update is called once per frame
    void Update()
    {
        transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Price.ToString();
        int Level = 1;
        if (gameObject.name == "ShipVelocity")
            Level = GameManager.Instance.ShipSpeedLevel;
        if (gameObject.name == "Quantity")
            Level = GameManager.Instance.QuantityLevel;
        if (gameObject.name == "Quality")
            Level = GameManager.Instance.QualityLevel;
        if (gameObject.name == "SpeedOfConveyor")
            Level = GameManager.Instance.ConvayorSpeedLevel;
        if (gameObject.name == "SpeedOfChecking")
            Level = GameManager.Instance.ScanningSpeedLevel;
        if (gameObject.name == "StackBoxes")
            Level = GameManager.Instance.TableStackLevel;

        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Level: " + Level;
        if (Level == 5)
        {
           transform.GetChild(1).gameObject.SetActive(false);
           transform.GetChild(2).gameObject.SetActive(false);
        } 
    }
}
