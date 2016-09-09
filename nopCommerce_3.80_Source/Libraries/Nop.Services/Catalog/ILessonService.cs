using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Category service interface
    /// </summary>
    public partial interface ILessonService
    {
        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="lesson">Lesson</param>
        void DeleteLesson(Lesson lesson);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="lessonName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IPagedList<Lesson> GetAllLessons(string lessonName = "", int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="lessonId">Category identifier</param>
        /// <returns>Category</returns>
        Lesson GetLessonById(int lessonId);

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="lesson">Lesson</param>
        void InsertLesson(Lesson lesson);

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="lesson">Lesson</param>
        void UpdateLesson(Lesson lesson);      
    }
}
