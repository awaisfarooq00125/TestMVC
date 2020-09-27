using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestMVC.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Script.Serialization;

namespace TestMVC.Controllers
{
    public class AllUsersController : Controller
    {
        //
        // GET: /AllUsers/

        private TestDbEntities objTestDb;

        public AllUsersController()
        {
            objTestDb = new TestDbEntities();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAllUsers()
        {
            List<UserViewModel> listUsers = new List<UserViewModel>();
            string cs = ConfigurationManager.ConnectionStrings["TestDbEntities"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("select * from tblUser", con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserViewModel user = new UserViewModel();
                    user.UserId = Convert.ToInt32(rdr["UserId"]);
                    user.FirstName = rdr["FirstName"].ToString();
                    user.LastName = rdr["LastName"].ToString();
                    user.Email = rdr["Email"].ToString();
                    user.UserName = rdr["UserName"].ToString();
                    listUsers.Add(user);
                }

            }
            
            return Json(listUsers, JsonRequestBehavior.AllowGet);
            //IEnumerable<UserViewModel> listUsers = (from objUsers in objTestDb.tblUser
                                                                           
            //                                                                select new UserViewModel()
            //                                                                {
            //                                                                   UserId = objUsers.UserId,
            //                                                                   FirstName = objUsers.FirstName,
            //                                                                   LastName = objUsers.LastName,
            //                                                                   Email = objUsers.Email,
            //                                                                   UserName = objUsers.UserName,
                                                                             
            //                                                                }).ToList();

            //return Json(listUsers,JsonRequestBehavior.AllowGet);
            
        }

	}
}