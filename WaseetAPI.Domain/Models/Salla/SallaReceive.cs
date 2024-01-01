using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
//using System.Text;

namespace WaseetAPI.Domain.Models.Salla
{
    public class SallaReceive
    {
        //[Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        //[JsonIgnore]
        //public decimal? id { get; set; }
        [JsonProperty("event")]
        public string Event { get; set; }
        public string merchant { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? created_at { get; set; }
        //[NotMapped]
        public SallaOrders data { get; set; }
        //string _data_msg;
        //[JsonIgnore]
        //public string data_msg
        //{
        //    get
        //    {
        //        return _data_msg;
        //    }
        //    set
        //    {
        //        _data_msg = JsonConvert.SerializeObject(data);
        //    }
        //}
        //[JsonIgnore]
        //public int status { get; set; }
    }
    public class CustomDateTimeConverter : DateTimeConverterBase
    {
        private const string Format = "dd-MM-yyyy HH:mm:ss";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString(Format));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }

            var s = reader.Value.ToString();
            DateTime result;
            if (DateTime.TryParseExact(s, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            return null;
        }
    }
}
