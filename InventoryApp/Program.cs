using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventorySystemApp
{
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new List<T>();
        private string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(_log);
        }

        public void SaveToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                using (var writer = new StreamWriter(_filePath))
                {
                    writer.Write(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine("No inventory file found.");
                    return;
                }

                using (var reader = new StreamReader(_filePath))
                {
                    string json = reader.ReadToEnd();
                    var data = JsonSerializer.Deserialize<List<T>>(json);
                    if (data != null)
                        _log = data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }
    }

    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Hammer", 10, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Screwdriver", 15, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Pliers", 8, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Wrench", 5, DateTime.Now));
            _logger.Add(new InventoryItem(5, "Drill", 3, DateTime.Now));
        }

        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            var items = _logger.GetAll();
            if (items.Count == 0)
            {
                Console.WriteLine("No items found.");
                return;
            }

            foreach (var item in items)
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded:d}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "inventory.json");

            var app = new InventoryApp(filePath);

            app.SeedSampleData();
            app.SaveData();

            app = new InventoryApp(filePath);
            app.LoadData();
            app.PrintAllItems();

            Console.WriteLine($"\nInventory file saved at: {filePath}");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
