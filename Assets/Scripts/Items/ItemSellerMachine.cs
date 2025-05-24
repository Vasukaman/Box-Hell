using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;


public class ItemSellerMachine : MonoBehaviour
{
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text itemNameText;
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


    [Header("Sound Effects")]
    [SerializeField] private AudioClip itemInsertedSound;
    [SerializeField] private AudioClip itemRemovedSound;
    [SerializeField] private AudioClip processingStartSound;
    [SerializeField] private AudioClip itemsSoldSound;
    [SerializeField] private AudioClip moneyCollectedSound;

    private bool previousValidState;
    private List<ItemCore> previousItems = new List<ItemCore>();

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
        bool isValidState = itemsInTrigger.Count > 0;

        // Play sound when item is inserted
        if (isValidState && !previousValidState && itemInsertedSound != null)
        {
            AudioSource.PlayClipAtPoint(itemInsertedSound, transform.position);
        }

        // Play sound when item is removed
        if (!isValidState && previousValidState && itemRemovedSound != null)
        {
            AudioSource.PlayClipAtPoint(itemRemovedSound, transform.position);
        }

        greenLight.SetActive(isValidState);
        redLight.SetActive(!isValidState);
        UpdateDisplay();

        previousValidState = isValidState;
    }

    public void TryActivating(PlayerCore player)
    {
        if (itemsInTrigger.Count == 0 || isProcessing) return;

        currentItem = itemsInTrigger[0];
        isProcessing = true;
        animationComponent.Play();

        // Play processing start sound
        if (processingStartSound != null)
        {
            AudioSource.PlayClipAtPoint(processingStartSound, transform.position);
        }
    }

    public void TrySelling()
    {
        foreach (ItemCore item in itemsInTrigger)
        {
            storedMoney += item.price;
            Destroy(item.gameObject);
        }

        // Play items sold sound
        if (itemsSoldSound != null)
        {
            AudioSource.PlayClipAtPoint(itemsSoldSound, transform.position);
        }

        currentItem = null;
        isProcessing = false;
        UpdateMoneyDisplay();
        UpdateDisplay();
    }

    public void CollectMoney(PlayerCore player)
    {
        if (storedMoney > 0)
        {
            player.GiveCoins(storedMoney);
            storedMoney = 0;
            UpdateMoneyDisplay();

            // Play money collected sound
            if (moneyCollectedSound != null)
            {
                AudioSource.PlayClipAtPoint(moneyCollectedSound, transform.position);
            }
        }
    }
    private void StopProcessing()
    {
        animationComponent.Stop();
        animationComponent.Rewind();

        isProcessing = false;
    }

    private void UpdateDisplay()
    {
        bool isValidState = itemsInTrigger.Count > 0;
        Debug.Log("Valid State? " + isValidState);
        Debug.Log("itemsInTrigger? " + itemsInTrigger);
        int priceSum = 0;
        foreach (ItemCore item in itemsInTrigger)
        {
            priceSum += item.price;
        }
       
        greenLight.SetActive(isValidState);
        redLight.SetActive(!isValidState);
        priceText.text = isValidState ? priceSum.ToString() + "$" : "";
        itemNameText.text = isValidState ? itemsInTrigger[0].item.name : "";

    }

    private void UpdateMoneyDisplay()
    {
        moneyText.text = storedMoney.ToString()+"$";
    }


}
