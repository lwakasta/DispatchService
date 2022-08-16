using DispatchService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace DispatchService.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class AdminController : Controller
    {
        public ActionResult Main()
        {
            User current_user = null;
            using (HCSContext db = new HCSContext())
            {
                current_user = db.Users.Include(r => r.Role).FirstOrDefault(u => u.login == User.Identity.Name);
                if (current_user != null)
                {
                    ViewBag.User = current_user;
                }

                var users = db.Users.Include(r => r.Role).Include(d => d.Department).ToList();
                ViewBag.Users = users;

                IEnumerable<AdminAddressModel> adm = (from street in db.Streets
                                                      orderby street.street_name
                                                      select new AdminAddressModel
                                                      {
                                                          street = street.street_name
                                                      }).ToList();
                foreach (var street in adm)
                {
                    street.Houses = db.Addresses
                        .Include(a => a.Street)
                        .Where(a => a.Street.street_name == street.street) 
                        .GroupBy(a => a.house)
                        .Select(a => new HouseModel { house = a.Key, flats = a.OrderBy(b => b.flat.Length).ThenBy(b => b.flat).Select(b => b.flat).ToList() })
                        .OrderBy(a => a.house.Length).ThenBy(a => a.house).ToList(); 
                }

                ViewBag.Addresses = adm;

                ViewBag.Division = db.Divisions.FirstOrDefault().division_name;
                ViewBag.Categories = db.Categories.ToList();
                return View();
            }
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            using (HCSContext db = new HCSContext())
            {
                ViewBag.Roles = db.Roles.OrderBy(n => n.role_name).ToList();
                ViewBag.Departments = db.Departments.OrderBy(n => n.department_name).ToList();
            }
            return PartialView();
        }

        [HttpPost]
        public ActionResult CreateUser(User user_vm)
        {
            using (HCSContext db = new HCSContext())
            {
                db.Users.Add(user_vm);
                db.SaveChanges();
            }
            return RedirectToAction("Main");
        }

        [HttpGet]
        public ActionResult EditUser(int id)
        {
            using (HCSContext db = new HCSContext())
            {
                User user = db.Users.FirstOrDefault(u => u.user_id == id);
                ViewBag.Roles = db.Roles.OrderBy(n => n.role_name).ToList();
                ViewBag.Departments = db.Departments.OrderBy(n => n.department_name).ToList();

                User current_user = db.Users.Include(r => r.Role).FirstOrDefault(u => u.login == User.Identity.Name);
                if (current_user != null)
                {
                    ViewBag.User = current_user;
                }

                return View(user);
            }            
        }

        [HttpPost]
        public ActionResult EditUser(int id, string user_name, int role_id, int department_id)
        {
            using (HCSContext db = new HCSContext())
            {
                User user = db.Users.Find(id);
                user.user_name = user_name;
                user.role_id = role_id;
                user.department_id = department_id;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Main");
        }

        public ActionResult DeleteUser(int id)
        {
            using (HCSContext db = new HCSContext())
            {
                User user = db.Users.Find(id);
                if (user != null)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Main");
        }

        public ActionResult AddAddress(string divisionName = "", string streetName = "", string houseNumber = "", string flatNumber = "", int flatFrom = 0, int flatTo = 0)
        {
            if ((divisionName != "") && (streetName != "") && (houseNumber != "") && ((flatNumber != "") || (flatFrom != 0 && flatTo != 0)))
            {
                Address address_check = null;
                Street street_check = null;

                using (HCSContext db = new HCSContext())
                {
                    street_check = db.Streets.FirstOrDefault(s => s.street_name == streetName);
                    if (street_check == null)
                    {
                        Street street = new Street();
                        street.street_name = streetName;
                        street.division_id = db.Divisions.FirstOrDefault(d => d.division_name == divisionName).division_id;
                        db.Streets.Add(street);
                        db.SaveChanges();
                    }

                    if (flatNumber != "")
                    {
                        address_check = db.Addresses.Include(s => s.Street).Where(a => a.Street.street_name == streetName && a.house == houseNumber && a.flat == flatNumber).FirstOrDefault();
                        if (address_check == null)
                        {
                            Street street = db.Streets.FirstOrDefault(s => s.street_name == streetName);
                            Address address = new Address();
                            address.street_id = street.street_id;
                            address.house = houseNumber;
                            address.flat = flatNumber;
                            db.Addresses.Add(address);
                            db.SaveChanges();
                        }
                    }
                    else if (flatFrom != 0 && flatTo != 0)
                    {
                        for (int i = flatFrom; i <= flatTo; i++)
                        {
                            address_check = db.Addresses.Include(s => s.Street).Where(a => a.Street.street_name == streetName && a.house == houseNumber && a.flat == i.ToString()).FirstOrDefault();
                            if (address_check == null)
                            {
                                Street street = db.Streets.FirstOrDefault(s => s.street_name == streetName);
                                Address address = new Address();
                                address.street_id = street.street_id;
                                address.house = houseNumber;
                                address.flat = i.ToString();
                                db.Addresses.Add(address);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Main");
        }

        [HttpGet]
        public ActionResult CreateCategory()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult CreateCategory(Category cat_vm)
        {
            using (HCSContext db = new HCSContext())
            {
                db.Categories.Add(cat_vm);
                db.SaveChanges();
            }
            return RedirectToAction("Main");
        }

        [HttpGet]
        public ActionResult EditCategory(int id)
        {
            using (HCSContext db = new HCSContext())
            {
                Category cat = db.Categories.FirstOrDefault(c => c.category_id == id);

                User current_user = db.Users.Include(r => r.Role).FirstOrDefault(u => u.login == User.Identity.Name);
                if (current_user != null)
                {
                    ViewBag.User = current_user;
                }

                return View(cat);
            }
        }

        [HttpPost]
        public ActionResult EditCategory(int id, string category_name)
        {
            using (HCSContext db = new HCSContext())
            {
                Category cat = db.Categories.Find(id);
                cat.category_name = category_name;
                db.Entry(cat).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Main");
        }

        public ActionResult DeleteCategory(int id)
        {
            using (HCSContext db = new HCSContext())
            {
                Category cat = db.Categories.Find(id);
                if (cat != null)
                {
                    db.Categories.Remove(cat);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Main");
        }
    }
}