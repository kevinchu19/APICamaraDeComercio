using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.Response.Pdf;
using APICamaraDeComercio.Services.Entities;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Reflection;
using System.Transactions;

namespace APICamaraDeComercio.Repositories
{
    public class RepositoryBase
    {
        public RepositoryBase(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public async Task<string> ExecuteSqlInsertToTablaSAR(List<FieldMap> fieldMapList, object resource, object valorIdentificador, string jobName)
        {
            string query = "";

            foreach (FieldMap fieldMap in fieldMapList)
            {
                if (fieldMap.ParentTable != null)
                {
                    int index = 0;
                    foreach (var item in (dynamic)resource.GetType().GetProperty(fieldMap.ParentProperty).GetValue(resource, null))
                    {
                        index++;
                        query += ArmoQueryInsertTablaSAR(fieldMap, item, valorIdentificador, index) + ";";
                    }
                }
                else
                {
                    query += ArmoQueryInsertTablaSAR(fieldMap, resource, valorIdentificador,0) + ";";
                }
            }
            

            using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnectionString")))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    try
                    {
                        await command.ExecuteNonQueryAsync();

                        await InsertaCwJmSchedules(jobName);
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627)
                        {
                            return $"Error al generar registracion. El id de operacion ya existe.";
                        }
                        else
                        {
                            return ex.Message + ex.StackTrace;
                        }

                    }
                }

                return "";
            }
        }

        private async Task InsertaCwJmSchedules(string codjob)
        {
            using (SqlConnection sql = new SqlConnection(Configuration.GetConnectionString("DefaultConnectionString")))
            {
                using (SqlCommand cmd = new SqlCommand("ALM_InsCwJmSchedules", sql))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@CODJOB", codjob));

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    //Logger.Information("Se insertó cwjmschedules");
                }
            }
        }

        private string ArmoQueryInsertTablaSAR(FieldMap fieldMap, object resource, object valorIdentificador, int index)
        {
           
            Type typeComprobante = resource.GetType();

            string query = "INSERT INTO [dbo].[" + fieldMap.Table + "] (";

            foreach (var item in fieldMap.Fields)
            {
                query = query + item.Field + ",";
            }

            query = query.Remove(query.Length - 1, 1) + ") ( SELECT ";

            foreach (var item in fieldMap.Fields)
            {
                query += ResuelvoField(item, resource, valorIdentificador, index) + ",";
            }
            query = query.Remove(query.Length - 1, 1) + ");";

            return query;


        }

        private string ResuelvoField(FieldValue item, object resource, object valorIdentificador, int index)
        {
            
            if (item.PropertyName == "identificador")
            {
                return (string)FormatStringSql(valorIdentificador);
            }

            if (item.PropertyName == "item")
            {
                return index.ToString();
            }
            if (item.PropertyName != null)
            {
                object value = resource.GetType()
                                .GetProperty(item.PropertyName)
                                .GetValue(resource, null);

                if (value != null)
                {
                    if (value is string )
                    {
                        return FormatStringSql(value);
                    }
                
                    decimal number;
                    if (decimal.TryParse(Convert.ToString(value), out number))
                    {
                        return Convert.ToString(value);
                    }

                
                }
            }
            
            if (item.FixedValue!=null)
            {
                return FormatStringSql(item.FixedValue);
            }

            if (item.Function!=null)
            {
                string script = $"(SELECT dbo.{item.Function.Name} ('";

                foreach (var parameter in item.Function.Parameters)
                {
                    script += $"{resource.GetType().GetProperty(parameter.PropertyName).GetValue(resource, null)},";
                }
                script = script.Remove(script.Length - 1, 1) + "') )";
                return script;
            }

            return FormatStringSql("");
        }

        private string FormatStringSql(object value)
        {

            if (value == null)
            {
                return "NULL";
            }

            if (value.ToString() == "NULL")
            {
                return "NULL";
            }

            if (value.ToString() == "GETDATE()")
            {
                return value.ToString();
            }

            if (value.ToString().IndexOf("ROW_NUMBER") > -1)
            {
                return value.ToString();
            }

            return "'" + value.ToString() + "'";
        }

        public virtual async Task<ComprobanteResponse> GetTransaccion(string identificador, string table)
        {
            string query = $"SELECT {table}_STATUS, ISNULL({table}_MODFOR, ISNULL({table}_MODFVT, {table}_MODFST)) {table}_MODFOR, " +
                $"ISNULL({table}_CODFOR, ISNULL({table}_CODFVT, {table}_CODFST)) {table}_CODFOR, " +
                $"ISNULL({table}_NROFOR, ISNULL({table}_NROFVT, {table}_NROFST)) {table}_NROFOR " +
                $"FROM {table} WHERE {table}_IDENTI = '{identificador}'";

            using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnectionString")))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    try
                    {
                        await connection.OpenAsync();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    switch ((string)reader[$"{table}_STATUS"])
                                    {
                                        case "E":
                                            return new ComprobanteResponse(new ComprobanteDTO (identificador, (string)reader[$"{table}_STATUS"], "Procesada con error",(string)reader[$"{table}_ERRMSG"],null));

                                        case "S":
                                            return new ComprobanteResponse(new ComprobanteDTO(identificador,
                                                                                    (string)reader[$"{table}_STATUS"], 
                                                                                    "Procesada Exitosamente", 
                                                                                    "", 
                                                                                    new ComprobanteGenerado { codigocomprobante= (string)(reader[$"{table}_CODFOR"] ),
                                                                                                              numerocomprobante= Convert.ToInt64(reader[$"{table}_NROFOR"])
                                                                                    }));
                                        case "N":
                                            return new ComprobanteResponse(new ComprobanteDTO(identificador,
                                                                                   (string)reader[$"{table}_STATUS"],
                                                                                    "Pendiente de procesar",
                                                                                    "",
                                                                                    null));

                                        default:
                                            break;
                                    }

                                }
                            }
                            else
                            {
                                return new ComprobanteResponse(new ComprobanteDTO(identificador, "404", "Identificador Inexistente", $"El identificador {identificador} no existe.", null));
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        return new ComprobanteResponse(new ComprobanteDTO(identificador, "500", "Error de acceso", $"Error de conexion con la base de datos", null));
                    }

                    return new ComprobanteResponse(new ComprobanteDTO(identificador, "200", "", "", null));
                }


            }
        }

        public async Task<TResult?> ExecuteStoredProcedure<TResult>(string sqlCommand, Dictionary<string, object> parameters)
        {
            TResult? result = default;

            using (SqlConnection sql = new SqlConnection(Configuration.GetConnectionString("DefaultConnectionString")))
            {
                using (SqlCommand cmd = new SqlCommand(sqlCommand, sql))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.Add(new SqlParameter(item.Key, item.Value));
                    }

                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result = MapToValue<TResult>(reader);
                        }
                    }
                }
            }

            return (TResult?)result;

        }

        public TResponse MapToValue<TResponse>(SqlDataReader reader)
        {
            var respuesta = (TResponse)Activator.CreateInstance(typeof(TResponse), null);
            Type typeResponse = typeof(TResponse);
            System.Reflection.PropertyInfo[] listaPropiedades = typeResponse.GetProperties();

            for (int i = 0; i < listaPropiedades.Count(); i++)
            {
                if (reader.GetColumnSchema().Any(c => c.ColumnName == listaPropiedades[i].Name))
                {
                    if (reader[listaPropiedades[i].Name] != DBNull.Value)
                    {

                        if (listaPropiedades[i].PropertyType == typeof(string))
                        {
                            listaPropiedades[i].SetValue(respuesta, reader[listaPropiedades[i].Name].ToString());
                        }
                        else
                        {
                            if (listaPropiedades[i].PropertyType == typeof(bool))
                            {
                                listaPropiedades[i].SetValue(respuesta, reader[listaPropiedades[i].Name].ToString() == "S" ? true : false);
                            }
                            else
                            {
                                if (listaPropiedades[i].PropertyType == typeof(decimal))
                                {
                                    listaPropiedades[i].SetValue(respuesta, (decimal)reader[listaPropiedades[i].Name]);
                                }
                                else
                                {
                                    listaPropiedades[i].SetValue(respuesta, reader[listaPropiedades[i].Name]);
                                }
                            }
                        }

                    }
                }

            }

            return respuesta;
        }

    }
}
