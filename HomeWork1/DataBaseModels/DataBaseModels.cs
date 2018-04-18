using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Models;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using HomeWork1;
using System.Collections.ObjectModel;

namespace Todos.DataBaseModels
{
    public class TodoItemDataBase
    {
       
        public static void createTable()
        {
            SQLiteConnection conn = new SQLiteConnection("TodoItemDataBase.db");
            string sql = @"CREATE TABLE IF NOT EXISTS
                              TodoItemList (Id      VARCHAR( 140 ) PRIMARY KEY NOT NULL,
                                            Title    VARCHAR( 140 ),
                                            Description    VARCHAR( 140 ),
                                            Date VARCHAR( 140 ),
                                            Completed VARCHAR( 140 ),
                                            ImageName VARCHAR( 140 )
                                            );";
            using (var statement = conn.Prepare(sql))
            {
                statement.Step();
            }
        }

        public static void insert(string id, string title, string description, DateTimeOffset date, bool completed, string ImageName)
        {
            SQLiteConnection conn = new SQLiteConnection("TodoItemDataBase.db");
            CultureInfo provider = CultureInfo.InvariantCulture;
            using (var custstmt = conn.Prepare("INSERT INTO TodoItemList (Id, Title, Description, Date, Completed, ImageName) VALUES (?, ?, ?, ?, ?, ?)"))
            {
                custstmt.Bind(1, id);           //  数字对应字段出现的次序
                custstmt.Bind(2, title);
                custstmt.Bind(3, description);
                custstmt.Bind(4, date.ToString(provider));
                custstmt.Bind(5, completed.ToString());
                custstmt.Bind(6, ImageName);
                custstmt.Step();
            }
        }

        public static void update(string id, string title, string description, DateTimeOffset date)   //  当点击update,更新数据库
        {
            SQLiteConnection conn = new SQLiteConnection("TodoItemDataBase.db");
            CultureInfo provider = CultureInfo.InvariantCulture;
            using (var custstmt = conn.Prepare("UPDATE TodoItemList SET Title = ?, Description = ?, Date = ? WHERE Id = ?"))
            {
                custstmt.Bind(1, title);
                custstmt.Bind(2, description);
                custstmt.Bind(3, date.ToString(provider));
                custstmt.Bind(4, id);
                custstmt.Step();
            }
        }

        public static void delete(string id)        //  当点击delete， 更新数据库
        {
            SQLiteConnection conn = new SQLiteConnection("TodoItemDataBase.db");
            using (var statement = conn.Prepare("DELETE FROM TodoItemList WHERE Id = ?"))
            {
                statement.Bind(1, id);
                statement.Step();
            }
        }

        public static void updateCompleted()   //  点击checkbox时, 更新item的completed
        {
            SQLiteConnection conn = new SQLiteConnection("TodoItemDataBase.db");
            for (int i = 0; i < WholePage.ViewModel.AllItems.Count(); i++)
            {
                using (var custstmt = conn.Prepare("UPDATE TodoItemList SET Completed = ? WHERE Id = ?"))
                {
                    custstmt.Bind(1, WholePage.ViewModel.allItems[i].completed.ToString());
                    custstmt.Bind(2, WholePage.ViewModel.allItems[i].id);
                    custstmt.Step();
                }
            }
        }

        public static ObservableCollection<TodoItem> getAllItems()      //  重启时, 读取数据库的数据
        {
            SQLiteConnection conn = new SQLiteConnection("TodoItemDataBase.db");
            ObservableCollection<TodoItem> items = new ObservableCollection<TodoItem>();

            using (var statement = conn.Prepare("SELECT Id, Title, Description, Date, Completed, ImageName FROM TodoItemList"))
            {

                while (SQLiteResult.ROW == statement.Step())
                {
                    TodoItem item = new TodoItem(new BitmapImage(new Uri("ms-appx:///Assets/" + (string)statement[5])), (string)statement[1], (string)statement[2], Convert.ToDateTime(statement[3]), Convert.ToBoolean(statement[4]));
                    item.id = (string)statement[0];
                    items.Add(item);
                }
            }
            return items;
        }

        public static StringBuilder query(string input)
        {
            string input2 = "%" + input + "%";
            SQLiteConnection conn = new SQLiteConnection("TodoItemDataBase.db");
            StringBuilder result = new StringBuilder();
            using (var statement = conn.Prepare("SELECT Title,Description,Date FROM TodoItemList WHERE Title LIKE ? OR Description LIKE ? OR Date LIKE ?"))
            {
                statement.Bind(1, input2);
                statement.Bind(2, input2);
                statement.Bind(3, input2);
                while (SQLiteResult.ROW == statement.Step())
                {
                    result.Append("Title: ").Append((string)statement[0]).Append(" Description: ").Append((string)statement[1]).Append(" Date: ").Append((string)statement[2]).Append("\n");
                }
            }
            return result;
        }
    }
}

