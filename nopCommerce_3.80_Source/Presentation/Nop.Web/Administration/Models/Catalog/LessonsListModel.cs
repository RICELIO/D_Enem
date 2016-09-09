using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class LessonsListModel : BaseNopModel
    {
        public LessonsListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Lessons.List.SearchLessonsName")]
        [AllowHtml]
        public string SearchLessonsName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Lessons.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}