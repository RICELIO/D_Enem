using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class CourseListModel : BaseNopModel
    {
        public CourseListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Course.List.SearchCategoryName")]
        [AllowHtml]
        public string SearchCategoryName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Course.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}