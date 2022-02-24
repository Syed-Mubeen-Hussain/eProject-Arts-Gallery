using ArtGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Art> Arts { get; set; }
        public List<Auction> AuctionArt { get; set; }
    }

    public class CategoryArtsViewModel
    {
        public string searchTerm { get; set; }
        public List<Art> Arts { get; set; }
        public int numberOfArts { get; set; }
    }

    public class HomeExhibitionArtsViewModel
    {
        public string searchTerm { get; set; }
        public List<Art> Arts { get; set; }
        public Exhibition Exhibition { get; set; }
        public int numberOfArts { get; set; }
    }
    
    public class CartViewModel
    {
        public List<Art> Arts { get; set; }
        public Category Category { get; set; }
        public Customer customer { get; set; }
    }

    public class CheckoutViewModel
    {
        public List<Art> Art { get; set; }
        public Customer customer { get; set; }
        public int totalAmount { get; set; }
        public string address { get; set; }
        public int phone { get; set; }
        public string email { get; set; }
        public int zip { get; set; }
        public int credit_number { get; set; }
        public int pin { get; set; }
    }

    public class ShowOrderViewModel
    {
        public List<order> Orders { get; set; }
        public List<order_details> OrdersDetails { get; set; }
    }
}