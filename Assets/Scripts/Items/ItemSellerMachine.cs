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
    [SerializeField] private BoxCollider triggerCollider;   

    private List<Collider> currentColliders = new List<Collider>();
    private List<ItemCore> itemsInTrigger = new List<ItemCore>();
    private ItemCore currentItem;
    private bool isProcessing;
    private int storedMoney;

    [SerializeField] private DamageToActivate activateButton;
    [SerializeField] private DamageToActivate collectCoinsButton;
    private void Awake()
    {
        UpdateMoneyDisplay();
        TryGetComponent(out triggerCollider);

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

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        //Clear the colliders at the start of each FixedUpdate
       currentColliders.Clear();
    }


    private void LateUpdate()
    {
        CleanupDestroyedItems();
        UpdateTriggerState();
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) return;
        currentColliders.Add(other);
    }

    private void CleanupDestroyedItems()
    {
        // Remove colliders from destroyed or disabled GameObjects
       // currentColliders.RemoveWhere(c => c == null || !c.enabled || c.gameObject == null);

        // Update itemsInTrigger list
        itemsInTrigger = currentColliders
            .Select(c => c.GetComponentInParent<ItemCore>())
            .Where(item => item != null)
            .ToList();
    }

    private void UpdateTriggerState()
    {
        bool isValidState = itemsInTrigger.Count == 1;
        greenLight.SetActive(isValidState);
        redLight.SetActive(!isValidState);
        priceText.text = isValidState ? itemsInTrigger[0].price.ToString() : "";

       // if (!isValidState) StopProcessing 
        // Clear stored colliders each frame
      
    }

    private void StopProcessing()
    {
        animationComponent.Stop();
        animationComponent.Rewind();

        isProcessing = false;
    }

    private void UpdateDisplay()
    {
        bool isValidState = itemsInTrigger.Count == 1;
        Debug.Log("Valid State? " + isValidState);
        Debug.Log("itemsInTrigger? " + itemsInTrigger);

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
      //  CleanNullItems();
        if (itemsInTrigger.Count != 1 || isProcessing) return;

        currentItem = itemsInTrigger[0];
        isProcessing = true;
        animationComponent.Play();

    }

    public void TrySelling()
    {
        //CleanNullItems();
        if (currentItem != null)
        {
            storedMoney += currentItem.price;
            Destroy(currentItem.gameObject);
            //CleanNullItems();

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
