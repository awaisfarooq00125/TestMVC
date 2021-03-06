﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestMVC.Models;
using System.Data;
using System.Data.SqlClient;

namespace TestMVC.Controllers
{
    public class CityController : Controller
    {
        TestDbEntities db = new TestDbEntities();
        
        public ActionResult CityIndex()
        {
	int a = 0 ;
            return View();
        }
        public ActionResult GetAllCities()
        {
            //if (User != null)
            //{


            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            //var HallName = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            //string whereCondition = "";
            string sorting = "";
            if (!(string.IsNullOrEmpty(sortColumn) && !(string.IsNullOrEmpty(sortColumnDir))))
            {
                if (!string.IsNullOrEmpty(sortColumn))
                {
                    sorting = " Order by " + sortColumn + " " + sortColumnDir + "";
                }
            }
            else
            {
                sorting = " Order by s.CityId asc";
            }
            //if (!(string.IsNullOrEmpty(HallName)))
            //{
            //    whereCondition = " LOWER(s.HallName) like ('%" + HallName + "%')";
            //}
            //else
            //{
            //    whereCondition = " LOWER(s.HallName) like ('%%')";
            //}
            List<CityViewModel> listsub = new List<CityViewModel>();
            DataTableReader dtr = clsSqlCity.getCityListCount();
            while (dtr.Read())
            {
                recordsTotal = Convert.ToInt32(dtr["MyRowCount"]);
            }
            DataTableReader dt = clsSqlCity.getCityList(start, length, sorting);
            //     int i = 0;
            while (dt.Read())
            {
                listsub.Add(new CityViewModel()
                {
                    CityId = Convert.ToInt32(dt["CityId"]),
                    CityName = dt["CityName"].ToString(),
                    CountryId = Convert.ToInt32(dt["CountryId"]),
                    CountryName = dt["CountryName"].ToString()
                });
            }
            //List<clsCountry> listsub = new List<clsCountry>();
            //listsub = (from c in db.tblCountries
            //           select new clsCountry
            //           {
            //               CountryId = c.CountryId,
            //               CountryName = c.CountryName
            //           }).ToList();
            var data = listsub.ToList();
            //recordsTotal = listsub.Count();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                JsonRequestBehavior.AllowGet);
        }

      

        //add & edit City
        public ActionResult InsertUpdateCity(int id)
        {
            if (User != null)
            {
                CityViewModel user = new CityViewModel();
                if (id > 0)
                {

                    user = (from f in db.tblCity
                            join r in db.tblCountry on f.CountryId equals r.CountryId
                            where f.CityId == id
                            select new CityViewModel
                            {
                                CityId = f.CityId,
                                CityName = f.CityName,
                                CountryId = r.CountryId,
                                CountryName = db.tblCountry.Where(x => x.CountryId == r.CountryId).Select(x => x.CountryName).FirstOrDefault() ?? ""
                            }).FirstOrDefault();
                }
                else
                {
                    user.CityId = 0;
                    user.CountryId = 0;
                    user.CountryName = "";
                    user.CityName = "";
                }
                return PartialView(user);
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }
        [HttpPost]
        public ActionResult InsertUpdateCity(CityViewModel user)
        {
            string message = "";
            bool status = false;
            try
            {
                string returnId = "0";
                string insertUpdateStatus = "";
                int? CityId = user.CityId;
                if (user.CityId > 0)
                {
                    insertUpdateStatus = "Update";
                }
                else
                {
                    insertUpdateStatus = "Save";
                }
                returnId = InsertUpdateCityDb(user, insertUpdateStatus);
                if (returnId == "Success")
                {
                    ModelState.Clear();
                    status = true;
                    message = "City Successfully Updated";
                }
                else
                {
                    ModelState.Clear();
                    status = false;
                    message = returnId;
                }
            }
            catch (Exception ex)
            {
                status = false;
                message = ex.Message.ToString();
            }

            return new JsonResult { Data = new { status = status, message = message } };
        }
        private string InsertUpdateCityDb(CityViewModel st, string insertUpdateStatus)
        {
            string returnId = "0";
            string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connection))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateCity", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@CountryId", SqlDbType.Int).Value = st.CountryId;
                        cmd.Parameters.Add("@CityId", SqlDbType.Int).Value = st.CityId;
                        cmd.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = st.CountryName;
                        cmd.Parameters.Add("@CityName", SqlDbType.NVarChar).Value = st.CityName;
                        cmd.Parameters.Add("@InsertUpdateStatus", SqlDbType.NVarChar).Value = insertUpdateStatus;
                        cmd.Parameters.Add("@CheckReturn", SqlDbType.NVarChar, 300).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        returnId = cmd.Parameters["@CheckReturn"].Value.ToString();
                        cmd.Dispose();


                    }
                    con.Close();
                    con.Dispose();
                }
                catch (Exception ex)
                {
                    returnId = ex.Message.ToString();
                }
            }
            return returnId;
        }

        public ActionResult DeleteModule(int id)
        {
            string message = "";
            bool status = false;

            CityViewModel st = new CityViewModel();
            st.CityId = id;

            st.CityName = "";
            string returnId = InsertUpdateCityDb(st, "Delete");
            if (returnId == "Success")
            {
                ModelState.Clear();
                status = true;
                message = "City Successfully Deleted";
            }
            else
            {
                ModelState.Clear();
                status = false;
                message = returnId;
            }
            return new JsonResult { Data = new { status = status, message = message } };
        }
	}
}
