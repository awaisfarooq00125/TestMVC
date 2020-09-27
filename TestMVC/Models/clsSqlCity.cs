using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace TestMVC.Models
{
    public class clsSqlCity
    {
        public static DataTableReader getCityListCount()
        {
            DataTable tdt = new DataTable();
            string connection = ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connection))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT Count(s.CityId) MyRowCount FROM tblCity s inner join tblCountry c  on  s.CountryId= c.CountryId";
                cmd.Connection = con;
                SqlDataReader r = cmd.ExecuteReader();
                tdt.Load(r);
                r.Close();
                con.Close();
                SqlConnection.ClearPool(con);
            }
            return tdt.CreateDataReader();

        }
        public static DataTableReader getCityList(string start, string length, string sorting)
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
                cmd.CommandText = " select isnull(s.CityId,0) as 'CityId',isnull(s.CityName,'') + ',' + ISNULL(r.CountryName,'') + ',' + ISNULL(re.RegionName,'')  as 'CityName',ISNULL(r.CountryId,'') as 'CountryId',ISNULL(r.CountryName,'') as 'CountryName',ISNULL(re.RegionId,0) as 'RegionId' from tblCity s inner join tblCountry r on s.CountryId = r.CountryId inner join tblRegion as re on re.RegionId = r.RegionId " + sorting + " OFFSET " + voffset + " ROWS  FETCH NEXT " + length + " ROWS ONLY";
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