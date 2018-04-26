using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using HomeWork1;

namespace Todos.Tile
{
    public class Tile
    {
        public static void tileCreate()       //  创建磁贴
        {
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueueForWide310x150(true);
            updater.EnableNotificationQueueForSquare150x150(true);
            updater.EnableNotificationQueueForSquare310x310(true);
            updater.EnableNotificationQueue(true);
            updater.Clear();



            for (int i = 0; i < WholePage.ViewModel.allItems.Count; i++)
            {
                if (i < 5)
                {
                    XmlDocument document = new XmlDocument();
                    string xmlString = File.ReadAllText("Tile/Tile.xml");
                    document.LoadXml(xmlString);     //  读取XML文件

                    var Texttitle = document.GetElementsByTagName("text");
                    var Image = document.GetElementsByTagName("image");

                    document.LoadXml(string.Format(xmlString, WholePage.ViewModel.allItems[i].title, WholePage.ViewModel.allItems[i].description, WholePage.ViewModel.allItems[i].date.ToString("yyyy-MM-dd"), "ms-appx:///Assets/" + WholePage.ViewModel.allItems[i].imageName));
                    updater.Update(new TileNotification(document));
                }
            }
        }
    }
}
