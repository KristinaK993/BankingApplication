using System.Collections.Generic;

public interface IRepository<T>
{
    void Add(T item);
    T GetAccountByNumber(string accountNumber);
    void SaveData();
    void LoadData();
    List<T> GetAll();
}
