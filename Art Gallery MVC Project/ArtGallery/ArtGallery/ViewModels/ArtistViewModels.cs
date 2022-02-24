using ArtGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.ViewModels
{
    public class ArtistViewModel
    {
        public List<Style_of_Art> Style_Of_Arts { get; set; }
        public List<Art> Arts { get; set; }
        public string SearchTerm { get; set; }
        public Pager page { get; set; }
        public int pageNo { get; set; }
    }

    public class ArtistLoginViewModel
    {
        public string ar_userName { get; set; }
        public string ar_password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class Pager
    {
        public Pager(int totalItems, int? page, int pageSize = 10)
        {
            if (pageSize == 0) pageSize = 10;

            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            var currentPage = page != null ? (int)page : 1;
            var startPage = currentPage - 5;
            var endPage = currentPage + 4;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }

        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
    }
}