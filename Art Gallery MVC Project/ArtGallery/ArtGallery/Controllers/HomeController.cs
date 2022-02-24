using ArtGallery.Models;
using ArtGallery.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace ArtGallery.Controllers
{
    public class HomeController : Controller
    {
        db_ArtGalleryEntities db = new db_ArtGalleryEntities();

        #region Home Page
        public ActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();
            model.Categories = db.Categories.Take(4).ToList();
            model.Arts = db.Arts.Where(x => x.art_status == 1).OrderByDescending(x => x.art_id).ToList();
            var artIds = db.Arts.Where(x => x.art_status == 1).Select(x => x.art_id).ToList();
            model.AuctionArt = db.Auctions.Where(x => artIds.Contains(x.auc_fk_art.Value)).ToList();
            return View(model);
        }
        #endregion

        #region About

        public ActionResult About()
        {
            return View();
        }

        #endregion

        #region Contact

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(Contact c)
        {
            JsonResult result = new JsonResult();
            if (c != null)
            {

                db.Contacts.Add(c);
                int a = db.SaveChanges();
                if (a > 0)
                {
                    result.Data = new { Success = true };
                }
                else
                {
                    result.Data = new { Success = false };
                }
            }
            else
            {
                result.Data = new { Success = false };
            }
            return result;
        }
        #endregion

        #region Shop Page 

        //I had previously made <a href""></a> throw-in shop page  so page reload 
        public ActionResult Shop(string search, int? sortBy, int? pageNo, int? categoryID, int? maximumPrice, int? minimumPrice)
        {
            ShopViewModel model = new ShopViewModel();

            model.Categories = db.Categories.Where(x => x.Arts.Count > 0).ToList();

            var ArtWork = db.Arts.Where(x => x.art_status == 1).ToList();

            if (ArtWork != null && ArtWork.Count > 0)
            {

                model.searchTerm = search;

                model.MaximumPrice = db.Arts.Where(x => x.art_status == 1).Max(x => x.art_price);

                model.pageSize = 10;

                pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

                model.sortBy = sortBy;

                model.categoryID = categoryID;

                model.Arts = ShopMethod(sortBy, categoryID, pageNo, maximumPrice, minimumPrice, search);

                var totalArts = ShopMethodCount(sortBy, categoryID, maximumPrice, minimumPrice, search);

                model.page = new Pager(totalArts, pageNo, model.pageSize);

                var auction_Arts = db.Auctions.ToList();

            }
            return View(model);
        }

        //and then after i made the shop page in ajax throw so both are code here
        public ActionResult FilterArts(string search, int? sortBy, int? pageNo, int? categoryID, int? maximumPrice, int? minimumPrice)
        {
            FilterArtsViewModel model = new FilterArtsViewModel();

            model.pageSize = 10;

            pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

            model.sortBy = sortBy;

            model.searchTerm = search;

            model.categoryID = categoryID;

            model.maximumPrice = maximumPrice;

            model.minimumPrice = maximumPrice;

            model.Arts = ShopMethod(sortBy, categoryID, pageNo, maximumPrice, minimumPrice, search);

            var totalArts = ShopMethodCount(sortBy, categoryID, maximumPrice, minimumPrice, search);

            model.page = new Pager(totalArts, pageNo, model.pageSize);

            return PartialView(model);
        }

        public List<Art> ShopMethod(int? sortBy, int? categoryID, int? pageNo, int? maximumPrice, int? minimumPrice, string search)
        {
            int pageSize = 10;

            pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

            var Arts = db.Arts.OrderByDescending(x => x.art_id).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                Arts = Arts.Where(x => x.art_name.ToLower().Contains(search.Trim().ToLower())).OrderByDescending(x => x.art_id).ToList();
            }

            if (maximumPrice.HasValue)
            {
                Arts = Arts.Where(x => x.art_price < maximumPrice).ToList();
            }

            if (minimumPrice.HasValue)
            {
                Arts = Arts.Where(x => x.art_price > minimumPrice).ToList();
            }

            if (categoryID.HasValue)
            {
                Arts = Arts.Where(x => x.art_fk_cat == categoryID).ToList();
            }

            if (sortBy.HasValue)
            {
                var sort = sortBy.Value;
                switch (sortBy)
                {
                    case 2:
                        Arts = Arts.OrderByDescending(x => x.art_id).ToList();
                        break;
                    case 3:
                        Arts = Arts.OrderBy(x => x.art_price).ToList();
                        break;
                    case 4:
                        Arts = Arts.OrderByDescending(x => x.art_price).ToList();
                        break;
                    default:
                        Arts = Arts.OrderByDescending(x => x.art_id).ToList();
                        break;
                }
            }

            return Arts.Where(x => x.art_status == 1).Skip((pageNo.Value - 1) * pageSize).Take(pageSize).ToList();
        }

        public int ShopMethodCount(int? sortBy, int? categoryID, int? maximumPrice, int? minimumPrice, string search)
        {
            var Arts = db.Arts.ToList();

            if (!string.IsNullOrEmpty(search))
            {
                Arts = Arts.Where(x => x.art_name.ToLower().Contains(search.ToLower())).ToList();
            }

            if (maximumPrice.HasValue)
            {
                Arts = Arts.Where(x => x.art_price < maximumPrice).ToList();
            }

            if (minimumPrice.HasValue)
            {
                Arts = Arts.Where(x => x.art_price > minimumPrice).ToList();
            }

            if (categoryID.HasValue)
            {
                Arts = Arts.Where(x => x.art_fk_cat == categoryID).ToList();
            }

            if (sortBy.HasValue)
            {
                var sort = sortBy.Value;
                switch (sortBy)
                {
                    case 2:
                        Arts = Arts.OrderByDescending(x => x.art_id).ToList();
                        break;
                    case 3:
                        Arts = Arts.OrderBy(x => x.art_price).ToList();
                        break;
                    case 4:
                        Arts = Arts.OrderByDescending(x => x.art_price).ToList();
                        break;
                    default:
                        Arts = Arts.OrderByDescending(x => x.art_price).ToList();
                        break;
                }
            }

            return Arts.Where(x => x.art_status == 1).Count();
        }

        #endregion

        #region Single Art Page
        public ActionResult SingleArt(int? id)
        {
            SingleArtViewModel model = new SingleArtViewModel();
            if (id > 0)
            {
                model.Art = db.Arts.Where(x => x.art_id == id && x.art_status == 1).FirstOrDefault();
                if (model.Art == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                model.Category = db.Categories.Where(x => x.cat_id == model.Art.art_fk_cat).FirstOrDefault();
                model.RelatedArts = db.Arts.Where(x => x.art_fk_cat == model.Category.cat_id && x.art_id != id && x.art_status == 1).OrderByDescending(x => x.art_id).Take(4).ToList();
                model.Artist = db.Artists.Where(x => x.ar_id == model.Art.art_fk_ar).FirstOrDefault();
                model.comments = db.Art_Comment.Where(x => x.ac_fk_art_id == model.Art.art_id).OrderByDescending(x => x.ac_id).ToList();
                var comments_cus_id = db.Art_Comment.Where(x => x.ac_fk_art_id == model.Art.art_id).OrderByDescending(x => x.ac_id).Select(x => x.ac_fk_cus_id).ToList();
                model.customer = db.Customers.ToList();

                var Art_Views_Data = db.Art_Views.Where(x => x.av_fk_art_id == model.Art.art_id).FirstOrDefault();
                if (Art_Views_Data == null)
                {
                    Art_Views views = new Art_Views();
                    views.av_fk_art_id = model.Art.art_id;
                    views.av_veiws_count = 1;
                    db.Art_Views.Add(views);
                    db.SaveChanges();
                }
                else
                {
                    var Update_Art_Views_Data = db.Art_Views.Where(x => x.av_fk_art_id == model.Art.art_id).FirstOrDefault();
                    Update_Art_Views_Data.av_fk_art_id = model.Art.art_id;
                    Update_Art_Views_Data.av_veiws_count = Update_Art_Views_Data.av_veiws_count + 1;
                    db.Entry(Update_Art_Views_Data).State = EntityState.Modified;
                    db.SaveChanges();
                }
                model.artViews = db.Art_Views.Where(x => x.av_fk_art_id == model.Art.art_id).FirstOrDefault();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public bool SingleArt(int id)
        {
            if (id > 0)
            {
                var customer_id = int.Parse(Session["customer_id"].ToString());
                var data = db.Carts.Where(x => x.art_id == id && x.cus_id == customer_id).Select(x => x.art_id).FirstOrDefault();
                Cart c = new Cart();
                c.art_id = id;
                c.cus_id = int.Parse(Session["customer_id"].ToString());
                if (data != id)
                {
                    db.Carts.Add(c);
                    db.SaveChanges();


                    var ArtIds = db.Arts.Select(x => x.art_id).ToList();
                    var cartItems = db.Carts.Where(x => ArtIds.Contains(x.art_id) && x.cus_id == customer_id).Select(x => x.art_id).ToList();
                    Session["cartItemsCount"] = cartItems.Count();

                    return true;
                }
                else
                {

                    var ArtIds = db.Arts.Select(x => x.art_id).ToList();
                    var cartItems = db.Carts.Where(x => ArtIds.Contains(x.art_id) && x.cus_id == customer_id).Select(x => x.art_id).ToList();
                    Session["cartItemsCount"] = cartItems.Count();

                    return false;
                }
            }


            return false;
        }

        #endregion

        #region Category Arts
        public ActionResult CategoryArts(int id)
        {
            if (id > 0)
            {
                TempData["categoryName"] = db.Categories.Where(x => x.cat_id == id).Select(x => x.cat_name).FirstOrDefault();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public ActionResult CategoryArtsTable(int id, string search)
        {
            CategoryArtsViewModel model = new CategoryArtsViewModel();

            model.Arts = db.Arts.Where(x => x.art_status == 1).OrderByDescending(x => x.art_id).ToList();

            model.numberOfArts = 8;

            model.Arts = db.Arts.Where(x => x.art_fk_cat == id && x.art_status == 1).Take(model.numberOfArts).OrderByDescending(x => x.art_id).ToList();

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult CategoryArtsTable(int id, string search, int? numberOfArts)
        {
            CategoryArtsViewModel model = new CategoryArtsViewModel();

            model.Arts = db.Arts.Where(x => x.art_status == 1).ToList();

            model.numberOfArts = numberOfArts.HasValue ? numberOfArts.Value : 8;

            if (!string.IsNullOrEmpty(search))
            {
                model.Arts = db.Arts.OrderByDescending(x => x.art_id).Where(x => x.art_name.ToLower().Contains(search.Trim().ToLower())).ToList();
            }
            else
            {
                model.Arts = db.Arts.OrderByDescending(x => x.art_id).Where(x => x.art_fk_cat == id).Take(model.numberOfArts).ToList();
            }

            return PartialView(model);
        }
        #endregion

        #region Cart
        [Authorize]
        public ActionResult Cart()
        {
            return View();
        }

        public ActionResult CartTable()
        {
            var ArtIds = db.Arts.Select(x => x.art_id).ToList();
            var customer_id = int.Parse(Session["customer_id"].ToString());
            var cartItems = db.Carts.Where(x => ArtIds.Contains(x.art_id) && x.cus_id == customer_id).Select(x => x.art_id).ToList();
            var arts = db.Arts.Where(x => cartItems.Contains(x.art_id)).ToList();
            Session["CartTable"] = 1;
            return PartialView(arts);
        }

        public ActionResult RemoveCartItem(int id)
        {
            if (id > 0)
            {
                var customer_id = int.Parse(Session["customer_id"].ToString());
                var data = db.Carts.Where(x => x.art_id == id && x.cus_id == customer_id).FirstOrDefault();
                db.Entry(data).State = EntityState.Deleted;
                db.SaveChanges();

                var ArtIds = db.Arts.Select(x => x.art_id).ToList();
                var cartItems = db.Carts.Where(x => ArtIds.Contains(x.art_id) && x.cus_id == customer_id).Select(x => x.art_id).ToList();
                Session["cartItemsCount"] = cartItems.Count();
            }
            return RedirectToAction("CartTable");
        }
        #endregion

        #region SignUp
        public ActionResult CustomerSignUp()
        {
            var countryList = db.Countries.ToList();
            ViewBag.Country = new SelectList(countryList, "con_id", "con_name");


            return View();
        }

        [HttpPost]
        public ActionResult CustomerSignUp(Customer cus, HttpPostedFileBase c_image)
        {
            var countryList = db.Countries.ToList();
            ViewBag.Country = new SelectList(countryList, "con_id", "con_name");

            var category = db.Categories.ToList();
            ViewBag.category = new SelectList(category, "cat_id", "cat_name");


            if (c_image != null)
            {
                string filename = Path.GetFileNameWithoutExtension(c_image.FileName);
                string extension = Path.GetExtension(c_image.FileName);
                HttpPostedFileBase PostedFile = c_image;
                int length = PostedFile.ContentLength;
                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".png" || extension.ToLower() == ".jpeg")
                {
                    if (length <= 1000000)
                    {
                        filename = filename + extension;
                        cus.c_image = "/Content/images/" + filename;
                        c_image.SaveAs(Path.Combine(Server.MapPath("~/Content/images"), filename));
                    }
                    else
                    {
                        ViewBag.sizeMsg = "<script>alert('Image Size Must be in 1 MB')</script>";
                        return View();
                    }
                }
                else
                {
                    ViewBag.extensionMsg = "<script>alert('Image Not Supported')</script>";
                    return View();
                }
            }
            else
            {
                cus.c_image = "/Content/images/ByDefaultArtistImg.jpg";
            }
            db.Customers.Add(cus);
            db.SaveChanges();

            return RedirectToAction("CustomerLogin", "Home");
        }
        #endregion

        #region Login
        public ActionResult CustomerLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CustomerLogin(Customer cus, string ReturnUrl)
        {
            if (IsValid(cus) == true)
            {
                FormsAuthentication.SetAuthCookie("username", false);
                Session["customer_name"] = cus.c_userName.ToString();
                if (ReturnUrl != null)
                {

                    var ArtIds = db.Arts.Select(x => x.art_id).ToList();
                    var customer_id = int.Parse(Session["customer_id"].ToString());
                    var cartItems = db.Carts.Where(x => ArtIds.Contains(x.art_id) && x.cus_id == customer_id).Select(x => x.art_id).ToList();
                    Session["cartItemsCount"] = cartItems.Count();
                    return Redirect(ReturnUrl);
                }
                else
                {

                    var ArtIds = db.Arts.Select(x => x.art_id).ToList();
                    var customer_id = int.Parse(Session["customer_id"].ToString());
                    var cartItems = db.Carts.Where(x => ArtIds.Contains(x.art_id) && x.cus_id == customer_id).Select(x => x.art_id).ToList();
                    Session["cartItemsCount"] = cartItems.Count();
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.Clear();
                ViewBag.error = "Invalid Username or Password";
                return View();
            }
        }

        public bool IsValid(Customer cus)
        {
            var credentials = db.Customers.Where(x => x.c_userName.ToLower() == cus.c_userName.ToLower() && x.c_password.ToLower() == cus.c_password.ToLower()).FirstOrDefault();
            if (credentials != null)
            {
                Session["customer_id"] = credentials.c_id.ToString();
                Session["customer_name"] = credentials.c_userName.ToString();
                Session["customer_image"] = credentials.c_image.ToString();
                Session["Customer_regsiterDate"] = credentials.register_date.ToString();
                return (cus.c_userName.ToLower() == credentials.c_userName.ToLower() && cus.c_password.ToLower() == credentials.c_password.ToLower());
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Logout
        public ActionResult CustomerLogout()
        {
            FormsAuthentication.SignOut();
            Session["customer_id"] = null;
            Session["cartItemsCount"] = null;
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Favourite Art
        public ActionResult Favourite_Art(int id)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (id > 0)
            {
                var customer_id = int.Parse(Session["customer_id"].ToString());
                var data = db.favourite_Arts.Where(x => x.fa_fk_art_id == id && x.fa_fk_cus == customer_id).Select(x => x.fa_fk_art_id).FirstOrDefault();
                if (data != id)
                {
                    favourite_Arts fa = new favourite_Arts();
                    fa.fa_fk_art_id = id;
                    fa.fa_fk_cus = customer_id;
                    db.favourite_Arts.Add(fa);
                    int a = db.SaveChanges();
                    if (a > 0)
                    {
                        result.Data = new { Success = true };
                    }
                }
                else
                {
                    result.Data = new { Success = false };
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return result;
        }

        public ActionResult Favourite_Art_delete(int id)
        {
            if (id > 0)
            {
                var customer_id = int.Parse(Session["customer_id"].ToString());
                var data = db.favourite_Arts.Where(x => x.fa_fk_art_id == id && x.fa_fk_cus == customer_id).FirstOrDefault();
                db.Entry(data).State = EntityState.Deleted;
                int a = db.SaveChanges();
                if (a > 0)
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ShowFavouriteitems()
        {
            return PartialView();
        }
        #endregion

        #region Gallery
        public ActionResult AddCustomerGallery(int id)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (id > 0)
            {
                var customer_id = int.Parse(Session["customer_id"].ToString());

                Customer_Gallery customer_Gallery = new Customer_Gallery();
                var customer = db.Customer_Gallery.Where(x => x.cg_fk_customer == customer_id).Select(x => x.cg_fk_customer).FirstOrDefault();
                if (customer != customer_id)
                {
                    customer_Gallery.cg_fk_customer = customer_id;
                    db.Customer_Gallery.Add(customer_Gallery);
                    db.SaveChanges();
                }


                var customer_gallery_id = db.Customer_Gallery.Where(x => x.cg_fk_customer == customer_id).Select(x => x.cg_id).FirstOrDefault();
                var data = db.Customer_Gallery_Items.Where(x => x.cgi_fk_art_id == id && x.cgi_fk_cg_id == customer_gallery_id).FirstOrDefault();

                if (data == null)
                {
                    Customer_Gallery_Items customer_Gallery_Items = new Customer_Gallery_Items();
                    customer_Gallery_Items.cgi_fk_art_id = id;
                    customer_Gallery_Items.cgi_fk_cg_id = customer_gallery_id;
                    db.Customer_Gallery_Items.Add(customer_Gallery_Items);
                    int a = db.SaveChanges();
                    if (a > 0)
                    {
                        result.Data = new { Success = true };
                    }
                }
                else
                {
                    result.Data = new { Success = false };
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return result;

        }

        [Authorize]
        public ActionResult Gallery()
        {
            var customer_id = int.Parse(Session["customer_id"].ToString());
            var customer_Gallery = db.Customer_Gallery.Where(x => x.cg_fk_customer == customer_id).FirstOrDefault();

            if (customer_Gallery != null)
            {
                var customer_gallery_item = db.Customer_Gallery_Items.Where(x => x.cgi_fk_cg_id == customer_Gallery.cg_id).ToList();
                return View(customer_gallery_item);
            }
            else
            {
                return View();
            }
        }

        public ActionResult RemoveGalleryItem(int id)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            if (id > 0)
            {
                var customer_id = int.Parse(Session["customer_id"].ToString());
                var customer_gallery = db.Customer_Gallery.Where(x => x.cg_fk_customer == customer_id).FirstOrDefault();
                var data = db.Customer_Gallery_Items.Where(x => x.cgi_fk_art_id == id && x.cgi_fk_cg_id == customer_gallery.cg_id).FirstOrDefault();
                db.Entry(data).State = EntityState.Deleted;
                int a = db.SaveChanges();
                if (a > 0)
                {
                    result.Data = new { Success = true };
                }
                else
                {
                    result.Data = new { Success = false };
                }

            }
            else
            {
                result.Data = new { Success = false };
            }
            return result;
        }
        #endregion

        #region Artist

        public ActionResult ShowArtist()
        {
            return View();
        }

        public ActionResult ShowArtistTable(string search)
        {
            var artist = db.Artists.ToList();

            if (!string.IsNullOrEmpty(search))
            {
                artist = db.Artists.OrderByDescending(x => x.ar_id).Where(x => x.ar_userName.ToLower().Contains(search.Trim().ToLower())).ToList();
            }

            ViewBag.country = db.Countries.ToList();
            return PartialView(artist);
        }

        public ActionResult ArtistArts(int id)
        {
            if (id > 0)
            {
                ViewBag.ArtistData = db.Artists.Find(id);
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ArtistArtsTable(int id, string search)
        {
            CategoryArtsViewModel model = new CategoryArtsViewModel();

            model.Arts = db.Arts.Where(x => x.art_status == 1).OrderByDescending(x => x.art_id).ToList();

            model.numberOfArts = 8;

            model.Arts = db.Arts.Where(x => x.art_fk_ar == id && x.art_status == 1).OrderByDescending(x => x.art_id).Take(model.numberOfArts).ToList();

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ArtistArtsTable(int id, string search, int? numberOfArts)
        {
            CategoryArtsViewModel model = new CategoryArtsViewModel();

            model.Arts = db.Arts.Where(x => x.art_status == 1).OrderByDescending(x => x.art_id).ToList();

            model.numberOfArts = numberOfArts.HasValue ? numberOfArts.Value : 8;

            if (!string.IsNullOrEmpty(search))
            {
                model.Arts = db.Arts.OrderByDescending(x => x.art_id).Where(x => x.art_name.ToLower().Contains(search.Trim().ToLower())).ToList();
            }
            else
            {
                model.Arts = db.Arts.OrderByDescending(x => x.art_id).Where(x => x.art_fk_ar == id).Take(model.numberOfArts).ToList();
            }

            return PartialView(model);
        }
        #endregion

        #region Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            CheckoutViewModel model = new CheckoutViewModel();
            int customer_id = int.Parse(Session["customer_id"].ToString());
            var cart = db.Carts.Where(x => x.cus_id == customer_id).Select(x => x.art_id).ToList();
            if (cart.Count > 0)
            {
                model.Art = db.Arts.Where(x => cart.Contains(x.art_id)).ToList();
                model.customer = db.Customers.Where(x => x.c_id == customer_id).FirstOrDefault();
                model.totalAmount = db.Arts.Where(x => cart.Contains(x.art_id)).Sum(x => x.art_price);
                var countryId = int.Parse(model.customer.c_country);
                var country = db.Countries.Where(x => x.con_id == countryId).Select(x => x.con_name).FirstOrDefault();
                var cityId = int.Parse(model.customer.c_city);
                var city = db.Cities.Where(x => x.ci_id == cityId).Select(x => x.ci_name).FirstOrDefault();
                ViewBag.country = country;
                ViewBag.city = city;
            }
            else
            {
                return RedirectToAction("Shop", "Home");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Checkout(order order)
        {
            JsonResult result = new JsonResult();
            if (order != null)
            {
                order o = new order();
                o.o_fk_cus_id = order.o_fk_cus_id;
                o.o_address = order.o_address;
                o.o_phone = order.o_phone;
                o.o_email = order.o_email;
                o.o_zip = order.o_zip;
                o.o_total_amout = order.o_total_amout - (order.o_total_amout * 10 / 100);
                o.o_status = "Pending";
                if (!string.IsNullOrEmpty(order.o_easyPaisa))
                {
                    o.o_easyPaisa = order.o_easyPaisa;
                }
                if (!string.IsNullOrEmpty(order.o_jazzCash))
                {
                    o.o_jazzCash = order.o_jazzCash;
                }
                if (!string.IsNullOrEmpty(order.o_credit_number) && !string.IsNullOrEmpty(order.o_pin))
                {
                    o.o_credit_number = order.o_credit_number;
                    o.o_pin = order.o_pin;
                }
                if (!string.IsNullOrEmpty(order.o_hbl_bank_account_number) && !string.IsNullOrEmpty(order.o_cnic_number))
                {
                    o.o_hbl_bank_account_number = order.o_hbl_bank_account_number;
                    o.o_cnic_number = order.o_cnic_number;
                }
                if (!string.IsNullOrEmpty(order.o_cashOnDelivery))
                {
                    o.o_cashOnDelivery = order.o_cashOnDelivery;
                }
                db.orders.Add(o);
                db.SaveChanges();
                //website fund add amount 
                //start
                Website_fund website_Fund = new Website_fund();
                website_Fund.f_date = DateTime.Now.ToString();
                website_Fund.f_amount = order.o_total_amout * 10 / 100;
                website_Fund.f_fk_o_id = o.o_id;
                website_Fund.f_fk_c_id = o.o_fk_cus_id;
                db.Website_fund.Add(website_Fund);
                db.SaveChanges();
                //emd

                var cartIds = db.Carts.Where(x => x.cus_id == order.o_fk_cus_id).Select(x => x.art_id).ToList();
                var art = db.Arts.Where(x => cartIds.Contains(x.art_id)).ToList();
                var arts = db.Arts.Select(x => x.art_id).ToList();
                var cart = db.Carts.Where(x => x.cus_id == order.o_fk_cus_id && arts.Contains(x.art_id)).ToList();
                var order_id = db.orders.Where(x => x.o_id == o.o_id).Select(x => x.o_id).FirstOrDefault();

                foreach (var cartItems in cart)
                {
                    order_details order_Details = new order_details();
                    order_Details.od_fk_art_id = cartItems.art_id;
                    order_Details.od_fk_o = order_id;
                    db.order_details.Add(order_Details);
                    db.SaveChanges();
                }

                var cartids = db.Carts.Where(x => x.cus_id == order.o_fk_cus_id && arts.Contains(x.art_id)).Select(x => x.art_id).ToList();
                var existingCartItems = db.Carts.Where(x => cartids.Contains(x.art_id)).ToList();
                foreach (var cartItems in existingCartItems)
                {
                    db.Entry(cartItems).State = EntityState.Deleted;
                    db.SaveChanges();
                }

                foreach (var artsData in art)
                {
                    artsData.art_status = 0;
                    artsData.art_buy_status = 0;
                    db.Entry(artsData).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var art_id = db.Arts.Where(x => cartIds.Contains(x.art_id)).Select(x => x.art_id).ToList();
                var exhibition = db.Art_exhibition.Where(x => art_id.Contains(x.ae_fk_art_id.Value) && x.ae_status == 1).ToList();
                foreach (var e in exhibition)
                {
                    e.ae_status = 0;
                    db.Entry(e).State = EntityState.Modified;
                    db.SaveChanges();
                }

                notification pNotification = new notification();
                pNotification.n_text = "Pending";
                pNotification.n_date = DateTime.Now.ToString();
                pNotification.n_fk_c_id = o.o_fk_cus_id;
                pNotification.n_fk_o_id = o.o_id;
                db.notifications.Add(pNotification);
                db.SaveChanges();
                result.Data = new { Success = true };
            }
            else
            {
                result.Data = new { Success = false };
            }

            return result;
        }
        #endregion

        #region Methods
        public int UserNameCheck(string username)
        {
            var data = db.Customers.Where(x => x.c_userName.ToLower() == username.ToLower()).SingleOrDefault();
            if (data != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public ActionResult CartItemsCount()
        {
            return PartialView();
        }

        public ActionResult RemoveNotifications(int id)
        {
            if (id > 0)
            {
                var data = db.notifications.Where(x => x.n_fk_c_id == id).ToList();
                foreach (var item in data)
                {
                    db.Entry(item).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Auction in Single Art Page

        public ActionResult SingleArtAuction(int? id)
        {

            SingleArtViewModel model = new SingleArtViewModel();
            if (id > 0)
            {
                model.Art = db.Arts.Where(x => x.art_id == id && x.art_status == 1).FirstOrDefault();
                if (model.Art != null)
                {
                    model.Category = db.Categories.Where(x => x.cat_id == model.Art.art_fk_cat).FirstOrDefault();
                    model.RelatedArts = db.Arts.Where(x => x.art_fk_cat == model.Category.cat_id && x.art_id != id && x.art_status == 1).OrderByDescending(x => x.art_id).Take(4).ToList();
                    model.auctionId = model.Art.art_id;
                    model.comments = db.Art_Comment.Where(x => x.ac_fk_art_id == model.Art.art_id).OrderByDescending(x => x.ac_id).ToList();
                    var comments_cus_id = db.Art_Comment.Where(x => x.ac_fk_art_id == model.Art.art_id).OrderByDescending(x => x.ac_id).Select(x => x.ac_fk_cus_id).ToList();
                    model.customer = db.Customers.ToList();

                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                var auction = db.Auctions.Where(x => x.Art.art_id == id && x.auc_status == 1).FirstOrDefault();
                if (auction != null)
                {
                    model.endDate = db.Auctions.Where(x => x.auc_fk_art == id).FirstOrDefault();
                    var end_date = Convert.ToDateTime(model.endDate.auc_end_date);
                    if (end_date <= DateTime.Now)
                    {
                        var auction_id = db.Auctions.Where(x => x.auc_fk_art == id).FirstOrDefault();
                        auction_id.auc_status = 0;
                        var art = db.Arts.Where(x => x.art_id == id).FirstOrDefault();
                        art.art_status = 0;
                        db.SaveChanges();

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                var bids = db.Bids.Where(x => x.bid_fk_auc == auction.auc_id).ToList();
                if (bids != null)
                {
                    model.bids = db.Bids.Where(x => x.bid_fk_auc == auction.auc_id).ToList();
                    model.Country = db.Countries.ToList();
                    model.highestBid = db.Bids.Where(x => x.bid_fk_auc == auction.auc_id).OrderByDescending(x => x.bid_amount).FirstOrDefault();
                }
                var Art_Views_Data = db.Art_Views.Where(x => x.av_fk_art_id == model.Art.art_id).FirstOrDefault();
                if (Art_Views_Data == null)
                {
                    Art_Views views = new Art_Views();
                    views.av_fk_art_id = model.Art.art_id;
                    views.av_veiws_count = 1;
                    db.Art_Views.Add(views);
                    db.SaveChanges();
                }
                else
                {
                    var Update_Art_Views_Data = db.Art_Views.Where(x => x.av_fk_art_id == model.Art.art_id).FirstOrDefault();
                    Update_Art_Views_Data.av_fk_art_id = model.Art.art_id;
                    Update_Art_Views_Data.av_veiws_count = Update_Art_Views_Data.av_veiws_count + 1;
                    db.Entry(Update_Art_Views_Data).State = EntityState.Modified;
                    db.SaveChanges();
                }
                model.artViews = db.Art_Views.Where(x => x.av_fk_art_id == model.Art.art_id).FirstOrDefault();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public bool SingleArtAuction(int id)
        {
            var ArtIds = db.Arts.Select(x => x.art_id).ToList();
            var customer_id = int.Parse(Session["customer_id"].ToString());
            var cartItems = db.Carts.Where(x => ArtIds.Contains(x.art_id) && x.cus_id == customer_id).Select(x => x.art_id).ToList();
            Session["cartItemsCount"] = cartItems.Count();

            return false;
        }

        public ActionResult SingleArtAuctionTable()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult SingleArtAuctionTable(int id, decimal amount)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var auction = db.Auctions.Where(x => x.Art.art_id == id).FirstOrDefault();

            if (auction != null)
            {
                if (amount > 0)
                {
                    Bid bid = new Bid();
                    bid.bid_fk_cus = int.Parse(Session["customer_id"].ToString());
                    bid.bid_amount = Convert.ToInt64(amount);
                    bid.bid_timeStamp = DateTime.Now.ToString();
                    bid.bid_fk_auc = auction.auc_id;
                    db.Bids.Add(bid);
                    db.SaveChanges();

                    var auction_amount = db.Bids.Where(x => x.bid_fk_auc == auction.auc_id).OrderByDescending(x => x.bid_amount).Select(x => x.bid_amount).FirstOrDefault();
                    auction.auc_increment = auction_amount;
                    db.Entry(auction).State = EntityState.Modified;
                    db.SaveChanges();
                    result.Data = new { Success = true };
                }
                else
                {
                    result.Data = new { Success = false };
                }
            }

            else
            {
                result.Data = new { Success = false };
            }
            return result;
        }

        public ActionResult ShowAuctionTableResult(int id)
        {
            SingleArtViewModel model = new SingleArtViewModel();
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (id > 0)
            {
                model.Art = db.Arts.Where(x => x.art_id == id).FirstOrDefault();
                model.Category = db.Categories.Where(x => x.cat_id == model.Art.art_fk_cat).FirstOrDefault();
                model.RelatedArts = db.Arts.Where(x => x.art_fk_cat == model.Category.cat_id && x.art_id != id).OrderByDescending(x => x.art_id).Take(4).ToList();
                model.auctionId = model.Art.art_id;
                var auction = db.Auctions.Where(x => x.Art.art_id == id).FirstOrDefault();
                model.bids = db.Bids.Where(x => x.bid_fk_auc == auction.auc_id).ToList();
                model.Country = db.Countries.ToList();
                model.highestBid = db.Bids.Where(x => x.bid_fk_auc == auction.auc_id).OrderByDescending(x => x.bid_amount).FirstOrDefault();
            }
            return PartialView(model);
        }

        public ActionResult WinAuction(int art_id, int cus_id)
        {
            Session["ExamEndDate"] = DateTime.Now.AddDays(2);
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (art_id > 0 && cus_id > 0)
            {
                Cart c = new Cart();
                c.art_id = art_id;
                c.cus_id = cus_id;
                db.Carts.Add(c);
                db.SaveChanges();
                var winAuction = db.Bids.Where(x => x.Customer.c_id == cus_id).OrderByDescending(x => x.bid_amount).Select(x => x.Customer.c_userName).FirstOrDefault();
                var auction = db.Auctions.Where(x => x.auc_fk_art == art_id).FirstOrDefault();
                auction.auc_status = 0;
                var art = db.Arts.Where(x => x.art_id == art_id).FirstOrDefault();
                art.art_status = 0;
                db.SaveChanges();
                result.Data = winAuction;

                //Update the Cart Items Count
                var ArtIds = db.Arts.Select(x => x.art_id).ToList();
                var cartItems = db.Carts.Where(x => ArtIds.Contains(x.art_id) && x.cus_id == cus_id).Select(x => x.art_id).ToList();
                Session["cartItemsCount"] = cartItems.Count();
            }
            else
            {
                var auction = db.Auctions.Where(x => x.auc_fk_art == art_id).FirstOrDefault();
                auction.auc_status = 0;
                var art = db.Arts.Where(x => x.art_id == art_id).FirstOrDefault();
                art.art_status = 0;
                db.SaveChanges();
                result.Data = new { Success = false };
            }
            return result;
        }

        public ActionResult HighestBidderModelData(int id)
        {
            var auction = db.Auctions.Where(x => x.auc_fk_art == id).FirstOrDefault();
            var data = db.Bids.Where(x => x.bid_fk_auc == auction.auc_id).OrderByDescending(x => x.bid_amount).Select(x => x.Customer.c_id).FirstOrDefault();
            if (data > 0)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult HighestBidderAmount(int id)
        {
            var auction = db.Auctions.Where(x => x.auc_fk_art == id).FirstOrDefault();
            var data = db.Bids.Where(x => x.bid_fk_auc == auction.auc_id).OrderByDescending(x => x.bid_amount).Select(x => x.bid_amount).FirstOrDefault();
            if (data > 0)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Customer Profile
        public ActionResult CustomerProfile(int id)
        {
            var customer = db.Customers.Where(x => x.c_id == id).FirstOrDefault();
            int countryId = int.Parse(customer.c_country);
            var country = db.Countries.Where(x => x.con_id == countryId).Select(x => x.con_name).FirstOrDefault();
            int cityId = int.Parse(customer.c_country);
            var city = db.Cities.Where(x => x.ci_id == cityId).Select(x => x.ci_name).FirstOrDefault();
            ViewBag.country = country;
            ViewBag.city = city;
            return View(customer);
        }
        #endregion

        #region Exhibition

        public ActionResult Exhibition()
        {
            var exhibition_Data = db.Exhibitions.Where(x => x.e_status == 1).OrderByDescending(x => x.e_id).ToList();
            return View(exhibition_Data);
        }

        #endregion

        #region Exhibition Arts
        public ActionResult ExhibitionArts(int id)
        {


            TempData["ExhibitionName"] = db.Exhibitions.Where(x => x.e_id == id).Select(x => x.e_name).FirstOrDefault();

            return View();
        }

        public ActionResult ExhibitionArtsTable(int id, string search)
        {

            HomeExhibitionArtsViewModel model = new HomeExhibitionArtsViewModel();

            var art_exhibition = db.Art_exhibition.Where(x => x.ae_fk_e == id && x.ae_status == 1).Select(x => x.ae_fk_art_id).ToList();

            model.Exhibition = db.Exhibitions.Where(x => x.e_id == id).FirstOrDefault();

            model.numberOfArts = 8;

            model.Arts = db.Arts.Where(x => art_exhibition.Contains(x.art_id) && x.art_status == 1).OrderByDescending(x => x.art_id).Take(model.numberOfArts).ToList();

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ExhibitionArtsTable(int id, string search, int? numberOfArts)
        {
            HomeExhibitionArtsViewModel model = new HomeExhibitionArtsViewModel();

            var art_exhibition = db.Art_exhibition.Where(x => x.ae_fk_e == id && x.ae_status == 1).Select(x => x.ae_fk_art_id).ToList();

            model.Arts = db.Arts.Where(x => art_exhibition.Contains(x.art_id) && x.art_status == 1).ToList();

            model.Exhibition = db.Exhibitions.Where(x => x.e_id == id).FirstOrDefault();

            var art_ids = db.Arts.Where(x => art_exhibition.Contains(x.art_id)).Select(x => x.art_id).ToList();

            model.numberOfArts = numberOfArts.HasValue ? numberOfArts.Value : 8;

            if (!string.IsNullOrEmpty(search))
            {
                model.Arts = db.Arts.OrderByDescending(x => x.art_id).Where(x => x.art_name.ToLower().Contains(search.Trim().ToLower()) && art_ids.Contains(x.art_id)).Take(model.numberOfArts).ToList();
            }
            else
            {
                model.Arts = db.Arts.OrderByDescending(x => x.art_id).Where(x => art_ids.Contains(x.art_id)).Take(model.numberOfArts).ToList();
            }

            return PartialView(model);
        }
        #endregion

        #region View All Notification
        public ActionResult ViewAllNotification()
        {
            ViewNotificationViewModel model = new ViewNotificationViewModel();

            int customer_id = int.Parse(Session["customer_id"] != null ? Session["customer_id"].ToString() : "0");
            var customer = db.Customers.Where(x => x.c_id == customer_id).FirstOrDefault();
            if (customer != null)
            {
                var orderId = db.orders.Where(x => x.o_fk_cus_id == customer_id).Select(x => x.o_id).ToList();
                var a = new List<notification>();
                var notification = db.notifications.Where(x => x.n_fk_c_id == customer_id || x.n_fk_c_id == null).ToList();
                foreach (var item in notification)
                {
                    var register_uer_date = Convert.ToDateTime(Session["Customer_regsiterDate"].ToString());

                    if (register_uer_date <= Convert.ToDateTime(item.n_date.ToString()))
                    {
                        a.Add(item);
                    }
                }
                if (notification.Count <= 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    model.Notifications = a;
                    return View(model);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region Art Comments
        [Authorize]
        public ActionResult Arts_Comment(Art_Comment art_Comment)
        {
            if (art_Comment != null)
            {
                SingleArtViewModel model = new SingleArtViewModel();

                var comment = new Art_Comment();
                comment.ac_text = art_Comment.ac_text;
                comment.ac_time_stamp = DateTime.Now.ToString();
                comment.ac_fk_cus_id = int.Parse(Session["customer_id"].ToString());
                comment.ac_fk_art_id = art_Comment.ac_fk_art_id;
                db.Art_Comment.Add(comment);
                db.SaveChanges();

                model.Art = db.Arts.Where(x => x.art_id == comment.ac_fk_art_id).FirstOrDefault();
                model.comments = db.Art_Comment.Where(x => x.ac_fk_art_id == model.Art.art_id).OrderByDescending(x => x.ac_id).ToList();
                var comments_cus_id = db.Art_Comment.Where(x => x.ac_fk_art_id == model.Art.art_id).OrderByDescending(x => x.ac_id).Select(x => x.ac_fk_cus_id).ToList();
                model.customer = db.Customers.ToList();
                return PartialView(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region Show Order
        public ActionResult ShowOrder(int id)
        {
            ShowOrderViewModel model = new ShowOrderViewModel();
            if (id > 0)
            {
                model.Orders = db.orders.Where(x => x.o_fk_cus_id == id).OrderByDescending(x=>x.o_id).ToList();
                var orderIds = db.orders.Where(x => x.o_fk_cus_id == id).Select(x => x.o_id).ToList();
                model.OrdersDetails = db.order_details.Where(x => orderIds.Contains(x.od_fk_o.Value)).ToList();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
        #endregion
    }
}

