using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ArtGallery.Models;
using ArtGallery.ViewModels;

namespace ArtGallery.Controllers
{
    public class ArtistController : Controller
    {
        db_ArtGalleryEntities db = new db_ArtGalleryEntities();

        #region Dashboard
        public ActionResult Dashboard()
        {
            if (Session["ar_id"] == null)
            {
                return RedirectToAction("Login");
            }
            int ar_id = int.Parse(Session["ar_id"].ToString());
            var data = db.Arts.Where(x => x.art_fk_ar == ar_id).ToList();
            Session["Total_Arts"] = data.Count;
            var arts = db.Arts.Where(x => x.art_fk_ar == ar_id).ToList();
            int totalAmount = 0;
            foreach (var art in arts)
            {
                if (art.art_buy_status == 0)
                {
                    totalAmount += art.art_price - (art.art_price * 10 / 100);
                }
            }
            Session["totalAmount"] = totalAmount;
            var soldArts = db.Arts.Where(x => x.art_buy_status == 1 && x.art_fk_ar == ar_id).ToList();
            Session["soldArts"] = soldArts.Count;
            var isStockArts = db.Arts.Where(x => x.art_buy_status == 0 && x.art_fk_ar == ar_id).ToList();
            Session["inStockArts"] = isStockArts.Count;
            var FixedPrice_Arts = db.Arts.Where(x => x.art_isAuction == false && x.art_fk_ar == ar_id).ToList();
            Session["FixedPrice_Arts"] = FixedPrice_Arts.Count;
            var Auction_Arts = db.Arts.Where(x => x.art_isAuction == true && x.art_fk_ar == ar_id).ToList();
            Session["Auction_Arts"] = Auction_Arts.Count;
            return View();
        }
        #endregion

        #region Show Arts
        public ActionResult Arts()
        {
            if (Session["ar_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        //this is a partial view all art data show in (Arts View)
        public ActionResult ArtsTable(string search, int? pageNo)
        {
            if (Session["ar_id"] == null)
            {
                return RedirectToAction("Login");
            }
            int pageSize = 5;

            ArtistViewModel model = new ArtistViewModel();

            pageNo = pageNo.HasValue ? pageNo.Value > 0 ? pageNo.Value : 1 : 1;

            model.pageNo = pageNo.Value;

            model.Style_Of_Arts = db.Style_of_Art.ToList();

            int ar_id = int.Parse(Session["ar_id"].ToString());

            var arts = db.Arts.Where(x => x.art_fk_ar == ar_id).ToList();
                        
            var totalArts = db.Arts.Where(x => x.art_fk_ar == ar_id).Count();

            if (!string.IsNullOrEmpty(search))
            {
                arts = arts.Where(x => x.art_name.ToLower().Contains(search.Trim().ToLower())).ToList();
                model.SearchTerm = search;
                totalArts = arts.Count();
            }

            model.Arts = arts.OrderByDescending(x => x.art_id).Skip((pageNo.Value - 1) * pageSize).Take(pageSize).ToList();

            model.page = new Pager(totalArts, pageNo, pageSize);

            return PartialView(model);
        }
        #endregion

        #region Create Art
        public ActionResult CreateArt()
        {
            if (Session["ar_id"] == null)
            {
                return RedirectToAction("Login");
            }
            var styleOfArts = db.Style_of_Art.ToList();
            ViewBag.styleOfArt = new SelectList(styleOfArts, "soA_id", "soA_name");

            var category = db.Categories.ToList();
            ViewBag.category = new SelectList(category, "cat_id", "cat_name");

            return PartialView();
        }

        [HttpPost]
        public ActionResult CreateArt(Art art)
        {
            var styleOfArts = db.Style_of_Art.ToList();
            ViewBag.styleOfArt = new SelectList(styleOfArts, "soA_id", "soA_name");

            if (art != null)
            {
                Art a = new Art();
                a.art_name = art.art_name;
                a.art_price = art.art_price;
                a.art_description = art.art_description;
                a.art_dateOfCreation = DateTime.Now.ToString();
                a.art_description = art.art_description;
                a.Art_fk_soA_id = art.Art_fk_soA_id;
                a.art_fk_ar = int.Parse(Session["ar_id"].ToString());
                a.art_status = 1;
                a.art_buy_status = 1;
                a.art_isAuction = art.art_isAuction;


                a.art_fk_cat = int.Parse(Session["categoryID"].ToString());
                if (string.IsNullOrEmpty(art.art_image))
                {
                    a.art_image = "/content/images/no-image.png";
                }
                else
                {
                    a.art_image = art.art_image;
                }

                db.Arts.Add(a);
                db.SaveChanges();

                if (art.art_isAuction == true)
                {
                    Auction auction = new Auction();
                    auction.auc_item = a.art_name;
                    auction.auc_start_date = DateTime.Now.ToString();
                    auction.auc_end_date = DateTime.Now.AddDays(2).ToString();
                    auction.auc_amount = a.art_price;
                    auction.auc_increment = a.art_price;
                    auction.auc_status = 1;
                    auction.auc_fk_art = a.art_id;

                    db.Auctions.Add(auction);
                    db.SaveChanges();
                }

                notification pNotification = new notification();
                pNotification.n_text = a.art_name;
                pNotification.n_date = DateTime.Now.ToString();
                pNotification.n_fk_c_id = null;
                pNotification.n_fk_o_id = null;
                db.notifications.Add(pNotification);
                db.SaveChanges();
            }

            return RedirectToAction("ArtsTable");
        }
        #endregion

        #region Edit Art
        public ActionResult EditArt(int Id)
        {
            if (Session["ar_id"] == null)
            {
                return RedirectToAction("Login");
            }
            if (Id > 0)
            {
                var styleOfArts = db.Style_of_Art.ToList();
                ViewBag.styleOfArt = new SelectList(styleOfArts, "soA_id", "soA_name");

                var data = db.Arts.Where(x => x.art_id == Id).FirstOrDefault();
                return PartialView(data);
            }
            return RedirectToAction("ArtsTable");
        }

        [HttpPost]
        public ActionResult EditArt(Art art)
        {
            var data = db.Arts.Where(x => x.art_id == art.art_id).FirstOrDefault();

            if (data != null)
            {
                data.art_name = art.art_name;
                data.art_price = art.art_price;
                data.art_description = art.art_description;
                data.Art_fk_soA_id = art.Art_fk_soA_id;
                data.art_status = art.art_status;
                data.art_dateOfCreation = DateTime.Now.ToString();
                data.art_fk_cat = int.Parse(Session["categoryID"].ToString());
                data.art_fk_ar = int.Parse(Session["ar_id"].ToString());
                data.art_isAuction = art.art_isAuction;
                
                if (art.art_isAuction == true)
                {

                    var auction = db.Auctions.Where(x => x.auc_fk_art == data.art_id).FirstOrDefault();
                    auction.auc_item = data.art_name;
                    auction.auc_amount = data.art_price;
                    auction.auc_end_date = DateTime.Now.AddDays(1).ToString();
                    auction.auc_status = data.art_status.Value;

                    db.Entry(auction).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (!string.IsNullOrEmpty(art.art_image))
                {
                    data.art_image = art.art_image;
                }

                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("ArtsTable");
        }
        #endregion

        #region Delete Art
        [HttpPost]
        public ActionResult DeleteArt(int Id)
        {
            if (Id > 0)
            {
                var data = db.Arts.Where(x => x.art_id == Id).FirstOrDefault();
                if (data != null)
                {
                    data.art_status = 0;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ArtsTable");
        }
        #endregion

        #region Details Art
        public ActionResult detailsArt(int Id)
        {
            if (Session["ar_id"] == null)
            {
                return RedirectToAction("Login");
            }
            var data = db.Arts.Where(x=> x.art_id == Id).FirstOrDefault();
            var styleOfArt = db.Style_of_Art.Where(x => x.soA_id == data.Art_fk_soA_id).FirstOrDefault();
            ViewBag.styleOfArt = styleOfArt;
            return PartialView(data);
        }
        #endregion

        #region SignUp
        public ActionResult SignUp()
        {
            var countryList = db.Countries.ToList();
            ViewBag.Country = new SelectList(countryList, "con_id", "con_name");

            var category = db.Categories.ToList();
            ViewBag.category = new SelectList(category, "cat_id", "cat_name");

            return View();
        }

        [HttpPost]
        public ActionResult SignUp(Artist ar, HttpPostedFileBase ar_image)
        {
            var countryList = db.Countries.ToList();
            ViewBag.Country = new SelectList(countryList, "con_id", "con_name");

            var category = db.Categories.ToList();
            ViewBag.category = new SelectList(category, "cat_id", "cat_name");


            if (ar_image != null)
            {
                string filename = Path.GetFileNameWithoutExtension(ar_image.FileName);
                string extension = Path.GetExtension(ar_image.FileName);
                HttpPostedFileBase PostedFile = ar_image;
                int length = PostedFile.ContentLength;
                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".png" || extension.ToLower() == ".jpeg")
                {
                    if (length <= 1000000)
                    {
                        filename = filename + extension;
                        ar.ar_image = "/Content/images/" + filename;
                        ar_image.SaveAs(Path.Combine(Server.MapPath("~/Content/images"), filename));
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
                ar.ar_image = "/Content/images/ByDefaultArtistImg.jpg";
            }
            db.Artists.Add(ar);
            db.SaveChanges();
            return RedirectToAction("Login", "Artist");
        }
        #endregion

        #region Login
        public ActionResult Login()
        {
            if (Session["ar_id"] != null)
            {
                return RedirectToAction("Dashboard");
            }
            HttpCookie cookie = Request.Cookies["Artist_RememberMe"];
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
        public ActionResult Login(ArtistLoginViewModel ar)
        {
            ArtistLoginViewModel model = new ArtistLoginViewModel();
            if (ModelState.IsValid)
            {
                HttpCookie cookie = new HttpCookie("Artist_RememberMe");

                if (ar.RememberMe)
                {
                    cookie["username"] = ar.ar_userName.ToString();

                    byte[] b = ASCIIEncoding.ASCII.GetBytes(ar.ar_password);
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
                var data = db.Artists.Where(x => x.ar_userName.ToLower().Trim() == ar.ar_userName.ToLower().Trim() && x.ar_password.ToLower().Trim() == ar.ar_password.ToLower().Trim()).FirstOrDefault();
                if (data != null)
                {
                    Session["ar_id"] = data.ar_id;
                    Session["Artistdata"] = data.ar_userName.ToString();
                    Session["Artistimage"] = data.ar_image.ToString();
                    Session["categoryID"] = data.ar_fk_cat;
                    return RedirectToAction("Dashboard", "Artist");
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
            Session["ar_id"] = null;
            Session["Artistdata"] = null;
            Session["Artistimage"] = null;
            Session["categoryID"] = null;
            return RedirectToAction("Login");
        }
        #endregion

        #region Methods
        public ActionResult City(int CountryID)
        {
            var CityList = db.Cities.Where(x => x.ci_fk_con_id == CountryID).ToList();
            ViewBag.cityList = CityList;

            return PartialView();
        }

        public int UserNameCheck(string username)
        {
            var data = db.Artists.Where(x => x.ar_userName.ToLower() == username.ToLower()).SingleOrDefault();
            if (data != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void ArtistVault(int total_Amount)
        {
            int ar_id = int.Parse(Session["ar_id"].ToString());
            var data = db.Artists.Where(x => x.ar_id == ar_id).FirstOrDefault();
            if (data != null)
            {
                if (total_Amount > 0)
                {
                    data.Vault = total_Amount;
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        #endregion


    }
}