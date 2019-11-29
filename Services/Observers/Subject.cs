using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinancialEntries.Services.Observers
{
    public class Subject
    {
        IList observers = new List<Observer>();

        public void AddObserver(Observer observer)
        {
            observers.Add(observer);
        }

        public async void NotifyObservers()
        {
            await Task.Run(() => {
                foreach (Observer observer in observers)
                {
                    observer.Notify();
                }
            });
        }
    }
}