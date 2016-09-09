using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Helpers;
using Nop.Admin.Models.Catalog;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class LessonsController : BaseAdminController
    {
        #region Fields

        private readonly ILessonService _lessonService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IDiscountService _discountService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IExportManager _exportManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IVendorService _vendorService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IWorkContext _workContext;
        private readonly IImportManager _importManager;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public LessonsController(ILessonService lessonService, 
            IManufacturerService manufacturerService, 
            ICustomerService customerService,
            IUrlRecordService urlRecordService, 
            IPictureService pictureService, 
            ILanguageService languageService,
            ILocalizationService localizationService, 
            ILocalizedEntityService localizedEntityService,
            IDiscountService discountService,
            IPermissionService permissionService,
            IAclService aclService, 
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IExportManager exportManager, 
            IVendorService vendorService, 
            ICustomerActivityService customerActivityService,
            CatalogSettings catalogSettings,
            IWorkContext workContext,
            IImportManager importManager, 
            ICacheManager cacheManager)
        {
            this._lessonService = lessonService;
            this._manufacturerService = manufacturerService;
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._pictureService = pictureService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._discountService = discountService;
            this._permissionService = permissionService;
            this._vendorService = vendorService;
            this._aclService = aclService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._exportManager = exportManager;
            this._customerActivityService = customerActivityService;
            this._catalogSettings = catalogSettings;
            this._workContext = workContext;
            this._importManager = importManager;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLessons))
                return AccessDeniedView();

            var model = new LessonsListModel();
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, LessonsListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLessons))
                return AccessDeniedView();

            var lessons = _lessonService.GetAllLessons(model.SearchLessonsName, model.SearchStoreId, command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = lessons.Select(x =>
                {
                    var lessonModel = x.ToModel();
                    return lessonModel;
                }),
                Total = lessons.TotalCount
            };
            return Json(gridModel);
        }
        
        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLessons))
                return AccessDeniedView();

            var model = new LessonsModel();
            model.PageSize = _catalogSettings.DefaultCategoryPageSize;
            model.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
            model.AllowCustomersToSelectPageSize = true;            

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(LessonsModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLessons))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var lesson = model.ToEntity();
                lesson.CreatedOnUtc = DateTime.UtcNow;
                lesson.UpdatedOnUtc = DateTime.UtcNow;
                _lessonService.InsertLesson(lesson);

                //search engine name
                model.SeName = lesson.ValidateSeName(model.SeName, lesson.Title, true);
                _urlRecordService.SaveSlug(lesson, model.SeName, 0);

                //activity log
                _customerActivityService.InsertActivity("AddNewLesson", _localizationService.GetResource("ActivityLog.AddNewLesson"), lesson.Title);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Lesson.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = lesson.Id });
                }
                return RedirectToAction("List");
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLessons))
                return AccessDeniedView();

            var category = _lessonService.GetLessonById(id);
            if (category == null) 
                //No category found with the specified id
                return RedirectToAction("List");

            var model = category.ToModel();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(LessonsModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLessons))
                return AccessDeniedView();

            var lesson = _lessonService.GetLessonById(model.Id);
            if (lesson == null)
                //No category found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                int prevPictureId = lesson.Id;
                lesson = model.ToEntity(lesson);
                lesson.UpdatedOnUtc = DateTime.UtcNow;
                _lessonService.UpdateLesson(lesson);

                //search engine name
                model.SeName = lesson.ValidateSeName(model.SeName, lesson.Title, true);
                _urlRecordService.SaveSlug(lesson, model.SeName, 0);                                

                //activity log
                _customerActivityService.InsertActivity("EditLesson", _localizationService.GetResource("ActivityLog.EditLesson"), lesson.Title);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Lesson.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new {id = lesson.Id});
                }
                return RedirectToAction("List");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLessons))
                return AccessDeniedView();

            var lesson = _lessonService.GetLessonById(id);
            if (lesson == null)
                //No category found with the specified id
                return RedirectToAction("List");

            _lessonService.DeleteLesson(lesson);

            //activity log
            _customerActivityService.InsertActivity("DeleteLesson", _localizationService.GetResource("ActivityLog.DeleteLesson"), lesson.Title);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Lesson.Deleted"));
            return RedirectToAction("List");
        }        

        #endregion

        #region Export / Import

        public ActionResult ExportXml()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLessons))
                return AccessDeniedView();

            try
            {
                var xml = _exportManager.ExportLessonsToXml();
                return new XmlDownloadResult(xml, "lessons.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public ActionResult ExportXlsx()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLessons))
                return AccessDeniedView();

            try
            {
                var bytes =_exportManager.ExportLessonsToXlsx(_lessonService.GetAllLessons(showHidden: true));
                 
                return File(bytes, MimeTypes.TextXlsx, "lessons.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult ImportFromXlsx()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLessons))
                return AccessDeniedView();

            //a vendor cannot import categories
            if (_workContext.CurrentVendor != null)
                return AccessDeniedView();

            try
            {
                var file = Request.Files["importexcelfile"];
                if (file != null && file.ContentLength > 0)
                {
                    _importManager.ImportCategoriesFromXlsx(file.InputStream);
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion
    }
}
