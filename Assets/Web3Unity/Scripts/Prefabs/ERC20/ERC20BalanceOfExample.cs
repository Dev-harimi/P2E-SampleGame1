using System.Collections;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ERC20BalanceOfExample : MonoBehaviour
{
    TextMeshProUGUI NFT_count;

    async void Start()
    {
        string chain = "ethereum";
        string network = "rinkeby";
        string contract = "0x88B48F654c30e99bc2e4A1559b4Dcf1aD93FA656";
        string account = PlayerPrefs.GetString("Account");

        BigInteger balanceOf = await ERC20.BalanceOf(chain, network, contract, account);
        print(balanceOf);

        if (account != "")
        {
            if (balanceOf > 0)
            {
                NFT_count = GetComponent<TextMeshProUGUI>();
                NFT_count.text = "NFT count:" + balanceOf.ToString();
            }
        }
    }
}

