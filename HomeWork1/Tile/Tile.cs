using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using HomeWork1;

namespace Todos.Tile
{
    public class Tile
    {
        public static void tileCreate()       //  创建磁贴
        {
            Windows.Data.Xml.Dom.XmlDocument document = new Windows.Data.Xml.Dom.XmlDocument();
            document.LoadXml(File.ReadAllText("XMLFile1.xml"));     //  读取XML文件

            var Texttitle = document.GetElementsByTagName("text");

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            for (int i = 0; i < WholePage.ViewModel.AllItems.Count; i++)
            {
                if (i < 5)
                {
                    Texttitle[0].InnerText = Texttitle[2].InnerText = Texttitle[4].InnerText = WholePage.ViewModel.AllItems[i].title;
                    Texttitle[1].InnerText = Texttitle[3].InnerText = Texttitle[5].InnerText = WholePage.ViewModel.AllItems[i].description;
                    TileNotification newTile = new TileNotification(document);
                    TileUpdateManager.CreateTileUpdaterForApplication().Update(newTile);
                }
            }
        }
    }
}
