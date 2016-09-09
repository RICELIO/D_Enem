using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    [Validator(typeof(LessonValidator))]
    public partial class LessonsModel : BaseNopEntityModel
    {
        public LessonsModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }
        }

        [NopResourceDisplayName("Admin.Catalog.Lessons.Fields.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Lessons.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }        

        public int PageSize { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Lessons.Fields.AllowCustomersToSelectPageSize")]
        public bool AllowCustomersToSelectPageSize { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Lessons.Fields.PageSizeOptions")]
        public string PageSizeOptions { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Lesson.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Lesson.Fields.Deleted")]
        public bool Deleted { get; set; }
    }
}