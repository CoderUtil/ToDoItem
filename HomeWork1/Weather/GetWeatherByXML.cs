using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XML.Weather
{
    public class GetWeatherByXML
    {
        public async static Task<CityWeatherResponse> GetWeather(string Location)
        {
            string uri = "http://api.map.baidu.com/telematics/v3/weather?location=" + Location + "&ak=8IoIaU655sQrs95uMWRWPDIa"; ;
            var http = new HttpClient();
            var response = await http.GetAsync(uri);     //  获取http响应
            var result = await response.Content.ReadAsStringAsync();    //  抓取结果并转为字符串
            var serializer = new XmlSerializer(typeof(CityWeatherResponse)); //  去序列化, 将字符串转为Object

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (CityWeatherResponse)serializer.Deserialize(ms);
            return data;
        }
    }

    [XmlRoot(ElementName = "weather_data")]
    public class Weather_data
    {
        [XmlElement(ElementName = "temperature")]
        public List<string> Temperature { get; set; }
    }
    
    [XmlRoot(ElementName = "results")]
    public class Results
    {
        [XmlElement(ElementName = "weather_data")]
        public Weather_data Weather_data { get; set; }
    }

    [XmlRoot(ElementName = "CityWeatherResponse")]
    public class CityWeatherResponse
    {
        [XmlElement(ElementName = "results")]
        public Results Results { get; set; }
    }
}
