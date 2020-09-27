using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestMVC.Models;
using System.Data;
using System.Data.SqlClient;

namespace TestMVC.Controllers
{
    public class HomeController : Controller
    {
        TestDbEntities db = new TestDbEntities();
         
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllCountries()
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
                sorting = " Order by s.CountryId asc";
            }
            //if (!(string.IsNullOrEmpty(HallName)))
            //{
            //    whereCondition = " LOWER(s.HallName) like ('%" + HallName + "%')";
            //}
            //else
            //{
            //    whereCondition = " LOWER(s.HallName) like ('%%')";
            //}
            List<CountryViewModel> listsub = new List<CountryViewModel>();
            DataTableReader dtr = clsSqlCountry.getCountryListCount();
            while (dtr.Read())
            {
                recordsTotal = Convert.ToInt32(dtr["MyRowCount"]);
            }
            DataTableReader dt = clsSqlCountry.getCountryList(start, length, sorting);
            //     int i = 0;
            while (dt.Read())
            {
                listsub.Add(new CountryViewModel()
                {
                    CountryId = Convert.ToInt32(dt["CountryId"]),
                    CountryName = dt["CountryName"].ToString(),
                    RegionId = Convert.ToInt32(dt["RegionId"]),
                    RegionName = dt["RegionName"].ToString()                   
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //add & edit country
        public ActionResult InsertUpdateCountry(int id)
        {
            if (User != null)
            {
                CountryViewModel user = new CountryViewModel();
                if (id > 0)
                {
                    
                    user = (from f in db.tblCountry join r in db.tblRegion on f.RegionId equals r.RegionId
                            where f.CountryId == id 
                            select new CountryViewModel
                            {
                                CountryId = f.CountryId,
                                CountryName = f.CountryName,
                                RegionId = r.RegionId,
                                RegionName = db.tblRegion.Where(x=>x.RegionId == r.RegionId).Select(x=>x.RegionName).FirstOrDefault() ?? ""
                            }).FirstOrDefault();
                }
                else
                {
                    user.RegionId = 0;
                    user.CountryId = 0;
                    user.CountryName = "";
                    user.RegionName = "";
                }
                return PartialView(user);
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }
        [HttpPost]
        public ActionResult InsertUpdateCountry(CountryViewModel user)
        {
            string message = "";
            bool status = false;
            try
            {
                string returnId = "0";
                string insertUpdateStatus = "";
                int? CountryId = user.CountryId;
                if (user.CountryId > 0)
                {
                    insertUpdateStatus = "Update";
                }
                else
                {
                    insertUpdateStatus = "Save";
                }
                returnId = InsertUpdateCountryDb(user, insertUpdateStatus);
                if (returnId == "Success")
                {
                    ModelState.Clear();
                    status = true;
                    message = "Country Successfully Updated";
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
        private string InsertUpdateCountryDb(CountryViewModel st, string insertUpdateStatus)
        {
            string returnId = "0";
            string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connection))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateCountry", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@CountryId", SqlDbType.Int).Value = st.CountryId;
                        cmd.Parameters.Add("@RegionId", SqlDbType.Int).Value = st.RegionId;
                        cmd.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = st.CountryName;
                        cmd.Parameters.Add("@RegionName", SqlDbType.NVarChar).Value = st.RegionName;
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

            CountryViewModel st = new CountryViewModel();
            st.CountryId = id;

            st.CountryName = "";
            string returnId = InsertUpdateCountryDb(st, "Delete");
            if (returnId == "Success")
            {
                ModelState.Clear();
                status = true;
                message = "Country Successfully Deleted";
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