using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Models.Entities;
using IdentityServer.Models;
using IdentityServer.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Admin.StaffList
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IUserManager _userManager;

        private readonly int _pageSize = 15;

        public List<User> Moderators { get; set; } = [];
        public string Search { get; set; }
        public string SortColumn { get; set; } = "Username";
        public string SortDirection { get; set; } = "asc";
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }

        public IndexModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task OnGetAsync(string? search, string? sortColumn, string? sortDirection, int? pageNumber)
        {
            Search = search ?? "";
            SortColumn = sortColumn ?? "Username";
            SortDirection = sortDirection ?? "asc";
            PageNumber = pageNumber ?? 1;

            StaffListSpecifications specifications = new()
            {
                Search = this.Search,
                SortColumn = this.SortColumn,
                SortDirection = this.SortDirection,
                PageNumber = this.PageNumber,
                PageSize = _pageSize
            };

            PaginatedList<User> moderatorsList = await _userManager.GetStaffListAsync(specifications);

            Moderators = moderatorsList.Items;
            this.PageNumber = moderatorsList.Pagination.CurrentPage;
            this.TotalPages = moderatorsList.Pagination.TotalPages;
        }
        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            await _userManager.DeleteUserAsync(id);

            return RedirectToPage();
        }

        public string GetSortDirection(string column)
        {
            return SortColumn == column && SortDirection == "asc" ? "desc" : "asc";
        }

        public string GetSortIcon(string column)
        {
            if (SortColumn != column) return "";
            return SortDirection == "asc" ? "↑" : "↓";
        }
    }
}
