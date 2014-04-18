using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Utilities
{
    public interface IIndex
    {
        int Index { get; set; }
    }

    public class IndexedObservableCollection<T> : ObservableCollection<T> where T : IIndex
    {
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            item.Index = Count;
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            foreach (T obj in this.Skip(index))
            {
                obj.Index--;
            }
        }
    }
}
