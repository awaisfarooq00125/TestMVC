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
    public class RegionController : Controller
    {

        TestDbEntities db = new TestDbEntities();
        public ActionResult Index()
        {
           
            return View();
        }
        public ActionResult GetAllRegions()
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
                sorting = " Order by s.RegionId asc";
            }
            //if (!(string.IsNullOrEmpty(HallName)))
            //{
            //    whereCondition = " LOWER(s.HallName) like ('%" + HallName + "%')";
            //}
            //else
            //{
            //    whereCondition = " LOWER(s.HallName) like ('%%')";
            //}
            List<RegionViewModel> listsub = new List<RegionViewModel>();
            DataTableReader dtr = clsSqlRegion.getRegionListCount();
            while (dtr.Read())
            {
                recordsTotal = Convert.ToInt32(dtr["MyRowCount"]);
            }
            DataTableReader dt = clsSqlRegion.getRegionList(start, length, sorting);
            //     int i = 0;
            while (dt.Read())
            {
                listsub.Add(new RegionViewModel()
                {
                  
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

        

        //add & edit country
        public ActionResult InsertUpdateRegion(int id)
        {
            if (User != null)
            {
                RegionViewModel user = new RegionViewModel();
                if (id > 0)
                {

                    user = (from r in db.tblRegion
                            where r.RegionId == id
                            select new RegionViewModel
                            {
                               
                                RegionId = r.RegionId,
                                RegionName = db.tblRegion.Where(x => x.RegionId == r.RegionId).Select(x => x.RegionName).FirstOrDefault() ?? ""
                            }).FirstOrDefault();
                }
                else
                {
                    user.RegionId = 0;
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
        public ActionResult InsertUpdateRegion(RegionViewModel user)
        {
            string message = "";
            bool status = false;
            try
            {
                string returnId = "0";
                string insertUpdateStatus = "";
                int? RegionId = user.RegionId;
                if (user.RegionId > 0)
                {
                    insertUpdateStatus = "Update";
                }
                else
                {
                    insertUpdateStatus = "Save";
                }
                returnId = InsertUpdateRegionDb(user, insertUpdateStatus);
                if (returnId == "Success")
                {
                    ModelState.Clear();
                    status = true;
                    message = "Region Successfully Updated";
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
        private string InsertUpdateRegionDb(RegionViewModel st, string insertUpdateStatus)
        {
            string returnId = "0";
            string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connection))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateRegion", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@RegionId", SqlDbType.Int).Value = st.RegionId;              
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

            RegionViewModel st = new RegionViewModel();
            st.RegionId = id;

            st.RegionName = "";
            string returnId = InsertUpdateRegionDb(st, "Delete");
            if (returnId == "Success")
            {
                ModelState.Clear();
                status = true;
                message = "Region Successfully Deleted";
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