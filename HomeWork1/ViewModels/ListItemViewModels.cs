using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Todos.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace Todos.ViewModels
{
    public class TodoItemViewModel
    {
        public ObservableCollection<TodoItem> allItems = new ObservableCollection<TodoItem>();  

        public ObservableCollection<TodoItem> AllItems { get { return this.allItems; } }            //  数据绑定不支持绑定一个getAttribute方法, 因此只能这个属性的{get;}获取

        public TodoItemViewModel()
        {
            AddTodoItem(new BitmapImage(new Uri("ms-appx:///Assets/tv.jpg")), "Test1", "1111", DateTime.Now);
            AddTodoItem(new BitmapImage(new Uri("ms-appx:///Assets/tv.jpg")), "Test2", "2222", DateTime.Now);

        }

        public void AddTodoItem(BitmapImage image, string title, string description, DateTime date)
        {
            this.allItems.Add(new TodoItem(image, title, description, date));
        }

        public void updateTodoItem(BitmapImage image, string title, string description, DateTime date, int ItemId)
        {
            this.allItems[ItemId].image = image;
            this.allItems[ItemId].title = title;
            this.allItems[ItemId].description = description;
            this.allItems[ItemId].date = date;
        }

        public void removeTodoItem(int ItemId)
        {
            this.allItems.RemoveAt(ItemId);
        }
    }
}
