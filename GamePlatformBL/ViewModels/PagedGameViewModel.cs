using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePlatformBL.ViewModels
{
    public class PagedGameViewModel
    {
        public List<GameListViewModel> Games { get; set; }
        public string GenreName { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        // filtering options
        public int SelectedGameTypeId { get; set; }
        public List<int> SelectedGenreIds { get; set; } = [];
    }

}
