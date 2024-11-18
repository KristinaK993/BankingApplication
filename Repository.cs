using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BankingApplication;
using Newtonsoft.Json;

public class Repository<T> : IRepository<T>
{
    private List<T> items = new ();
    private readonly string filePath;

    public Repository(string filePath)
    {
        this.filePath = filePath;
        LoadData();
    }
    public void Add(T item)
    {
        items.Add(item);
        SaveData();
    }
    public T GetAccountByNumber(string accountNumber)
    {
        return items.OfType<BankAccount>()
            .FirstOrDefault(acc => acc.AccountNumber == accountNumber) as T;
    }
    public void SaveData()
    {
        try
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(items, Formatting.Indented));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }
    public void LoadData()
    {
        try
        {
            if (File.Exists(filePath))
            {
                items = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(filePath)) ?? new List<T>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }
    public List<T> GetAll()
    {
        return items;
    }
}
