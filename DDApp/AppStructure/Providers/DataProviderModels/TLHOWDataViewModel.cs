using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DDApp.AppStructure.Providers.DataProviderModels
{
    public class TLHOWLineTypeEnumConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (string.IsNullOrEmpty(reader.Value.ToString()))
                return TLHOWLineType.Thin;

            return base.ReadJson(reader, objectType, existingValue, serializer);
        }
    }

    public enum TLHOWDataFormat
    {
        Number,
        Percentage
    }

    [JsonConverter(typeof(TLHOWLineTypeEnumConverter))]
    public enum TLHOWLineType
    {
        Thin,
        Thick
    }

    public class TLHOWDataItemChartViewModel
    {
        [JsonProperty(PropertyName = "dottet-num")]
        public string AvgVal { get; set; }
        [JsonProperty(PropertyName = "solid-num")]
        public string Current { get; set; }
        [JsonProperty(PropertyName = "solid-line-val")]
        public string CurrentVal { get; set; }
    }

    public class TLHOWDataItemViewModel
    {
        [JsonProperty(PropertyName = "quality-title"), JsonRequired]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "quality-count"), JsonRequired]
        public string Value { get; set; }
        [JsonProperty(PropertyName = "quality-trees")]
        public double? MarkCount { get; set; }
        [JsonProperty(PropertyName = "quality-thickness"), JsonRequired]
        public TLHOWLineType LineType { get; set; }
        [JsonProperty(PropertyName = "quality-chart"), JsonRequired]
        public TLHOWDataItemChartViewModel Chart { get; set; }
        [JsonProperty(PropertyName = "quality-chart-info")]
        public string ChartInfo { get; set; }
        [JsonProperty(PropertyName = "quality-chart-file")]
        public string ChartFileUrl { get; set; }
    }

    public class TLHOWQualityViewModel
    {
        [JsonProperty(PropertyName = "chart"), JsonRequired]
        public List<TLHOWDataItemViewModel> Items { get; set; }
        [JsonProperty(PropertyName = "gold-line-title"), JsonRequired]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "gold-line-year"), JsonRequired]
        public string Year { get; set; }
    }

    public class TLHOWImageUrl : Dictionary<string, string>
    {
        /// <summary>
        /// Return the URL for the requested image size
        /// </summary>
        /// <param name="size">
        /// thumb
        /// xs
        /// s
        /// m
        /// l
        /// xl
        /// </param>
        /// <returns>URL</returns>
        public string GetUrl(string size)
        {
            return this[size];
        }
    }

    public class TLHOWAddress
    {
        [JsonProperty(PropertyName = "Street")]
        public string Street { get; set; }
        [JsonProperty(PropertyName = "StreetNr")]
        public string StreetNr { get; set; }
        [JsonProperty(PropertyName = "Postcode")]
        public string Postcode { get; set; }
        [JsonProperty(PropertyName = "Place")]
        public string Place { get; set; }
        [JsonProperty(PropertyName = "Longitude")]
        public decimal Longitude { get; set; }
        [JsonProperty(PropertyName = "Latitude")]
        public decimal Latitude { get; set; }
        [JsonProperty(PropertyName = "Phone")]
        public string Phone { get; set; }
        [JsonProperty(PropertyName = "PhonePrefix")]
        public string PhonePrefix { get; set; }
        [JsonProperty(PropertyName = "PhoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty(PropertyName = "PhoneDirect")]
        public string PhoneDirect { get; set; }
        [JsonProperty(PropertyName = "Fax")]
        public string Fax { get; set; }
        [JsonProperty(PropertyName = "FaxPrefix")]
        public string FaxPrefix { get; set; }
        [JsonProperty(PropertyName = "FaxNumber")]
        public string FaxNumber { get; set; }
        [JsonProperty(PropertyName = "FaxDirect")]
        public string FaxDirect { get; set; }
        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "Url")]
        public string Url { get; set; }
    }

    public class TLHOWCertViewModel
    {
        [JsonProperty(PropertyName = "image")]
        public List<TLHOWImageUrl> Images { get; set; }
        [JsonIgnore]
        public string ImageUrl { get { return Images?.FirstOrDefault().GetUrl("m"); } }
        [JsonProperty(PropertyName = "link")]
        public string SiteUrl { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }

    public class TLHOWItemDescViewModel
    {
        [JsonProperty(PropertyName = "desc")]
        public string Description { get; set; }
    }

    public class TLHOWChiefDoctor
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "sort")]
        public int Sort { get; set; }
        [JsonProperty(PropertyName = "firstname")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "funktion")] // Funktion is writen with a 'C' Klaus
        public string Function { get; set; }
        [JsonProperty(PropertyName = "cv")]
        public string CV { get; set; }
        [JsonProperty(PropertyName = "picture")]
        public List<TLHOWImageUrl> Images { get; set; }
        [JsonIgnore]
        public string Image { get { return Images?.FirstOrDefault().GetUrl("m"); } }
        [JsonProperty(PropertyName = "address")]
        public TLHOWAddress Address { get; set; }
    }

    public class TLHOWDataViewModel
    {
        [JsonProperty(PropertyName = "fa")]
        public string FA { get; set; }
        [JsonProperty(PropertyName = "certificates")]
        public List<TLHOWCertViewModel> Certificates { get; set; }
        [JsonProperty(PropertyName = "qualities")] // NOT IN KLAUS API
        public Dictionary<string, TLHOWQualityViewModel> Qualities { get; set; }
        [JsonProperty(PropertyName = "equipments")] // NOT IN KLAUS API
        public Dictionary<string, TLHOWItemDescViewModel> Equipments { get; set; }
        [JsonProperty(PropertyName = "information")] // NOT IN KLAUS API
        public Dictionary<string, TLHOWItemDescViewModel> Information { get; set; }
        [JsonProperty(PropertyName = "services_DONT")] // NOT IN KLAUS API
        public Dictionary<string, TLHOWItemDescViewModel> Services { get; set; }
        [JsonIgnore]
        public string ImageMain { get { return ImageGallery?.FirstOrDefault().GetUrl("thumb"); } }
        [JsonIgnore]
        public string ImageMainBig { get { return ImageGallery?.FirstOrDefault().GetUrl("xl"); } }
        [JsonProperty(PropertyName = "picture")]
        public List<TLHOWImageUrl> ImageGallery { get; set; }
        [JsonIgnore]
        public string HeadDoctorImage { get { return ChiefDoctors?.OrderBy(x => x.Sort).FirstOrDefault()?.Image; } }
        [JsonIgnore]
        public string HeadDoctorName { get { return ChiefDoctors?.OrderBy(x => x.Sort).FirstOrDefault()?.Name; } }
        [JsonProperty(PropertyName = "desc")]
        public string DepartmentDescription { get; set; }
        [JsonProperty(PropertyName = "special")]
        public string DepartmentSpecialDescription { get; set; }
        [JsonProperty(PropertyName = "chief_doctors")]
        public TLHOWChiefDoctor[] ChiefDoctors { get; set; }
        [JsonProperty(PropertyName = "workingtime")] // is empty string in the response
        public string WorkingTime { get; set; }
        [JsonProperty(PropertyName = "address")]
        public TLHOWAddress Address { get; set; }
        [JsonIgnore]
        public string Phone { get { return Address?.Phone; } }
        [JsonIgnore]
        public string EMail { get { return Address?.Email; } }
        [JsonIgnore]
        public string Website { get { return Address?.Url; } }
    }
}
