using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Todos.DataBaseModels;

namespace Todos.Models
{
    public class TodoItem      
    {
        public string id { get; set; }

        public BitmapImage image { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public DateTimeOffset date { get; set; }

        public bool completed { get; set; }

        public TodoItem(BitmapImage image, string title, string description, DateTimeOffset date, bool completed = false)
        {
            this.id = Guid.NewGuid().ToString();
            this.image = image;
            this.title = title;
            this.description = description;
            this.date = date;
            this.completed = completed;
        }
    }
}
