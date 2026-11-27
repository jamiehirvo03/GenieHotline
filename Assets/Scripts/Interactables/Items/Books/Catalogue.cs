using Ink.Parsed;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Catalogue : MonoBehaviour
{
    //Newspaper

    //Lucky Star Stamp

    //Mystical Lamp

    // Noodlebox

    [System.Serializable]
    public class Items
    {
        public string name;
        public string description;
        public Sprite image;

        public int price;
        public int maxStock;

        public GameObject itemTile;

        [HideInInspector] public int numberInStock;
        [HideInInspector] public bool purchasedToday;
    }
    [Header("Add Items")] public List<Items> items = new List<Items>();

    private Color defaultColour;
    [SerializeField] private Color flashColour;

    private void Start()
    {
        SetTileDetails();
    }

    private void SetTileDetails()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Items currentItemTile = items[i];

            currentItemTile.itemTile.GetComponent<CatalogueTile>().itemImage.GetComponent<Image>().sprite = currentItemTile.image;

            currentItemTile.itemTile.GetComponent<CatalogueTile>().itemName.text = currentItemTile.name;

            currentItemTile.itemTile.GetComponent<CatalogueTile>().itemDescription.text = currentItemTile.description;

            currentItemTile.itemTile.GetComponent<CatalogueTile>().itemStock.text = $"{currentItemTile.numberInStock}/{currentItemTile.maxStock}";

            currentItemTile.itemTile.GetComponent<CatalogueTile>().itemPrice.text = $"{currentItemTile.price} GB";

        }
    }

    public void SelectItem(Items selectedItem)
    {
        if (GameManager.GetInstance().geneBucksBalance > selectedItem.price)
        {
            PurchaseItem(selectedItem);
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(DoFlash(selectedItem.itemTile.GetComponent<CatalogueTile>().itemImage));
        }
    }

    private IEnumerator DoFlash(GameObject selectedTile)
    {
        defaultColour = selectedTile.GetComponent<Image>().color;

        selectedTile.GetComponent<Image>().color = flashColour;

        yield return new WaitForSeconds(0.1f);

        selectedTile.GetComponent<Image>().color = defaultColour;
    }

    private void PurchaseItem(Items purchasedItem)
    {
        purchasedItem.numberInStock--;

        GameManager.GetInstance().geneBucksBalance -= purchasedItem.price;

        if (purchasedItem.numberInStock <= 0)
        {
            //purchasedItem.tileImage
        }
    }
}
