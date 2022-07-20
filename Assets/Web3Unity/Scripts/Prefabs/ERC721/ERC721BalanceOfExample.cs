using System.Collections;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ERC721BalanceOfExample : MonoBehaviour
{
    TextMeshProUGUI NFT_count;

    async void Start()
    {
        string chain = "ethereum";
        string network = "mainnet";
        string contract = "0xce591197A5e9ed499d1658512d690682e023c303";
        string account = PlayerPrefs.GetString("Account");

        if (account != "")
        {
            int balance = await ERC721.BalanceOf(chain, network, contract, account);
            print(balance);

            if (balance > 0)
            {
                NFT_count = GetComponent<TextMeshProUGUI>();
                NFT_count.text = "NFT count:" + balance.ToString();
            }
        }
    }
}
