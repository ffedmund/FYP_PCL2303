using System;
using FYP;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemInteractManager : PlayerSetupBehaviour
{

    public Button useButton;
    public Button throwButton;
    public Button dismantleButton;
    public Button shortcutButton;
    public GameObject choiceWindow;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public GameObject itemObject;
    public Color normalItemColor;
    public Color shortcutItemColor;

    public static Action<ItemInteract, Vector2> OnItemInteract;
    public static Action OnItemNotInteract;

    static ItemInteract currentInteractingItem;


    PlayerManager playerManager;
    PlayerInventory playerInventory;
    PlayerQuickSlots playerQuickSlots;
    UIController uiController;

    // Start is called before the first frame update
    void Start()
    {
        playerInventory = FindAnyObjectByType<PlayerInventory>();
        playerQuickSlots = FindAnyObjectByType<PlayerQuickSlots>();
        uiController = FindAnyObjectByType<UIController>();
        OnItemInteract += ItemInteract;
        OnItemNotInteract += ItemNotInteract;
        useButton.onClick.AddListener(() => { UseItem(); });
        throwButton.onClick.AddListener(() => { ThrowItem(); });
        dismantleButton.onClick.AddListener(() => { DismantleItem(); });
        shortcutButton.onClick.AddListener(() => { EditShortcutItem(); });
        ItemNotInteract();
    }

    public void UseItem()
    {
        Item item = GetItemFromSlot();
        if (item is MaterialItem)
        {
            MaterialItem materialItem = (MaterialItem)item;
            UseItemEffect(materialItem);
            DestroyItem(item);
            uiController.UpdateUI();
            ItemNotInteract();
        }
        else
        {
            uiController.EnableAllSelectedSlots();
            currentInteractingItem.GetComponentInChildren<Button>().onClick.Invoke();
        }
    }

    void DestroyItem(Item item)
    {
        if (item is WeaponItem)
        {
            playerInventory.weaponsInventory.Remove((WeaponItem)item);
        }
        if (item is HelmetEquipment)
        {
            playerInventory.helmetEquipmentsInventory.Remove((HelmetEquipment)item);
        }
        if (item is ArmEquipment)
        {
            playerInventory.armEquipmentsInventory.Remove((ArmEquipment)item);
        }
        if (item is TorsoEquipment)
        {
            playerInventory.torsoEquipmentsInventory.Remove((TorsoEquipment)item);
        }
        if (item is LegEquipment)
        {
            playerInventory.legEquipmentsInventory.Remove((LegEquipment)item);
        }
        if (item is SpellItem)
        {
            playerInventory.spellInventory.Remove((SpellItem)item);
        }
        if (item is ArtifactItem)
        {
            playerInventory.artifactsInventory.Remove((ArtifactItem)item);
            playerInventory.GetComponent<ArtifactAbilityController>().UpdateArtifacts();
        }
        if (item is MaterialItem)
        {
            playerInventory.RemoveItem(item);
            foreach (Quest quest in playerInventory.GetComponent<PlayerManager>().playerData.quests)
            {
                if (quest.goalChecker.ItemLosted(item.itemName))
                {
                    break;
                }
            };

            if(!playerInventory.materialsInventory.ContainsKey(item.itemName)){
                playerQuickSlots.RemoveQuickSlotItem(item);
            }
        }
        uiController.UpdateUI();
        ItemNotInteract();
    }

    void ThrowItem()
    {
        Item item = GetItemFromSlot();
        DestroyItem(item);
        CreateThrowItem(item);
    }

    void DismantleItem()
    {   
        Item item = GetItemFromSlot();
        DestroyItem(item);
        if (item is WeaponItem)
        {
            playerInventory.AddItem(playerInventory.resources, ((WeaponItem)item).level * 10 + (int)((WeaponItem)item).rarity * 20);
        }
        else if (item is EquipmentItem)
        {
            playerInventory.AddItem(playerInventory.resources, ((EquipmentItem)item).level * 10 + (int)((EquipmentItem)item).rarity * 20);
        }
    }
    
    void EditShortcutItem()
    {
        Item item = GetItemFromSlot();
        if (item is MaterialItem)
        {
            bool isShortcutItem = playerQuickSlots.HandleItemToSlots(item);
            ColorBlock colorBlock = shortcutButton.GetComponent<Button>().colors;
            colorBlock.normalColor = isShortcutItem ? shortcutItemColor : normalItemColor;
            colorBlock.selectedColor = isShortcutItem ? shortcutItemColor : normalItemColor;
            shortcutButton.GetComponent<Button>().colors = colorBlock;
        }
    }

    public static void UseItemEffect(MaterialItem item)
    {
        PlayerManager playerManager = GameManager.instance.localPlayerManager;
        PlayerStats playerStats = playerManager.playerStats;;
        AnimatorHandler animatorHandler = playerManager.GetComponentInChildren<AnimatorHandler>();
        // playerManager = FindAnyObjectByType<PlayerManager>();
        if(playerManager.isInteracting)
        {
            return;
        }
        switch (item.itemEffect.itemEffectType)
        {
            case ItemEffectType.Heal:
                if (playerStats.currentHealth.Value + item.itemEffect.amount <= playerStats.maxHealth)
                {
                    playerStats.Heal(item.itemEffect.amount);
                }
                else
                {
                    playerStats.Heal(playerStats.maxHealth - playerStats.currentHealth.Value);
                }
                animatorHandler.PlayTargetAnimation("Eat", true);
                break;
            case ItemEffectType.Buff:
                Debug.Log($"Add Buff[{item.itemEffect.buff}] duration:{item.itemEffect.duration}");
                FindAnyObjectByType<UIController>().buffSlotsUIController.CreateBuffIcon(item.itemEffect.buff, item.itemEffect.duration);
                BuffManager.SetBuff(item.itemEffect.buff, playerStats, item.itemEffect.duration);
                animatorHandler.PlayTargetAnimation("Eat", true);
                break;
            case ItemEffectType.Projectile:
                Vector3 playerPosition = new Vector3(playerStats.transform.position.x, playerStats.transform.GetComponentInChildren<Renderer>().bounds.size.y, playerStats.transform.position.z);
                GameObject projectile = Instantiate(item.modelPrefab, playerPosition + playerStats.transform.forward, Quaternion.identity);
                projectile.transform.AddComponent<Projectile>().Throw(playerStats.transform.forward, item.itemEffect.amount);
                break;
            default:
                break;
        }
    }

    void CreateThrowItem(Item item)
    {
        if(NetworkManager.Singleton.IsConnectedClient)
        {
            NetworkObjectManager.Singleton.CreateNetworkObjectHandler(item, playerInventory.transform.position, showParticle: true);
            if(item.itemName == "crystal_ball")
            {
                playerManager.minimapIconController.ResetID();
            }
        }
        else
        {
            if (item is MaterialItem)
            {
                GameObject throwedItemObject = Instantiate(itemObject, playerInventory.transform.position, Quaternion.identity);
                Destroy(throwedItemObject.transform.GetComponent<WeaponPickUp>());
                throwedItemObject.AddComponent<MaterialPickUp>().material = (MaterialItem)item;
                throwedItemObject.GetComponent<MaterialPickUp>().interactableText = item.itemName;
            }
            else if (item is WeaponItem)
            {
                GameObject throwedItemObject = Instantiate(itemObject, playerInventory.transform.position, Quaternion.identity);
                throwedItemObject.transform.GetComponent<WeaponPickUp>().weapon = (WeaponItem)item;
                throwedItemObject.GetComponent<WeaponPickUp>().interactableText = item.itemName;
            }
        }
    }

    Item GetItemFromSlot()
    {
        Item item = currentInteractingItem.GetComponent<InventorySlot>().GetItem();
        Debug.Log("Current Iteract Item: " + item);
        return item;
    }

    void ShowItemDetail()
    {
        Item item = GetItemFromSlot();
        title.SetText(item.name);
        if (item is ArtifactItem)
        {
            string descriptionText = "";
            foreach (var stats in ((ArtifactItem)item).abilities)
            {
                descriptionText += $"{stats.statType}: {(stats.value > 0 ? "+" : "")}{stats.value}{(stats.valueType == ArtifactItem.ValueType.Percentage ? "%" : "")}\n";
            }
            foreach (var series in ((ArtifactItem)item).series)
            {
                descriptionText += $"<color=\"yellow\">{series.name}</color>\n";
            }
            description.SetText(descriptionText);
        }
        if (item is WeaponItem)
        {
            string descriptionText = "";
            descriptionText += "Level: " + ((WeaponItem)item).level + "\n";
            if (((WeaponItem)item).isMeleeWeapon)
            {
                descriptionText += "Light Attack: " + ((WeaponItem)item).lightAttackDamageModifier * 25 + "\n";
                descriptionText += "Heavy Attack: " + ((WeaponItem)item).heavyAttackDamageModifier * 25;
            }

            if (((WeaponItem)item).isSpellCaster)
            {
                descriptionText += "Spell Caster\n";
            }
            else if (((WeaponItem)item).isFaithCaster)
            {
                descriptionText += "Faith Caster\n";
            }
            else if (((WeaponItem)item).isPyroCaster)
            {
                descriptionText += "Pyro Caster\n";
            }

            description.SetText(descriptionText);
        }
        if (item is EquipmentItem)
        {
            string descriptionText = "";
            descriptionText += "Level: " + ((EquipmentItem)item).level + "\n";
            descriptionText += "Defense: " + ((EquipmentItem)item).physicalDefense + "\n";
            description.SetText(descriptionText);
        }
        if (item is MaterialItem)
        {
            description.SetText(item.itemDescription + $"\nEffect:\n{((MaterialItem)item).itemEffect.itemEffectType}");
        }
        if (item is SpellItem)
        {
            string descriptionText = "";
            descriptionText += ((SpellItem)item).isFaithSpell ? "Faith Spell\n" : ((SpellItem)item).isMagicSpell ? "Magic Spell\n" : "Pyro Spell\n";
            descriptionText += "Mana Cost: " + ((SpellItem)item).manaCost + "\n";
            if (item is ProjectileSpell)
            {
                descriptionText += "Base Damage: " + ((ProjectileSpell)item).baseDamage + "\n";
            }
            else if (item is HealingSpell)
            {
                descriptionText += "Healing Amount: " + ((HealingSpell)item).healAmount + "\n";
            }
            description.SetText(descriptionText);
        }
    }

    void ItemInteract(ItemInteract interactingItem, Vector2 position)
    {
        if (currentInteractingItem)
        {
            currentInteractingItem.isInteracting = false;
        }
        transform.position = position;
        currentInteractingItem = interactingItem;
        if (GetItemFromSlot() == null || uiController.IsEquipmentSlotsEnabled())
        {
            ItemNotInteract();
        }
        else
        {
            ShowItemDetail();
            ColorBlock colorBlock = shortcutButton.GetComponent<Button>().colors;
            colorBlock.normalColor = playerQuickSlots.IndexOfQuickSlots(GetItemFromSlot().itemName) != -1 ? shortcutItemColor : normalItemColor;
            shortcutButton.GetComponent<Button>().colors = colorBlock;
            Item item = GetItemFromSlot();
            shortcutButton.gameObject.SetActive(item is MaterialItem && ((MaterialItem)item).itemEffect.itemEffectType != ItemEffectType.None);
            useButton.GetComponentInChildren<TextMeshProUGUI>().SetText((item is MaterialItem) ? "Use" : "Equip");
            throwButton.GetComponentInChildren<TextMeshProUGUI>().SetText((item is MaterialItem) ? "Throw" : "Throw");
            if (item is MaterialItem)
            {
                dismantleButton.gameObject.SetActive(false);
            }
            else
            {
                dismantleButton.gameObject.SetActive(true);
            }
            if (item is MaterialItem && ((MaterialItem)item).itemEffect.itemEffectType == ItemEffectType.None || item is ArtifactItem)
            {
                useButton.gameObject.SetActive(false);
            }
            else
            {
                useButton.gameObject.SetActive(true);
            }
            choiceWindow.SetActive(true);
        }
    }

    public void ItemNotInteract()
    {
        choiceWindow.SetActive(false);
        if (currentInteractingItem)
        {
            currentInteractingItem.isInteracting = false;
            currentInteractingItem = null;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        OnItemInteract = null;
        OnItemNotInteract = null;
    }

    public override void Setup(PlayerManager playerManager)
    {
        this.playerManager = playerManager;
        playerInventory = playerManager.GetComponent<PlayerInventory>();
        playerQuickSlots = playerManager.GetComponent<PlayerQuickSlots>();
    }
}
