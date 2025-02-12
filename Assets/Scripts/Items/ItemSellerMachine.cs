using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;


public class ItemSellerMachine : MonoBehaviour
{
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private GameObject greenLight;
    [SerializeField] private GameObject redLight;
    [SerializeField] private Animation animationComponent;

    private List<ItemCore> itemsInTrigger = new List<ItemCore>();
    private ItemCore currentItem;
    private bool isProcessing;
    private int storedMoney;

    [SerializeField] private DamageToActivate activateButton;
    [SerializeField] private DamageToActivate collectCoinsButton;
    private void Awake()
    {
        activateButton.OnActivateEvent += TryActivating;
        collectCoinsButton.OnActivateEvent += CollectMoney;

    }

    private void OnDestroy()
    {
        activateButton.OnActivateEvent -= TryActivating;
        collectCoinsButton.OnActivateEvent -= CollectMoney;
    }
    private void Start()
    {
        UpdateMoneyDisplay();
    }

    private void CleanNullItems()
    {
        // Remove all null items using LINQ Where filter
        itemsInTrigger = itemsInTrigger.Where(item => item != null).ToList();
    }

    private void OnTriggerEnter(Collider other)
    {
        CleanNullItems();
        ItemCore item = other.GetComponentInParent<ItemCore>();
        if (item != null && !itemsInTrigger.Contains(item))
        {
            itemsInTrigger.Add(item);
            UpdateDisplay();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CleanNullItems();
        ItemCore item = other.GetComponentInParent<ItemCore>();
        if (item != null && itemsInTrigger.Contains(item))
        {
            itemsInTrigger.Remove(item);
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        bool isValidState = itemsInTrigger.Count == 1;
        greenLight.SetActive(isValidState);
        redLight.SetActive(!isValidState);
        priceText.text = isValidState ? itemsInTrigger[0].price.ToString() : "";
    }

    private void UpdateMoneyDisplay()
    {
        moneyText.text = storedMoney.ToString();
    }

    public void TryActivating(PlayerCore player)
    {
        CleanNullItems();
        if (itemsInTrigger.Count != 1 || isProcessing) return;

        currentItem = itemsInTrigger[0];
        isProcessing = true;
        animationComponent.Play();
    }

    public void TrySelling()
    {CleanNullItems();
        if (currentItem != null)
        {
            storedMoney += currentItem.price;
            Destroy(currentItem.gameObject);
            CleanNullItems();

            UpdateMoneyDisplay();
            UpdateDisplay();
        }

        currentItem = null;
        isProcessing = false;
    }

    public void CollectMoney(PlayerCore player)
    {
        if (storedMoney > 0)
        {
            player.GiveCoins(storedMoney);
            storedMoney = 0;
            UpdateMoneyDisplay();
        }
    }
}
