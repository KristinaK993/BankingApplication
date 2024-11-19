using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class Repository<T> : IRepository<T> where T : class
{
    private List<T> items;
    private readonly string filePath;

    public Repository(string filePath)
    {
        this.filePath = filePath;
        items = LoadData();
    }

    // Ladda data från JSON
    public List<T> LoadData()
    {
        if (File.Exists(filePath))
        {
            Console.WriteLine($"Loading data from {filePath}");
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

    // Lägg till ett nytt objekt
    public void Add(T item)
    {
        items.Add(item);
        SaveData();
    }

    // Hämta alla objekt
    public List<T> GetAll()
    {
        return items;
    }

    // Hämta ett objekt baserat på AccountNumber
    public T GetAccountByNumber(string accountNumber)
    {
        if (typeof(T) == typeof(BankAccount))
        {
            return items.OfType<BankAccount>()
                        .FirstOrDefault(acc => acc.AccountNumber == accountNumber) as T;
        }

        throw new InvalidOperationException($"The type {typeof(T).Name} does not support this operation.");
    }

    // Hämta en användare baserat på UserName (endast om T är BankUser)
    public T GetUserByUserName(string userName)
    {
        if (typeof(T) == typeof(BankUser))
        {
            return items.OfType<BankUser>()
                        .FirstOrDefault(user => user.UserName == userName) as T;
        }

        throw new InvalidOperationException($"The type {typeof(T).Name} does not support this operation.");
    }
}


