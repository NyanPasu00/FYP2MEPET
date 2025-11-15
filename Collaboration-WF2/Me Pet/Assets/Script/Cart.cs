using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class CartItem
{
    public string itemName;
    public int price;
    public Sprite icon;
    public int quantity;

    public CartItem(ItemData data)
    {
        itemName = data.itemName;
        price = data.price;
        icon = data.icon;
        quantity = 1; // default quantity
    }
}
public class Cart : MonoBehaviour
{
    public int cartNum;
    public List<CartItem> items = new List<CartItem>();
    public int totalCost;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addItem(ItemData item)
    {
        // Check if item already exists
        CartItem existing = items.Find(i => i.itemName == item.itemName);
        if (existing != null)
        {
            existing.quantity++;
        }
        else
        {
            items.Add(new CartItem(item));
        }

        calculateTotalCost();

        Debug.Log("Added to cart: " + item.itemName);
    }

    public void removeItem(string itemName)
    {
        items.RemoveAll(i => i.itemName == itemName);
        calculateTotalCost();
    }

    public void updateCart(string itemName, int change)
    {
        CartItem item = items.Find(i => i.itemName == itemName);

        if (item != null)
        {
            item.quantity += change;          // Apply +1 or -1
            item.quantity = Mathf.Max(item.quantity, 0);  // Clamp to 0

            if (item.quantity == 0)
            {
                removeItem(itemName);  // Auto remove if zero
            }
        }

        calculateTotalCost();
    }

    public void PrintCart()
    {
        Debug.Log("----- Cart Contents -----");
        if (items.Count == 0)
        {
            Debug.Log("Cart is empty!");
            return;
        }

        foreach (CartItem item in items)
        {
            Debug.Log($"Item: {item.itemName}, Price: {item.price}, Quantity: {item.quantity}");
        }
        Debug.Log("-------------------------");
    }

    public void calculateTotalCost()
    {
        totalCost = 0;

        foreach (CartItem item in items)
        {
            totalCost += item.price * item.quantity;
        }
    }
}
