using System;
using System.Collections.Generic;
using System.IO;
using BankingApplication;
using Newtonsoft.Json;

public class Repository<T> where T : class
{
    private List<T> items;
    private string filePath;

    // Konstruktor som accepterar en filväg
    public Repository(string filePath)
    {
        this.filePath = filePath;
        items = LoadData();
    }

    // Ladda data från JSON
    private List<T> LoadData()
    {
        if (File.Exists(filePath))
        {
            var jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<T>>(jsonData) ?? new List<T>();
        }
        return new List<T>();
    }

    // Spara data till JSON
    public void SaveData()
    {
        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(filePath, jsonData);
    }

    // Hämta ett objekt baserat på ett AccountNumber
    public T GetAccountByNumber(string accountNumber)
    {
        return items.OfType<BankAccount>().FirstOrDefault(acc => acc.AccountNumber == accountNumber) as T;
    }

    // Lägg till ett nytt objekt i listan
    public void AddItem(T item)
    {
        items.Add(item);
        SaveData();
    }

    // Ta bort ett objekt
    public void RemoveItem(T item)
    {
        items.Remove(item);
        SaveData();
    }

    // Hämta alla objekt
    public List<T> GetAll()
    {
        return items;
    }
}

