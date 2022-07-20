using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MetamaskAccount : MonoBehaviour
{
    TextMeshProUGUI myAccount;

    void Start()
    {
        myAccount = GetComponent<TextMeshProUGUI>();
        myAccount.text = PlayerPrefs.GetString("Account");
    }

}
