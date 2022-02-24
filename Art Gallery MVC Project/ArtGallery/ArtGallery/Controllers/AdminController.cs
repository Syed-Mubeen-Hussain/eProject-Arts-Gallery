using ArtGallery.Models;
using ArtGallery.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace adtGallery.Controllers
{
    public class AdminController : Controller
    {
        db_ArtGalleryEntities db = new db_ArtGalleryEntities();

        #region Dashboard
        public ActionResult Dashboard()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            var Arts = db.Arts.ToList();
            Session["Total_Arts"] = Arts.Count;
            var Active_Arts = db.Arts.Where(x => x.art_status == 1).ToList();
            Session["Active_Arts"] = Active_Arts.Count;
            var FixedPrice_Arts = db.Arts.Where(x => x.art_isAuction == false && x.art_status == 1).ToList();
            Session["FixedPrice_Arts"] = FixedPrice_Arts.Count;
            var Auction_Arts = db.Arts.Where(x => x.art_isAuction == true && x.art_status == 1).ToList();
            Session["Auction_Arts"] = Auction_Arts.Count;
            var Artist = db.Artists.ToList();
            Session["Artist"] = Artist.Count;
            var Customer = db.Customers.ToList();
            Session["Customer"] = Customer.Count;
            var Categories = db.Categories.Where(x=>x.cat_status == 1).ToList();
            Session["Categories"] = Categories.Count;
            var Orders = db.orders.ToList();
            Session["Orders"] = Orders.Count;
            var WebsiteFund = db.Website_fund.ToList() != null ? db.Website_fund.Sum(x => x.f_amount) : 0;
            Session["TotalAmount"] = WebsiteFund;
            var Exhibition = db.Exhibitions.Where(x => x.e_status == 1).Count();
            Session["Exhibition"] = Exhibition;
            return View();
        }
        #endregion

        #region Categories
        public ActionResult Category()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        
        public ActionResult CategoryTable(string search, int? pageNo)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            int pageSize = 5;

            AdminCategoryViewModel model = new AdminCategoryViewModel();

            pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

            model.pageNo = pageNo.Value;

            var category = db.Categories.ToList();

            var totalcategory = db.Categories.Count();

            if (!string.IsNullOrEmpty(search))
            {
                category = category.Where(x => x.cat_name.ToLower().Contains(search.Trim().ToLower())).ToList();
                model.SearchTerm = search;
                totalcategory = category.Count();
            }

            model.categories = category.OrderByDescending(x => x.cat_id).Skip((pageNo.Value - 1) * pageSize).Take(pageSize).ToList();

            model.page = new Pager(totalcategory, pageNo, pageSize);

            return PartialView(model);
        }
        #endregion

        #region Create Category
        public ActionResult CreateCategory()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }

            return PartialView();
        }

        [HttpPost]
        public ActionResult CreateCategory(Category cat)
        {
            if (cat != null)
            {
                Category c = new Category();
                c.cat_name = cat.cat_name;
                c.cat_status = 1;

                if (string.IsNullOrEmpty(cat.cat_image))
                {
                    c.cat_image = "/Content/images/no-image.png";
                }
                else
                {
                    c.cat_image = cat.cat_image;
                }

                db.Categories.Add(c);
                db.SaveChanges();
            }

            return RedirectToAction("CategoryTable");
        }
        #endregion

        #region Edit Category
        public ActionResult EditCategory(int Id)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            if (Id > 0)
            {
                var data = db.Categories.Where(x => x.cat_id == Id).FirstOrDefault();
                return PartialView(data);
            }
            return RedirectToAction("CategoryTable");
        }

        [HttpPost]
        public ActionResult EditCategory(Category cat)
        {
            var data = db.Categories.Where(x => x.cat_id == cat.cat_id).FirstOrDefault();

            if (data != null)
            {
                data.cat_name = cat.cat_name;
                data.cat_status = cat.cat_status;
                if (!string.IsNullOrEmpty(cat.cat_image))
                {
                    data.cat_image = cat.cat_image;
                }

                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("CategoryTable");
        }
        #endregion

        #region Delete Category
        [HttpPost]
        public ActionResult DeleteCategory(int Id)
        {
            if (Id > 0)
            {
                var data = db.Categories.Where(x => x.cat_id == Id).FirstOrDefault();
                if (data != null)
                {
                    data.cat_status = 0;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("CategoryTable");
        }
        #endregion

        #region Details Category
        public ActionResult DetailsCategory(int Id)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            var data = db.Categories.Find(Id);
            return PartialView(data);
        }
        #endregion

        #region Login
        public ActionResult Login()
        {
            if (Session["ad_id"] != null)
            {
                return RedirectToAction("Dashboard");
            }
            HttpCookie cookie = Request.Cookies["Admin_RememberMe"];
            if (cookie != null)
            {
                ViewBag.username = cookie["username"].ToString();

                string EncryptedPassword = cookie["password"].ToString();
                byte[] b = Convert.FromBase64String(EncryptedPassword);
                string decryptedPassword = ASCIIEncoding.ASCII.GetString(b);

                ViewBag.password = decryptedPassword.ToString();
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(AdminLoginViewModel ad)
        {
            AdminLoginViewModel model = new AdminLoginViewModel();
            if (ModelState.IsValid)
            {
                HttpCookie cookie = new HttpCookie("Admin_RememberMe");

                if (ad.RememberMe)
                {
                    cookie["username"] = ad.ad_userName.ToString();

                    byte[] b = ASCIIEncoding.ASCII.GetBytes(ad.ad_password);
                    string EncryptedPassword = Convert.ToBase64String(b);
                    cookie["password"] = EncryptedPassword;

                    HttpContext.Response.Cookies.Add(cookie);
                    cookie.Expires = DateTime.Now.AddDays(2);
                }
                else
                {
                    HttpContext.Response.Cookies.Add(cookie);
                    cookie.Expires = DateTime.Now.AddDays(-1);
                }
                var data = db.Admins.Where(x => x.ad_username.ToLower().Trim() == ad.ad_userName.ToLower().Trim() && x.ad_password.ToLower().Trim() == ad.ad_password.ToLower().Trim()).FirstOrDefault();
                if (data != null)
                {
                    Session["ad_id"] = data.ad_id;
                    Session["ad_username"] = data.ad_username.ToString();
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    ViewBag.error = "Invalid Username or Password";
                }
            }
            return View();
        }
        #endregion

        #region Sign Out
        public ActionResult SignOut()
        {
            Session["ad_id"] = null;
            Session["ad_username"] = null;
            return RedirectToAction("Login");
        }
        #endregion

        #region Arts
        public ActionResult Arts()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        
        public ActionResult ArtTable(string search, int? pageNo)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            int pageSize = 5;

            AdminArtViewModel model = new AdminArtViewModel();

            pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

            model.pageNo = pageNo.Value;

            model.Style_Of_Arts = db.Style_of_Art.ToList();

            var art = db.Arts.ToList();

            var totalcategory = db.Arts.Count();

            if (!string.IsNullOrEmpty(search))
            {
                art = art.Where(x => x.art_name.ToLower().Contains(search.Trim().ToLower())).ToList();
                model.SearchTerm = search;
                totalcategory = art.Count();
            }

            model.Arts = art.OrderByDescending(x => x.art_id).Skip((pageNo.Value - 1) * pageSize).Take(pageSize).ToList();

            model.page = new Pager(totalcategory, pageNo, pageSize);

            return PartialView(model);
        }
        #endregion

        #region Delete Art
        [HttpPost]
        public ActionResult DeleteArt(int Id)
        {
            if (Id > 0)
            {
                var data = db.Arts.Where(x => x.art_id == Id && x.art_status == 1).FirstOrDefault();
                if (data != null)
                {
                    data.art_status = 0;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ArtTable", "Admin");
        }
        #endregion

        #region Details Art
        public ActionResult DetailsArt(int Id)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            var data = db.Arts.Where(x => x.art_id == Id).FirstOrDefault();
            var styleOfArt = db.Style_of_Art.Where(x => x.soA_id == data.Art_fk_soA_id).FirstOrDefault();
            ViewBag.styleOfArt = styleOfArt;
            return PartialView(data);
        }
        #endregion

        #region Artist
        public ActionResult Artist()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        
        public ActionResult ArtistTable(string search, int? pageNo)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            int pageSize = 5;

            AdminArtistViewModel model = new AdminArtistViewModel();

            pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

            model.pageNo = pageNo.Value;

            var Artist = db.Artists.ToList();

            var totalartist = db.Artists.Count();

            if (!string.IsNullOrEmpty(search))
            {
                Artist = Artist.Where(x => x.ar_userName.ToLower().Contains(search.Trim().ToLower())).ToList();
                model.SearchTerm = search;
                totalartist = Artist.Count();
            }

            model.Artist = Artist.OrderByDescending(x => x.ar_id).Skip((pageNo.Value - 1) * pageSize).Take(pageSize).ToList();

            model.page = new Pager(totalartist, pageNo, pageSize);

            var country = db.Countries.ToList();
            ViewBag.Country = country;

            var city = db.Cities.ToList();
            ViewBag.City = city;

            return PartialView(model);
        }
        #endregion

        #region Details Artist
        public ActionResult DetailsArtist(int Id)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            var data = db.Artists.Find(Id);


            var country = db.Countries.ToList();
            ViewBag.Country = country;

            var city = db.Cities.ToList();
            ViewBag.City = city;

            return PartialView(data);
        }
        #endregion

        #region Order
        public ActionResult Order()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        
        public ActionResult OrderTable(string search, int? pageNo)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            int pageSize = 5;

            AdminOrderViewModel model = new AdminOrderViewModel();

            pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

            model.pageNo = pageNo.Value;

            var order = db.orders.ToList();

            var totalartist = db.orders.Count();

            if (!string.IsNullOrEmpty(search))
            {
                order = order.Where(x => x.Customer.c_userName.ToLower().Contains(search.Trim().ToLower())).ToList();
                model.SearchTerm = search;
                totalartist = order.Count();
            }
           
            model.websiteFund = db.Website_fund.ToList();

            model.Orders = order.OrderByDescending(x => x.o_id).Skip((pageNo.Value - 1) * pageSize).Take(pageSize).ToList();

            model.page = new Pager(totalartist, pageNo, pageSize);

            return PartialView(model);
        }
        #endregion

        #region Details Order
        public ActionResult DetailsOrder(int Id)
        {
            AdminOrder_detailsViewModel model = new AdminOrder_detailsViewModel();

            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            var order = db.orders.Where(x => x.o_id == Id).FirstOrDefault();
            model.Order_Details = db.order_details.Where(x => x.od_fk_o == Id).ToList(); ;
            model.Order = db.orders.Where(x => x.o_id == Id).FirstOrDefault();
            model.Customer = db.Customers.Where(x => x.c_id == order.o_fk_cus_id).FirstOrDefault();
            var country = db.Countries.ToList();
            ViewBag.Country = country;
            var city = db.Cities.ToList();
            ViewBag.City = city;

            return PartialView(model);
        }
        #endregion

        #region Change Status from Order
        public ActionResult ChangeStatus(int Id)
        {
            AdminOrderChangeStatusViewModel model = new AdminOrderChangeStatusViewModel();
            if (Id > 0)
            {
                model.Order = db.orders.Where(x => x.o_id == Id).FirstOrDefault();
                model.Customer = db.Customers.Where(x => x.c_id == model.Order.Customer.c_id).FirstOrDefault();
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ChangeStatus(int Id, string Status)
        {
            if (Id > 0 && !string.IsNullOrEmpty(Status))
            {
                var data = db.orders.Where(x => x.o_id == Id).FirstOrDefault();
                data.o_status = Status;
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();

                notification notification = new notification();
                notification.n_fk_o_id = data.o_id;
                notification.n_fk_c_id = data.o_fk_cus_id;
                notification.n_text = data.o_status;
                notification.n_date = DateTime.Now.ToString();  
                db.notifications.Add(notification);
                db.SaveChanges();

                return RedirectToAction("OrderTable", "Admin");
            }
            return RedirectToAction("OrderTable", "Admin");
        }
        #endregion

        #region Customers
        public ActionResult Customer()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        
        public ActionResult CustomerTable(string search, int? pageNo)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            int pageSize = 5;

            AdminCustomerViewModel model = new AdminCustomerViewModel();

            pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

            model.pageNo = pageNo.Value;

            var customer = db.Customers.ToList();

            var totalcategory = db.Customers.Count();

            if (!string.IsNullOrEmpty(search))
            {
                customer = customer.Where(x => x.c_userName.ToLower().Contains(search.Trim().ToLower())).ToList();
                model.SearchTerm = search;
                totalcategory = customer.Count();
            }

            model.Customers = customer.OrderByDescending(x => x.c_id).Skip((pageNo.Value - 1) * pageSize).Take(pageSize).ToList();

            model.page = new Pager(totalcategory, pageNo, pageSize);

            return PartialView(model);
        }
        #endregion

        #region Details Customers
        public ActionResult DetailsCustomer(int Id)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            var data = db.Customers.Find(Id);
            return PartialView(data);
        }
        #endregion

        #region Exhibition
        public ActionResult Exhibition()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        
        public ActionResult ExhibitionTable(string search, int? pageNo)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            int pageSize = 5;

            AdminExhibitionViewModel model = new AdminExhibitionViewModel();

            pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

            model.pageNo = pageNo.Value;

            var exhibition = db.Exhibitions.ToList();

            var totalcategory = db.Exhibitions.Count();

            if (!string.IsNullOrEmpty(search))
            {
                exhibition = exhibition.Where(x => x.e_name.ToLower().Contains(search.Trim().ToLower())).ToList();
                model.SearchTerm = search;
                totalcategory = exhibition.Count();
            }

            model.exhibitions = exhibition.OrderByDescending(x => x.e_id).Skip((pageNo.Value - 1) * pageSize).Take(pageSize).ToList();

            model.page = new Pager(totalcategory, pageNo, pageSize);

            return PartialView(model);
        }
        #endregion

        #region Create Exhibition
        public ActionResult CreateExhibition()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }

            return PartialView();
        }

        [HttpPost]
        public ActionResult CreateExhibition(Exhibition exhibition)
        {
            if (exhibition != null)
            {
                Exhibition e = new Exhibition();
                e.e_name = exhibition.e_name;
                e.e_start_date = DateTime.Now.ToString();
                e.e_end_date = DateTime.Now.AddDays(1).ToString();
                e.e_country = exhibition.e_country;
                e.e_city = exhibition.e_city;
                e.e_zip_code = exhibition.e_zip_code;
                e.e_status = 1;

                if (string.IsNullOrEmpty(exhibition.e_image))
                {
                    e.e_image = "/Content/images/no-image.png";
                }
                else
                {
                    e.e_image = exhibition.e_image;
                }

                db.Exhibitions.Add(e);
                db.SaveChanges();
            }

            return RedirectToAction("ExhibitionTable");
        }
        #endregion

        #region Edit Exhibition
        public ActionResult EditExhibition(int Id)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            if (Id > 0)
            {
                var data = db.Exhibitions.Where(x => x.e_id == Id).FirstOrDefault();
                return PartialView(data);
            }
            return RedirectToAction("ExhibitionTable");
        }

        [HttpPost]
        public ActionResult EditExhibition(Exhibition e)
        {
            var data = db.Exhibitions.Where(x => x.e_id == e.e_id).FirstOrDefault();

            if (data != null)
            {
                data.e_name = e.e_name;
                data.e_country = e.e_country;
                data.e_city = e.e_city;
                data.e_end_date = DateTime.Now.AddDays(1).ToString();
                data.e_status = e.e_status;
                data.e_zip_code = e.e_zip_code;
                if (!string.IsNullOrEmpty(e.e_image))
                {
                    data.e_image = e.e_image;
                }

                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("ExhibitionTable");
        }
        #endregion

        #region Delete Exhibition
        [HttpPost]
        public ActionResult DeleteExhibition(int Id)
        {
            if (Id > 0)
            {
                var data = db.Exhibitions.Where(x => x.e_id == Id).FirstOrDefault();
                if (data != null)
                {
                    data.e_status = 0;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ExhibitionTable");
        }
        #endregion

        #region Details Exhibition
        public ActionResult DetailsExhibition(int Id)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            var data = db.Exhibitions.Find(Id);
            return PartialView(data);
        }
        #endregion

        #region Art Exhibition
        public ActionResult ArtExhibition()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        
        public ActionResult ArtExhibitionTable(string search, int? pageNo)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            int pageSize = 5;

            AdminArtExhibitionViewModel model = new AdminArtExhibitionViewModel();

            pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

            model.pageNo = pageNo.Value;

            var Artexhibition = db.Art_exhibition.Where(x=>x.ae_status == 1).ToList();

            var totalcategory = db.Art_exhibition.Where(x=>x.ae_status == 1).Count();

            if (!string.IsNullOrEmpty(search))
            {
                Artexhibition = Artexhibition.Where(x => x.Art.art_name.ToLower().Contains(search.Trim().ToLower())).ToList();
                model.SearchTerm = search;
                totalcategory = Artexhibition.Count();
            }

            model.Art_exhibitions = Artexhibition.OrderByDescending(x => x.ae_id).Skip((pageNo.Value - 1) * pageSize).Take(pageSize).ToList();

            model.page = new Pager(totalcategory, pageNo, pageSize);

            return PartialView(model);
        }
        #endregion

        #region Create Art Exhibition
        public ActionResult CreateArtExhibition()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }

            var exhibitions = db.Exhibitions.Where(x => x.e_status == 1).ToList();
            ViewBag.exhibitions = new SelectList(exhibitions, "e_id", "e_name");
            var arts = db.Arts.Where(x => x.art_status == 1).ToList();
            ViewBag.arts = new SelectList(arts, "art_id", "art_name");
            return PartialView();
        }

        [HttpPost]
        public ActionResult CreateArtExhibition(Art_exhibition art_exhibition)
        {
            var exhibitions = db.Exhibitions.Where(x => x.e_status == 1).ToList();
            ViewBag.exhibitions = new SelectList(exhibitions, "e_id", "e_name");
            var arts = db.Arts.Where(x => x.art_status == 1).ToList();
            ViewBag.arts = new SelectList(arts, "art_id", "art_name");

            var Artexhibition = db.Art_exhibition.Where(x => x.ae_fk_e == art_exhibition.ae_fk_e && x.ae_fk_art_id == art_exhibition.ae_fk_art_id && x.Exhibition.e_status == 1 && art_exhibition.ae_status == 1).FirstOrDefault();
            if (art_exhibition != null)
            {
                if (Artexhibition == null)
                {
                    Art_exhibition e = new Art_exhibition();
                    e.ae_fk_art_id = art_exhibition.ae_fk_art_id;
                    e.ae_fk_e = art_exhibition.ae_fk_e;
                    e.ae_status = 1;
                    db.Art_exhibition.Add(e);
                    db.SaveChanges();
                }
                else
                {
                    TempData["Duplicate_Art_Exhibition"] = "<script>alert('Already Exist In Art Exhibition Please Select Another')</script>";
                }
            }

            return RedirectToAction("ArtExhibitionTable");
        }
        #endregion

        #region Edit Art Exhibition
        public ActionResult EditArtExhibition(int Id)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            if (Id > 0)
            {
                var exhibitions = db.Exhibitions.ToList();
                ViewBag.exhibitions = new SelectList(exhibitions, "e_id", "e_name");
                var arts = db.Arts.Where(x => x.art_status == 1).ToList();
                ViewBag.arts = new SelectList(arts, "art_id", "art_name");
                var data = db.Art_exhibition.Where(x => x.ae_id == Id).FirstOrDefault();
                return PartialView(data);
            }
            return RedirectToAction("ArtExhibitionTable");
        }

        [HttpPost]
        public ActionResult EditArtExhibition(Art_exhibition ae)
        {
            var data = db.Art_exhibition.Where(x => x.ae_id == ae.ae_id).FirstOrDefault();
            var exhibitions = db.Exhibitions.Where(x => x.e_status == 1).ToList();
            ViewBag.exhibitions = new SelectList(exhibitions, "e_id", "e_name");
            var arts = db.Arts.Where(x => x.art_status == 1).ToList();
            ViewBag.arts = new SelectList(arts, "art_id", "art_name");
            if (data != null)
            {
                data.ae_fk_art_id = ae.ae_fk_art_id;
                data.ae_fk_e = ae.ae_fk_e;
                data.ae_status = ae.ae_status;

                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("ArtExhibitionTable");
        }
        #endregion

        #region Delete Art Exhibition
        [HttpPost]
        public ActionResult DeleteArtExhibition(int Id)
        {
            if (Id > 0)
            {
                var data = db.Art_exhibition.Where(x => x.ae_id == Id).FirstOrDefault();
                if (data != null)
                {
                    data.ae_status = 0;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ArtExhibitionTable");
        }
        #endregion

        #region Details Art Exhibition
        public ActionResult DetailsArtExhibition(int Id)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            var data = db.Art_exhibition.Find(Id);
            return PartialView(data);
        }
        #endregion

        #region Contact
        public ActionResult Contact()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        
        public ActionResult ContactTable(string search, int? pageNo)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            int pageSize = 5;

            AdminContactViewModel model = new AdminContactViewModel();

            pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

            model.pageNo = pageNo.Value;

            var contact = db.Contacts.ToList();

            var totalcontact = db.Contacts.Count();

            if (!string.IsNullOrEmpty(search))
            {
                contact = contact.Where(x => x.cont_name.ToLower().Contains(search.Trim().ToLower())).ToList();
                model.SearchTerm = search;
                totalcontact = contact.Count();
            }

            model.Contact = contact.OrderByDescending(x => x.cont_id).Skip((pageNo.Value - 1) * pageSize).Take(pageSize).ToList();

            model.page = new Pager(totalcontact, pageNo, pageSize);

            return PartialView(model);
        }
        #endregion

        #region Details Contact
        public ActionResult DetailsContact(int Id)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            var data = db.Contacts.Find(Id);
            return PartialView(data);
        }
        #endregion
    }
}