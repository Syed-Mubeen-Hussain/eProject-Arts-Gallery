using ArtGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.ViewModels
{
    public class ShopViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Art> Arts { get; set; }
        public Pager page { get; set; }
        public int? sortBy { get; set; }
        public int? categoryID { get; set; }
        public int pageSize { get; set; }
        public int MaximumPrice { get; set; }
        public string searchTerm { get; set; }
    }

    public class FilterArtsViewModel
    {
        public List<Art> Arts { get; set; }
        public Pager page { get; set; }
        public int? sortBy { get; set; }
        public int? categoryID { get; set; }
        public int pageSize { get; set; }
        public int? maximumPrice { get; set; }
        public int? minimumPrice { get; set; }
        public string searchTerm { get; set; }
    }

    public class SingleArtViewModel
    {
        public Category Category { get; set; }
        public Art Art { get; set; }
        public List<Art> RelatedArts { get; set; }
        public int auctionId { get;  set; }
        public List<Bid> bids { get; set; }
        public Bid highestBid { get; set; }
        public Auction endDate { get; set; }
        public List<Country> Country { get; set; }
        public Artist Artist { get; set; }
        public Art_Views artViews { get; set; }
        public List<Art_Comment> comments { get; set; }
        public List<Customer> customer { get; set; }
    }
}