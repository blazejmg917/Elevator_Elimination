using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PersonTooltip : MonoBehaviour
{
    [SerializeField, Tooltip("the text field to fill")] private TMP_Text textField;

    public void DisplayPerson(Person p)
    {
        textField.text = p.GetId() + ": " + p.GetDescription();
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        HideTooltip();
    }
}
