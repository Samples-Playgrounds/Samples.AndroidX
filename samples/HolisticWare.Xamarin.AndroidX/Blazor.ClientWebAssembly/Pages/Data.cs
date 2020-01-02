using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blazor.ClientWebAssembly
{
    public interface IDataMappings
    {
        public static string UrlMappingsGoogle 
        {
            get;
            set;
        }
        public static string UrlMappingsXamarin
        {
            get;
            set;
        }

    }
    public class DataMappings : IDataMappings
    {
        public DataMappings(Blazored.LocalStorage.ILocalStorageService LocalStorage)
        {

            return;
        }
        private static HttpClient client = new HttpClient();
        public static Blazored.LocalStorage.ILocalStorageService LocalStorage;

        public static string UrlMappingsGoogle 
        {
            get;
            set;
        } = $"https://raw.githubusercontent.com/xamarin/AndroidX/master/mappings/androidx-class-mapping.csv";
        public static string UrlMappingsXamarin 
        {
            get;
            set;
        } = $"https://raw.githubusercontent.com/xamarin/AndroidX/master/mappings/androidx-mapping.csv";

        private static string string_mappings_google;
        private static string string_mappings_xamarin;

        public static IEnumerable<MappingsGoogle> mappings_google;
        public static IEnumerable<MappingsXamarin> mappings_xamarin;

        public static async Task  GetSomeAsync()
        {
            string_mappings_google = await client.GetStringAsync(UrlMappingsGoogle);
            string_mappings_xamarin = await client.GetStringAsync(UrlMappingsXamarin);

            Console.WriteLine($"Mappings = {string_mappings_google}");

            await LocalStorage.SetItemAsync("androidx-class-mapping.csv", string_mappings_google);
            await LocalStorage.SetItemAsync("androidx-mapping.csv", string_mappings_xamarin);
            //Console.WriteLine($"Mappings = {result_xamarin}");

            return;
        }
    }
}