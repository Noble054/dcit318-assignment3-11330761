using System;
using System.Collections.Generic;

namespace WarehouseInventoryApp
{
    public interface IInventoryItem
    {
        string Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    public class ElectronicItem : IInventoryItem
    {
        public string Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(string id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }
    }

    public class GroceryItem : IInventoryItem
    {
        public string Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(string id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }
    }

    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    public class InventoryRepository<T> where T : IInventoryItem
    {
        private Dictionary<string, T> _items = new Dictionary<string, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException("Item already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(string id)
        {
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException("Item not found.");
            return _items[id];
        }

        public void RemoveItem(string id)
        {
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException("Item not found.");
            _items.Remove(id);
        }

        public List<T> GetAllItems()
        {
            return new List<T>(_items.Values);
        }

        public void UpdateQuantity(string id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException("Item not found.");
            _items[id].Quantity = newQuantity;
        }
    }

    public class WareHouseManager
    {
        private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
        private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem("E1", "Laptop", 5, "Dell", 24));
            _electronics.AddItem(new ElectronicItem("E2", "Smartphone", 10, "Samsung", 12));
            _electronics.AddItem(new ElectronicItem("E3", "TV", 3, "LG", 36));

            _groceries.AddItem(new GroceryItem("G1", "Yam", 70, DateTime.Now.AddMonths(12)));
            _groceries.AddItem(new GroceryItem("G2", "soya", 30, DateTime.Now.AddDays(7)));
            _groceries.AddItem(new GroceryItem("G3", "milk", 56, DateTime.Now.AddDays(2)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, string id, int quantity) where T : IInventoryItem
        {
            try
            {
                var currentItem = repo.GetItemById(id);
                repo.UpdateQuantity(id, currentItem.Quantity + quantity);
                Console.WriteLine("Stock updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, string id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine("Item removed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void DemoOperations()
        {
            try
            {
                _electronics.AddItem(new ElectronicItem("E1", "Tablet", 5, "Apple", 12));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                _groceries.RemoveItem("G99");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                _groceries.UpdateQuantity("G1", -5);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ShowInventory()
        {
            Console.WriteLine("\nElectronics:");
            PrintAllItems(_electronics);
            Console.WriteLine("\nGroceries:");
            PrintAllItems(_groceries);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var manager = new WareHouseManager();
            manager.SeedData();
            manager.ShowInventory();
            manager.DemoOperations();
            Console.WriteLine("\nFinal Inventory:");
            manager.ShowInventory();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}