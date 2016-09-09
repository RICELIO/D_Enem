using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Category service
    /// </summary>
    public partial class LessonService : ILessonService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category ID
        /// </remarks>
        private const string LESSON_BY_ID_KEY = "Nop.lesson.id-{0}";
        #endregion

        #region Fields

        private readonly IRepository<Lesson> _lessonRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAclService _aclService;
        private readonly CatalogSettings _catalogSettings;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="categoryRepository">Category repository</param>
        /// <param name="productCategoryRepository">ProductCategory repository</param>
        /// <param name="productRepository">Product repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="catalogSettings">Catalog settings</param>
        public LessonService(
            ICacheManager cacheManager,
            IRepository<Lesson> lessonRepository,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IWorkContext workContext,
            IStoreContext storeContext,
            IEventPublisher eventPublisher,
            IStoreMappingService storeMappingService,
            IAclService aclService,
            CatalogSettings catalogSettings)
        {
            this._cacheManager = cacheManager;
            this._lessonRepository = lessonRepository;
            this._aclRepository = aclRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._eventPublisher = eventPublisher;
            this._storeMappingService = storeMappingService;
            this._aclService = aclService;
            this._catalogSettings = catalogSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="lesson">Category</param>
        public virtual void DeleteLesson(Lesson lesson)
        {
            if (lesson == null)
                throw new ArgumentNullException("lesson");

            _lessonRepository.Delete(lesson);

            //cache
            _cacheManager.RemoveByPattern(LESSON_BY_ID_KEY);

            //event notification
            _eventPublisher.EntityUpdated(lesson);
        }
        
        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IPagedList<Lesson> GetAllLessons(string lessonName = "", int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _lessonRepository.Table;
            if (!String.IsNullOrWhiteSpace(lessonName))
                query = query.Where(c => c.Title.Contains(lessonName));

            //paging
            return new PagedList<Lesson>(query.ToList(), pageIndex, pageSize);
        }
                
        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category</returns>
        public virtual Lesson GetLessonById(int lessonId)
        {
            if (lessonId == 0)
                return null;
            
            string key = string.Format(LESSON_BY_ID_KEY, lessonId);
            return _cacheManager.Get(key, () => _lessonRepository.GetById(lessonId));
        }

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="lesson">Category</param>
        public virtual void InsertLesson(Lesson lesson)
        {
            if (lesson == null)
                throw new ArgumentNullException("lesson");

            _lessonRepository.Insert(lesson);

            //cache
            _cacheManager.RemoveByPattern(LESSON_BY_ID_KEY);

            //event notification
            _eventPublisher.EntityInserted(lesson);
        }

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="lesson">Category</param>
        public virtual void UpdateLesson(Lesson lesson)
        {
            if (lesson == null)
                throw new ArgumentNullException("lesson");

            _lessonRepository.Update(lesson);

            //cache
            _cacheManager.RemoveByPattern(LESSON_BY_ID_KEY);

            //event notification
            _eventPublisher.EntityUpdated(lesson);
        }
        
        /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="categoryNames">The nemes of the categories to check</param>
        /// <returns>List of names not existing categories</returns>
        public virtual string[] GetNotExistingCategories(string[] categoryNames)
        {
            if (categoryNames == null)
                throw new ArgumentNullException("categoryNames");

            var query = _lessonRepository.Table;
            var queryFilter = categoryNames.Distinct().ToArray();
            var filter = query.Select(c => c.Title).Where(c => queryFilter.Contains(c)).ToList();

            return queryFilter.Except(filter).ToArray();
        }

        #endregion
    }
}
