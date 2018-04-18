using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Todos.Models;
using Windows.UI.Xaml.Media.Imaging;
using Todos.DataBaseModels;
using System.Diagnostics;

namespace Todos.ViewModels
{
    public class TodoItemViewModel
    {
        public ObservableCollection<TodoItem> allItems = new ObservableCollection<TodoItem>();  

        public ObservableCollection<TodoItem> AllItems { get { return this.allItems; } }            //  数据绑定不支持绑定一个getAttribute方法, 因此只能这个属性的{get;}获取

        public int selectId = -1;
        
        public void AddTodoItem(BitmapImage image, string title, string description, DateTimeOffset date)
        {
            TodoItem newItem = new TodoItem(image, title, description, date);
            this.allItems.Add(newItem);

            TodoItemDataBase.insert(newItem.id, newItem.title, newItem.description, newItem.date, newItem.completed);      //  插入数据库项
        }

        public void updateTodoItem(BitmapImage image, string title, string description, DateTimeOffset date)
        {
            this.allItems[selectId].image = image;
            this.allItems[selectId].title = title;
            this.allItems[selectId].description = description;
            this.allItems[selectId].date = date;

            TodoItemDataBase.update(allItems[selectId].id, allItems[selectId].title, allItems[selectId].description, allItems[selectId].date);
        }

        public void removeTodoItem()
        {
            TodoItemDataBase.delete(allItems[selectId].id);
            this.allItems.RemoveAt(selectId);
        }
    }
}
