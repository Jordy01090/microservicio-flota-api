
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;

namespace Gestion_Flota.Shared
{
    public class DBXmlMethods
    {


        //public static XDocument GetXml<T>(T criterio)
        //{
        //    XDocument resultado = new XDocument(new XDeclaration("1.0", "utf-8", "true"));
        //    try
        //    {
        //        XmlSerializer xs = new XmlSerializer(typeof(T));//crea un serializador
        //        using XmlWriter xw = resultado.CreateWriter();
        //        xs.Serialize(xw, criterio);
        //        return resultado;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}

        public static XDocument GetXml<T>(T criterio)
        {
            try
            {
                var resultado = new XDocument(new XDeclaration("1.0", "utf-8", "true"));
                XmlSerializer xs = new XmlSerializer(typeof(T));
                using XmlWriter xw = resultado.CreateWriter();
                xs.Serialize(xw, criterio);
                return resultado;
            }
            catch
            {
                // en vez de `return null;`
                return new XDocument(new XElement("root"));
            }
        }



        public static async Task<DataSet> EjecutaBase(string nombreProcedimiento, string cadenaConexion, string proceso, string dataXML)
        {
            DataSet dsResultado = new DataSet();
            SqlConnection cnn = new SqlConnection(cadenaConexion);

            try
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter adt = new SqlDataAdapter();
                cmd.CommandText = nombreProcedimiento;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = cnn;
                cmd.CommandTimeout = 120;
                cmd.Parameters.Add("@iTransaccion", SqlDbType.VarChar).Value = proceso;
                cmd.Parameters.Add("@iXml", SqlDbType.Xml).Value = dataXML.ToString();
                await cnn.OpenAsync().ConfigureAwait(false);

                adt = new SqlDataAdapter(cmd);
                adt.Fill(dsResultado);

                cmd.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.Write("Logs", "EjecutaBase", ex.ToString());
                cnn.Close();
            }
            finally
            {
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }
            return dsResultado;

        }



    }
}
