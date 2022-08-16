using DispatchService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Services;
using System.Globalization;

namespace DispatchService.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {        
        public ActionResult Main()
        {            
            ViewBag.EndDate = DateTime.Now.ToString("d", new CultureInfo("ru-RU"));
            ViewBag.StartDate = DateTime.Now.AddDays(-30).ToString("d", new CultureInfo("ru-RU"));

            User current_user = null;
            using (HCSContext db = new HCSContext())
            {                
                current_user = db.Users.Include(r => r.Role).FirstOrDefault(u => u.login == User.Identity.Name);
                if (current_user != null)
                {
                    ViewBag.User = current_user;
                }

                return View();
            }
        }

        public ActionResult ShowRequest(int id)
        {
            List<RequestViewModel> req;
            User current_user = null;
            using (HCSContext db = new HCSContext())
            {
                req = (from division in db.Divisions
                       join street in db.Streets on division.division_id equals street.division_id
                       join address in db.Addresses on street.street_id equals address.street_id
                       join request in db.Requests on address.address_id equals request.address_id
                       join category in db.Categories on request.category_id equals category.category_id
                       join type in db.Types on request.type_id equals type.type_id
                       join request_step in db.RequestSteps on request.request_id equals request_step.request_id
                       join step in db.Steps on request_step.step_id equals step.step_id
                       join user in db.Users on request_step.user_id equals user.user_id
                       join role in db.Roles on user.role_id equals role.role_id
                       where request.request_id == id
                       select new RequestViewModel {
                           request_id = request.request_id,
                           division = division.division_name,
                           street = street.street_name,
                           house = address.house,
                           flat = address.flat,
                           category = category.category_name,
                           type = type.type_name,
                           description = request.description,
                           date_begin = request.date_begin,
                           date_end = request.date_end,
                           due_date = request.due_date,
                           step_name = step.step_name,
                           step_date_begin = request_step.date_begin,
                           step_date_end = request_step.date_end,
                           implemented_by = user.user_name,
                           user_role = role.role_name,
                           comment = request_step.comment
                       }).ToList();

                req.ElementAt(0).due_date_days = (int)req.ElementAt(0).due_date.Subtract(req.ElementAt(0).date_begin).TotalDays;                

                current_user = db.Users.Include(r => r.Role).FirstOrDefault(u => u.login == User.Identity.Name);
                if (current_user != null)
                {
                    ViewBag.User = current_user;
                }
            }
            ViewBag.RequestId = id.ToString();            

            return View(req);
        }

        [HttpGet]
        public ActionResult CreateRequest(string streetName, string houseNumber, string flatNumber)
        {
            User current_user;
            RequestViewModel reqVM = new RequestViewModel();
            reqVM.street = streetName;
            reqVM.house = houseNumber;
            reqVM.flat = flatNumber;
            reqVM.due_date_days = 1;

            using (HCSContext db = new HCSContext())
            {
                var categories = db.Categories.OrderBy(c => c.category_name).ToList();
                var types = db.Types.OrderBy(t => t.type_name).ToList();

                ViewBag.Types = types;
                ViewBag.Categories = categories;

                reqVM.category = categories.ElementAt(0).category_name;
                reqVM.type = types.ElementAt(0).type_name;

                current_user = db.Users.Include(r => r.Role).FirstOrDefault(u => u.login == User.Identity.Name);
                if (current_user != null)
                {
                    ViewBag.User = current_user;
                }
            }

            return View(reqVM);
        }

        [HttpPost]
        public ActionResult CreateRequest(string street, string house, string flat, int category, int type, string description, /*DateTime datepicker,*/ int days, string comment)
        {
            Request req = new Request();
            RequestStep req_step = new RequestStep();

            using (HCSContext db = new HCSContext())
            {
                var addressId = db.Addresses.Include(s => s.Street).FirstOrDefault(a => a.Street.street_name == street
                                                                                        && a.house == house
                                                                                        && a.flat == flat);
                if (addressId != null)
                {
                    req.address_id = addressId.address_id;
                } else
                {
                    ViewBag.Message = "Такого адреса не существует";
                    RequestViewModel reqVM = new RequestViewModel();
                    reqVM.street = street;
                    reqVM.house = house;
                    reqVM.flat = flat;
                    reqVM.category = db.Categories.Find(category).category_name;
                    reqVM.type = db.Types.Find(type).type_name;
                    reqVM.description = description;
                    reqVM.due_date_days = days;
                    reqVM.comment = comment;

                    var categories = db.Categories.OrderBy(c => c.category_name).ToList();
                    var types = db.Types.OrderBy(t => t.type_name).ToList();

                    ViewBag.Types = types;
                    ViewBag.Categories = categories;

                    User current_user = db.Users.Include(r => r.Role).FirstOrDefault(u => u.login == User.Identity.Name);
                    if (current_user != null)
                    {
                        ViewBag.User = current_user;
                    }

                    return View(reqVM);
                }

                req.category_id = Convert.ToInt32(category);
                req.type_id = Convert.ToInt32(type);
                req.description = description;

                DateTime now = DateTime.Now;
                req.date_begin = now;

                //datepicker = datepicker.Date + now.TimeOfDay;
                //req.due_date = datepicker;
                req.due_date = now.AddDays(days);

                db.Requests.Add(req);
                db.SaveChanges();                

                req_step.Request = req;
                req_step.step_id = 1;
                req_step.User = db.Users.FirstOrDefault(u => u.login == User.Identity.Name);
                req_step.date_begin = now;
                req_step.comment = comment;

                db.RequestSteps.Add(req_step);
                db.SaveChanges();
            }

            return RedirectToAction("ShowRequest", new { id = req.request_id });
        }

        [HttpGet]
        public ActionResult EditRequest(int id)
        {
            using (HCSContext db = new HCSContext())
            {                
                RequestViewModel req =  (RequestViewModel)(from division in db.Divisions
                           join street in db.Streets on division.division_id equals street.division_id
                           join address in db.Addresses on street.street_id equals address.street_id
                           join request in db.Requests on address.address_id equals request.address_id
                           join category in db.Categories on request.category_id equals category.category_id
                           join type in db.Types on request.type_id equals type.type_id
                           where request.request_id == id
                           select new RequestViewModel
                           {
                               request_id = request.request_id,
                               division = division.division_name,
                               street = street.street_name,
                               house = address.house,
                               flat = address.flat,
                               category = category.category_name,
                               type = type.type_name,
                               description = request.description,
                               date_begin = request.date_begin,
                               due_date = request.due_date
                           }).FirstOrDefault();
                req.due_date_days = (int)req.due_date.Subtract(req.date_begin).TotalDays;

                var categories = db.Categories.OrderBy(c => c.category_name).ToList();
                var types = db.Types.OrderBy(t => t.type_name).ToList();

                ViewBag.Types = types;
                ViewBag.Categories = categories;

                User current_user = db.Users.Include(r => r.Role).FirstOrDefault(u => u.login == User.Identity.Name);
                if (current_user != null)
                {
                    ViewBag.User = current_user;
                }

                return View(req);
            }            
        }

        [HttpPost]
        public ActionResult EditRequest(int id, string street, string house, string flat, int category, int type, string description, int days)
        {
            using (HCSContext db = new HCSContext())
            {
                Request req = db.Requests.FirstOrDefault(r => r.request_id == id);
                var addressId = db.Addresses.Include(s => s.Street).FirstOrDefault(a => a.Street.street_name == street
                                                                                        && a.house == house
                                                                                        && a.flat == flat);
                if (addressId != null)
                {
                    req.address_id = addressId.address_id;
                } else
                {
                    ViewBag.Message = "Такого адреса не существует";
                    RequestViewModel reqVM = new RequestViewModel();
                    reqVM.request_id = id;
                    reqVM.street = street;
                    reqVM.house = house;
                    reqVM.flat = flat;
                    reqVM.category = db.Categories.Find(category).category_name;
                    reqVM.type = db.Types.Find(type).type_name;
                    reqVM.description = description;
                    reqVM.due_date_days = days;

                    var categories = db.Categories.OrderBy(c => c.category_name).ToList();
                    var types = db.Types.OrderBy(t => t.type_name).ToList();

                    ViewBag.Types = types;
                    ViewBag.Categories = categories;

                    User current_user = db.Users.Include(r => r.Role).FirstOrDefault(u => u.login == User.Identity.Name);
                    if (current_user != null)
                    {
                        ViewBag.User = current_user;
                    }

                    return View(reqVM);
                } 

                req.category_id = Convert.ToInt32(category);
                req.type_id = Convert.ToInt32(type);
                req.description = description;
                req.due_date = req.date_begin.AddDays(days);

                db.Entry(req).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("ShowRequest", new { id });
        }

        public ActionResult DeleteRequest(int id)
        {
            using (HCSContext db = new HCSContext())
            {
                Request req = db.Requests.Find(id);
                if (req != null)
                {
                    db.Requests.Remove(req);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Main");
        }

        public ActionResult SearchRequests(SearchModel sm, int pageIndex = 1)
        {
            for (int i = 0; i < sm.type.Length; i++)
            {
                if (sm.type[i] == "emerge")
                    sm.type[i] = "Аварийная";
                else if (sm.type[i] == "request")
                    sm.type[i] = "Типовая";
                else if (sm.type[i] == "claim")
                    sm.type[i] = "Жалоба";
            }

            sm.endDate = sm.endDate.AddDays(1);

            if (sm.startDate == DateTime.MinValue)
            {
                sm.startDate = new DateTime(1999, 12, 3);
            }
            if (sm.endDate == DateTime.MinValue.AddDays(1))
            {
                sm.endDate = DateTime.Now;
            }

            using (HCSContext db = new HCSContext())
            {     
                PaginationModel pagModel = new PaginationModel();
                pagModel.PageIndex = pageIndex;
                pagModel.PageSize = 6;
                pagModel.RecordCount = (from division in db.Divisions
                                        join street in db.Streets on division.division_id equals street.division_id
                                        join address in db.Addresses on street.street_id equals address.street_id
                                        join request in db.Requests on address.address_id equals request.address_id
                                        join category in db.Categories on request.category_id equals category.category_id
                                        join type in db.Types on request.type_id equals type.type_id
                                        join request_step in db.RequestSteps on request.request_id equals request_step.request_id
                                        where sm.type.Contains(type.type_name)
                                            && (division.division_name == sm.division || sm.division == null)
                                            && (street.street_name == sm.street || sm.street == null)
                                            && (address.house == sm.house || sm.house == null)
                                            && (address.flat == sm.flat || sm.flat == null)
                                            && (category.category_name == sm.category || sm.category == null)
                                            && (request.date_begin >= sm.startDate && request.date_begin <= sm.endDate)
                                            && (!sm.status.Contains("overdue") || (sm.status.Contains("overdue") && ((request.date_end == null && request.due_date < DateTime.Now)) || (request.date_end != null && request.due_date < request.date_end)))
                                            && request_step.step_id == 1 && ((sm.status.Contains("ended") && request.date_end != null)
                                                 || (sm.status.Contains("new") && request_step.step_id == 1 && request_step.date_end == null)
                                                 || (sm.status.Contains("in_process") && request_step.date_end != null && request.date_end == null))
                                        select request.request_id).Count();
                pagModel.PageTotal = (pagModel.RecordCount / pagModel.PageSize) + ((pagModel.RecordCount % pagModel.PageSize) > 0 ? 1 : 0);                

                var requests = (from division in db.Divisions
                                join street in db.Streets on division.division_id equals street.division_id
                                join address in db.Addresses on street.street_id equals address.street_id
                                join request in db.Requests on address.address_id equals request.address_id
                                join category in db.Categories on request.category_id equals category.category_id
                                join type in db.Types on request.type_id equals type.type_id
                                join request_step in db.RequestSteps on request.request_id equals request_step.request_id
                                where sm.type.Contains(type.type_name)
                                    && (division.division_name == sm.division || sm.division == null)
                                    && (street.street_name == sm.street || sm.street == null)
                                    && (address.house == sm.house || sm.house == null)
                                    && (address.flat == sm.flat || sm.flat == null)
                                    && (category.category_name == sm.category || sm.category == null)
                                    && (request.date_begin >= sm.startDate && request.date_begin <= sm.endDate)
                                    && (!sm.status.Contains("overdue") || (sm.status.Contains("overdue") && ((request.date_end == null && request.due_date < DateTime.Now)) || (request.date_end != null && request.due_date < request.date_end)))
                                    && request_step.step_id == 1 && ((sm.status.Contains("ended") && request.date_end != null)
                                         || (sm.status.Contains("new") && request_step.step_id == 1 && request_step.date_end == null)
                                         || (sm.status.Contains("in_process") && request_step.date_end != null && request.date_end == null))
                                orderby request.request_id
                                select new RequestViewModel
                                {
                                    request_id = request.request_id,
                                    division = division.division_name,
                                    street = street.street_name,
                                    house = address.house,
                                    flat = address.flat,
                                    category = category.category_name,
                                    type = type.type_name,
                                    description = request.description,
                                    date_begin = request.date_begin,
                                    date_end = request.date_end,
                                    due_date = request.due_date
                                }).Skip((pageIndex - 1) * pagModel.PageSize).Take(pagModel.PageSize).ToList();  

                for (int i = 0; i < requests.Count; i++)
                {
                    if (requests[i].date_end != null)
                    {
                        DateTime end = (DateTime)requests[i].date_end;
                        requests[i].days_in_work = (int)Math.Ceiling(end.Subtract(requests[i].date_begin).TotalDays);
                    } else
                    {
                        requests[i].days_in_work = (int)Math.Ceiling(DateTime.Now.Subtract(requests[i].date_begin).TotalDays);
                    }
                    //requests[i].time_remain = requests[i].due_date.Subtract(requests[i].date_begin);
                    int due_date_days = (int)requests[i].due_date.Subtract(requests[i].date_begin).TotalDays;

                    if (requests[i].date_end.HasValue)
                    {
                        requests[i].color = "background: #DAEBDA;";
                    }
                    else if (due_date_days < requests[i].days_in_work)
                    {
                        requests[i].color = "background: #F7CBCB;";
                    }
                    else if (due_date_days - 1 <= requests[i].days_in_work)
                    {
                        requests[i].color = "background: #F5F1CC;";
                    }
                    else
                    {
                        requests[i].color = "background: white;";
                    }

                    if (requests[i].type == "Аварийная")
                    {
                        requests[i].border = "border: 3px solid #e50000;";
                    }
                }

                pagModel.Requests = requests;
                return Json(pagModel, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AutocompleteSearch(string prefix, string element_id, string divisionName, string streetName, string houseNumber)
        {
            HCSContext db = new HCSContext();

            List<AutocompleteModel> name = new List<AutocompleteModel>();

            if (element_id == "division")
            {
                name = (from division in db.Divisions
                        where division.division_name.StartsWith(prefix)
                        select new AutocompleteModel
                        {
                            name = division.division_name,
                            id = division.division_id
                        }).OrderBy(d => d.name).ToList();
            } 
            else if (element_id == "street")
            {
                name = (from street in db.Streets
                        join division in db.Divisions on street.division_id equals division.division_id
                        where street.street_name.StartsWith(prefix) && (division.division_name == divisionName || divisionName == "")                        
                        select new AutocompleteModel
                        {
                            name = street.street_name,
                            id = street.street_id
                        }).OrderBy(s => s.name).ToList();
            } 
            else if (element_id == "house")
            {
                name = (from address in db.Addresses
                        join street in db.Streets on address.street_id equals street.street_id
                        where address.house.StartsWith(prefix) && street.street_name == streetName                        
                        select new AutocompleteModel
                        {
                            name = address.house
                        }).Distinct().OrderBy(h => h.name.Length).ThenBy(h => h.name).ToList();
            } 
            else if (element_id == "flat") {
                name = (from address in db.Addresses
                        join street in db.Streets on address.street_id equals street.street_id
                        where address.flat.StartsWith(prefix) && street.street_name == streetName && address.house == houseNumber                        
                        select new AutocompleteModel
                        {
                            name = address.flat
                        }).OrderBy(f => f.name.Length).ThenBy(f => f.name).ToList();
            }
            else if (element_id == "category")
            {
                name = (from category in db.Categories
                        where category.category_name.StartsWith(prefix)
                        select new AutocompleteModel
                        {
                            name = category.category_name,
                            id = category.category_id
                        }).OrderBy(c => c.name).ToList();
            }

            return new JsonResult { Data = name, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult AddStep(int id)
        {            
            using (HCSContext db = new HCSContext())
            {
                //var steps = db.Steps.Where(s => s.step_id != 1).ToList();
                //ViewBag.Steps = steps;
                ViewBag.RequestId = id;
                return PartialView();
            }            
        }

        [HttpPost]
        public ActionResult AddStep(int id, int days, string comment, int stepId = 2)
        {
            RequestStep rs = new RequestStep();
            DateTime now = DateTime.Now;
            rs.date_begin = now;
            rs.comment = comment;
            rs.request_id = id;
            rs.step_id = stepId;

            using (HCSContext db = new HCSContext())
            {
                RequestStep last_step = db.RequestSteps.OrderByDescending(r => r.request_step_id).FirstOrDefault(s => s.request_id == id);
                last_step.date_end = DateTime.Now;
                User current_user = db.Users.FirstOrDefault(u => u.login == User.Identity.Name);
                rs.user_id = current_user.user_id;
                Step step = db.Steps.FirstOrDefault(s => s.step_id == stepId);
                if (step.step_name == "В работе")
                {
                    Request req = db.Requests.FirstOrDefault(r => r.request_id == id);
                    req.due_date = req.due_date.AddDays(days);
                }
                if (step.step_name == "Завершено")
                {
                    Request req = db.Requests.FirstOrDefault(r => r.request_id == id);
                    req.date_end = now;
                    rs.date_end = now;
                }

                db.RequestSteps.Add(rs);
                db.SaveChanges();
            }

            return RedirectToAction("ShowRequest", new { id });
        }
    }
}