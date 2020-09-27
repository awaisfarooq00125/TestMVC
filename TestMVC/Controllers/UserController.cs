using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestMVC.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace TestMVC.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        private TestDbEntities objTestDb;
        public UserController()
        {
            objTestDb = new TestDbEntities();
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserViewModel user)
        {
            bool status = false;
            string message = "";


            var check = objTestDb.tblUser.Where(x => x.UserName.ToLower().Trim() == user.UserName.ToLower().Trim()
                && x.Password.ToLower().Trim() == user.Password.ToLower().Trim()).FirstOrDefault();
            if (check != null)
            {
                status = true;
                message = "success";
                HttpCookie userCookie = new HttpCookie("User", check.UserId.ToString());
                userCookie.Expires = DateTime.MaxValue;

                HttpContext.Response.SetCookie(userCookie);
                HttpCookie newCookie = Request.Cookies["User"];

                Session["UserId"] = check.UserId.ToString();
                Session["UserName"] = check.UserName.ToString();

            }
            else
            {
                message = "Login Failed";
                status = false;
            }
            return new JsonResult { Data = new { abc = status, abc2 = message } };

        }



        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(UserViewModel model)
        {
            string message = string.Empty;
            string userName = string.Empty;
            bool status = false;
            var checkUserName = objTestDb.tblUser.Where(x => x.UserName == model.UserName).FirstOrDefault();
            if (checkUserName == null)
            {
                status = true;
                tblUser user = new tblUser()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.UserName,
                    Password = model.Password,
                };
                objTestDb.tblUser.Add(user);
                message = "Added";
                objTestDb.SaveChanges();
            }
            else
            {
                status = false;
                userName = "User name already added";
            }
            return Json(new { message = "User successfully" + message, user = userName, UserStatus = status, success = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult logoff()
        {
            Session.Abandon();
            if (Request.Cookies["User"] != null)
            {
                Response.Cookies["User"].Expires = DateTime.Now.AddDays(-1);
            }
            return RedirectToAction("Login", "User");
        }
        public ActionResult List()
        {
            return View();
        }

        //public ActionResult GetList()
        //{
        //    IEnumerable<UserViewModel> list = (from objUser in objTestDb.tblUser

        //                                       select new UserViewModel()
        //                                                                    {
        //                                                                       FirstName =objUser.FirstName,
        //                                                                       LastName = objUser.LastName,
        //                                                                       Email = objUser.Email,
        //                                                                       UserName = objUser.UserName
        //                                                                    }).ToList();
        //    return Json(list,JsonRequestBehavior.AllowGet);

        //    //return View(list);
        //}
        public ActionResult GetList()
        {
            List<UserViewModel> list = new List<UserViewModel>();
            string cs = ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spGetAllUsers", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserViewModel model = new UserViewModel();
                    model.FirstName = rdr["FirstName"].ToString();
                    model.LastName = rdr["LastName"].ToString();
                    model.Email = rdr["Email"].ToString();
                    model.UserName = rdr["UserName"].ToString();

                    list.Add(model);
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }



    }
}