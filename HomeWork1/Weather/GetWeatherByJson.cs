using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace JSON.Weather
{
    public class GetWeatherByJson
    {
        public async static Task<RootObject> GetWeather(double lat, double lon)
        {
            string uri = "http://openweathermap.org/data/2.5/weather?lat=" + lat + "&lon=" + lon + "&appid=b6907d289e10d714a6e88b30761fae22";
            var http = new HttpClient();
            var response = await http.GetAsync(uri);     //  获取http响应
            var result = await response.Content.ReadAsStringAsync();    //  抓取结果并转为字符串
            var serializer = new DataContractJsonSerializer(typeof(RootObject)); //  去序列化, 将字符串转为Object

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (RootObject)serializer.ReadObject(ms);

            return data;
        }
    }

    [DataContract]
    public class Weather
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string main { get; set; }

        [DataMember]
        public string description { get; set; }

        [DataMember]
        public string icon { get; set; }
    }

    [DataContract]
    public class Main
    {
        [DataMember]
        public string temp { get; set; }

        [DataMember]
        public string humidity { get; set; }

        [DataMember]
        public string pressure { get; set; }

        [DataMember]
        public string temp_min { get; set; }

        [DataMember]
        public string temp_max { get; set; }
    }

    [DataContract]
    public class RootObject
    {
        [DataMember]
        public List<Weather> weather { get; set; }

        [DataMember]
        public Main main { get; set; }

        [DataMember]
        public string name { get; set; }
    }
}
