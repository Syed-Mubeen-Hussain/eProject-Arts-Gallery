using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtGallery.Models;

namespace ArtGallery.ViewModels
{
    public class AdminLoginViewModel
    {
        public string ad_userName { get; set; }
        public string ad_password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class AdminCategoryViewModel
    {
        public List<Category> categories { get; set; }
        public string SearchTerm { get; set; }
        public Pager page { get; set; }
        public int pageNo { get; set; }
    }

    public class AdminArtViewModel
    {
        public List<Art> Arts { get; set; }
        public List<Style_of_Art> Style_Of_Arts { get; set; }
        public string SearchTerm { get; set; }
        public Pager page { get; set; }
        public int pageNo { get; set; }
    }

    public class AdminArtistViewModel
    {
        public List<Artist> Artist { get; set; }
        public string SearchTerm { get; set; }
        public Pager page { get; set; }
        public int pageNo { get; set; }
    }
    
    public class AdminOrderViewModel
    {
        public List<order> Orders { get; set; }
        public Category category { get; set; }
        public string SearchTerm { get; set; }
        public Pager page { get; set; }
        public int pageNo { get; set; }
        public List<Website_fund> websiteFund { get; set; }
    }

    public class AdminOrder_detailsViewModel
    {
        public List<order_details> Order_Details { get; set; }
        public Customer Customer { get; set; }
        public order Order { get; set; }
    }

    public class AdminOrderChangeStatusViewModel
    {
        public Customer Customer { get; set; }
        public order Order { get; set; }
    }

    public class AdminCustomerViewModel
    {
        public List<Customer> Customers { get; set; }
        public string SearchTerm { get; set; }
        public Pager page { get; set; }
        public int pageNo { get; set; }
    }

    public class AdminContactViewModel
    {
        public List<Contact> Contact { get; set; }
        public string SearchTerm { get; set; }
        public Pager page { get; set; }
        public int pageNo { get; set; }
    }

    public class AdminExhibitionViewModel
    {
        public List<Exhibition> exhibitions { get; set; }
        public List<Art> Arts { get; set; }
        public string SearchTerm { get; set; }
        public Pager page { get; set; }
        public int pageNo { get; set; }
    }

    public class AdminArtExhibitionViewModel
    {
        public List<Art_exhibition> Art_exhibitions { get; set; }
        public string SearchTerm { get; set; }
        public Pager page { get; set; }
        public int pageNo { get; set; }
    }
}