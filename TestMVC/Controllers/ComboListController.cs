using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestMVC.Models;

namespace TestMVC.Controllers
{
    public class ComboListController : Controller
    {
        TestDbEntities db = new TestDbEntities();

        IQueryable<clsList> AllItemsList;
        #region Region
        public JsonResult GetRegionOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsList = AllRegionListDetail();
            var select2pagedResult = new Select2PagedResult();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;

            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public IQueryable<clsList> AllRegionListDetail()
        {
            //string cacheKey = "Select2Options";
            ////check cache 
            //if (System.Web.HttpContext.Current.Cache[cacheKey] != null)
            //{
            //    return (IQueryable<clsList>)System.Web.HttpContext.Current.Cache[cacheKey];
            //}

            List<clsList> item = new List<clsList>();
            item = (from c in db.tblRegion
                    orderby c.RegionName
                    select new clsList
                    {
                        id = c.RegionId,
                        text = c.RegionName
                    }).ToList();

            var result = item.AsQueryable();

            //cache results
            //System.Web.HttpContext.Current.Cache[cacheKey] = result;

            return result;
        }
        #endregion

        List<clsList> GetPagedListOptions(string searchTerm, int pageSize, int pageNumber, out int totalSearchRecords)
        {
            var allSearchedResults = GetAllSearchResults(searchTerm);
            totalSearchRecords = allSearchedResults.Count;
            return allSearchedResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        List<clsList> GetAllSearchResults(string searchTerm)
        {
            //AllItemsList = AllItemsListDetail();
            var resultList = new List<clsList>();

            if (!string.IsNullOrEmpty(searchTerm))
                resultList = AllItemsList.Where(n => n.text.ToLower().Contains(searchTerm.ToLower())).ToList();
            else
                resultList = AllItemsList.ToList();
            return resultList;
        }
	}
}