using System.Collections.Generic;

public interface IRepository<T>
{
    void Add(T item);
    T GetAccountByNumber(string accountNumber);
    T GetUserByUserName(string userName);
    void SaveData();
   List<T> LoadData();
    List<T> GetAll();
}
