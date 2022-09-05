using System.Collections;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ERC1155BalanceOfExample : MonoBehaviour
{
    TextMeshProUGUI NFT_count;

    async void Start()
    {
        string chain = "ethereum";
        string network = "rinkeby";
        string contract = "0x88B48F654c30e99bc2e4A1559b4Dcf1aD93FA656";
        string account = PlayerPrefs.GetString("Account");
        string tokenId = "23684926255362940007669660479103283091728564736875332206883054277678082293761";

        BigInteger balanceOf = await ERC1155.BalanceOf(chain, network, contract, account, tokenId);
        print(balanceOf);

        if (balanceOf > 0)
        {
            NFT_count = GetComponent<TextMeshProUGUI>();
            NFT_count.text = "NFT count:" + balanceOf.ToString();
        }
    }
}
