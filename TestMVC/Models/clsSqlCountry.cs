using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace TestMVC.Models
{
    public class clsSqlCountry
    {
        public static DataTableReader getCountryListCount()
        {
            DataTable tdt = new DataTable();
            string connection = ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connection))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT Count(s.CountryId) MyRowCount FROM tblCountry s inner join tblRegion r on s.RegionId = r.RegionId";
                cmd.Connection = con;
                SqlDataReader r = cmd.ExecuteReader();
                tdt.Load(r);
                r.Close();
                con.Close();
                SqlConnection.ClearPool(con);
            }
            return tdt.CreateDataReader();
           
        }
        public static DataTableReader getCountryList(string start, string length, string sorting)
        {
            DataTable tdt = new DataTable();
            string connection = ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connection))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                if (string.IsNullOrEmpty(start))
                {
                    start = "0";
                }
                if (string.IsNullOrEmpty(length))
                {
                    length = "0";
                }
                int voffset = (Convert.ToInt32(start) / 10) * Convert.ToInt32(length);
                cmd.CommandText = " select isnull(s.CountryId,0) as 'CountryId',isnull(s.CountryName,'') as 'CountryName',ISNULL(r.RegionId,'') as 'RegionId',ISNULL(r.RegionName,'') as 'RegionName' from tblCountry s inner join tblRegion r on s.RegionId = r.RegionId " + sorting + " OFFSET " + voffset + " ROWS  FETCH NEXT " + length + " ROWS ONLY";
                cmd.Connection = con;
                SqlDataReader r = cmd.ExecuteReader();
                tdt.Load(r);
                r.Close();
                con.Close();
                SqlConnection.ClearPool(con);
            }

            return tdt.CreateDataReader();
        }
    }
}