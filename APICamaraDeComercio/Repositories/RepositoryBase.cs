using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.Response.Pdf;
using APICamaraDeComercio.Services.Entities;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

        public virtual async Task<string> ExecuteSqlInsertToTablaSAR(List<FieldMap> fieldMapList, object resource, object valorIdentificador, string jobName)
        {
            string query = "";
            Dictionary<string, object> fullFieldValues = new Dictionary<string, object>();
            Dictionary<string, object> fieldValues = new Dictionary<string, object>();


            foreach (FieldMap fieldMap in fieldMapList)
            {
                if (fieldMap.ParentTable != null)
                {
                    int index = 0;
                    foreach (var item in (dynamic)resource.GetType().GetProperty(fieldMap.ParentProperty).GetValue(resource, null))
                    {
                        index++;
                        query += ArmoQueryInsertTablaSAR(fieldMap, item, valorIdentificador, index, out fieldValues,resource) + ";";
                        foreach (var fieldValue in fieldValues)
                        {
                            fullFieldValues.Add(fieldValue.Key, fieldValue.Value);
                        }
                    }
                }
                else
                {
                    query += ArmoQueryInsertTablaSAR(fieldMap, resource, valorIdentificador,0, out fieldValues, null) + ";";
                    foreach (var fieldValue in fieldValues)
                    {
                        fullFieldValues.Add(fieldValue.Key, fieldValue.Value);
                    }
                }
            }
            

            using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnectionString")))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    
                    try
                    {
                        foreach (var item in fullFieldValues)
                        {
                            command.Parameters.AddWithValue(item.Key.ToString(), item.Value is null?DBNull.Value: item.Value);

                        }
                        await connection.OpenAsync();

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

        private string ArmoQueryInsertTablaSAR(FieldMap fieldMap, object resource, object valorIdentificador, int index
            , out Dictionary<string, object> fieldValues, object? parentResource = null)
        {
            fieldValues = new Dictionary<string, object>();

            Type typeComprobante = resource.GetType();

            string query = "INSERT INTO [dbo].[" + fieldMap.Table + "] (";

            foreach (var item in fieldMap.Fields)
            {
                query = query + item.Field + ",";
            }

            query = query.Remove(query.Length - 1, 1) + ") VALUES ( ";

            foreach (var item in fieldMap.Fields)
            {
               
                //query += ResuelvoField(item, resource, valorIdentificador, index, parentResource) + ",";
                if (item.FixedValue!=null)
                {
                    query += $"{FormatStringSql(item.FixedValue)},";
                }
                else
                {
                    fieldValues.Add($"@{item.Field}_{index.ToString()}", ResuelvoField(item, resource, valorIdentificador, index, parentResource));
                    query += $"@{item.Field}_{index.ToString()},";
                }
            }
            query = query.Remove(query.Length - 1, 1) + ");";
            
            return query;
        }

        private  object ResuelvoField(FieldValue item, object resource, object valorIdentificador, int index, object? parentResource = null)
        {
            
            if (item.PropertyName == "identificador")
            {
                return valorIdentificador;
            }

            if (item.PropertyName == "item")
            {
                return index;
            }
            if (item.PropertyName != null)
            {
                return resource.GetType()
                                .GetProperty(item.PropertyName)
                                .GetValue(resource, null);

                
            }
            
            if (item.FixedValue!=null)
            {
                return item.FixedValue;
            }

            if (item.Function != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();

                if (item.Function.Parameters.Count > 0)
                {  
                    
                    foreach (var parameter in item.Function.Parameters)
                    {
                        if (parameter.FixedValue != null)
                        {
                         
                            parameters.Add(parameter.Name, parameter.FixedValue);
                        }
                        else
                        {
                            try
                            {
                                parameters.Add(parameter.Name, resource.GetType().GetProperty(parameter.PropertyName).GetValue(resource, null));
                            }
                            catch (Exception)
                            {
                                parameters.Add(parameter.Name, parentResource.GetType().GetProperty(parameter.PropertyName).GetValue(parentResource, null));
                            }
                        }
                    }
                    
                }
                return ExecuteFunction(item.Function.Name, parameters);
               

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
            string query = $"SELECT {table}_STATUS, {table}_ERRMSG,ISNULL({table}_MODFOR, ISNULL({table}_MODFVT, {table}_MODFST)) {table}_MODFOR, " +
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
                                        case "P":
                                            return new ComprobanteResponse(new ComprobanteDTO(identificador,
                                                                                   (string)reader[$"{table}_STATUS"],
                                                                                    "En procesamiento",
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
        public async Task<List<TResult?>> ExecuteStoredProcedureList<TResult>(string sqlCommand, Dictionary<string, object> parameters)
        {

            List<TResult?> result = new List<TResult?>();

            using (SqlConnection sql = new SqlConnection(Configuration.GetConnectionString("DefaultConnectionString")))
            {
                using (SqlCommand cmd = new SqlCommand(sqlCommand, sql))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    foreach (var item in parameters)
                    {
                        SqlParameter parameter = new SqlParameter(item.Key, item.Value);
                        if (item.Value != null)
                        {
                            if (item.Value.GetType() == typeof(DataTable))
                            {
                                parameter.SqlDbType = SqlDbType.Structured;
                                parameter.TypeName = "dbo.Alm_SeguimientoFacturacionList";
                            }
                        }
                        cmd.Parameters.Add(parameter);

                    }

                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(MapToValue<TResult>(reader));
                        }
                    }
                }
            }

            return (List<TResult?>)result;

        }

        public async Task<List<TResult?>> ExecuteStoredProcedureArray<TResult>(string sqlCommand, Dictionary<string, object>? parameters = null)
        {
            List<TResult?> result = new List<TResult?>();

            using (SqlConnection sql = new SqlConnection(Configuration.GetConnectionString("DefaultConnectionString")))
            {
                using (SqlCommand cmd = new SqlCommand(sqlCommand, sql))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    if (parameters != null)
                    { 
                        foreach (var item in parameters)
                        {
                            SqlParameter parameter = new SqlParameter(item.Key, item.Value);
                            if (item.Value != null)
                            {
                                if (item.Value.GetType() == typeof(DataTable))
                                {
                                    parameter.SqlDbType = SqlDbType.Structured;
                                    parameter.TypeName = "dbo.Alm_SeguimientoFacturacionList";
                                }
                            }
                            cmd.Parameters.Add(parameter);

                        }
                    }

                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add((TResult)reader["values"]);
                        }
                    }
                }
            }

            return (List<TResult?>)result;
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
                        SqlParameter parameter = new SqlParameter(item.Key, item.Value);
                        if (item.Value != null)
                        {
                            if (item.Value.GetType() == typeof(DataTable))
                            {
                                parameter.SqlDbType = SqlDbType.Structured;
                                parameter.TypeName = "dbo.Alm_SeguimientoFacturacionList";
                            }
                        }
                        cmd.Parameters.Add(parameter);
                        
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
        private  object? ExecuteFunction (string sqlCommand, Dictionary<string, object> parameters)
        {

            object? result = null;

            using (SqlConnection sql = new SqlConnection(Configuration.GetConnectionString("DefaultConnectionString")))
            {
                using (SqlCommand cmd = new SqlCommand(sqlCommand, sql))
                {

                    cmd.CommandText = $"SELECT dbo.{sqlCommand} (";                    
                    foreach (var item in parameters)
                    {
                        cmd.CommandText += $"@{item.Key},";
                        SqlParameter parameter = new SqlParameter(item.Key, item.Value is null? DBNull.Value:item.Value);
                        cmd.Parameters.Add(parameter);
                    }
                    cmd.CommandText = cmd.CommandText.Remove(cmd.CommandText.Length - 1, 1);
                    cmd.CommandText += ") AS result";

                     sql.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = reader["result"];
                        }
                    }
                }
            }

            return result;

        }
    }
    
}
