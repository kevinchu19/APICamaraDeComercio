using APICamaraDeComercio.Services.Entities;
using Newtonsoft.Json;

namespace APICamaraDeComercio.Services
{
    public class FieldMapper
    {
        private static FieldMap? fieldMap { get; set; } = null;
        private static string fileContent = String.Empty;

        public bool LoadMappingFile(string path)
        {
            try
            {
                fileContent = System.IO.File.ReadAllText(path);
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };
                fieldMap = JsonConvert.DeserializeObject<FieldMap>(fileContent, settings);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
    }
}
