using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// course service interface
    /// </summary>
    public partial interface ICourseService
    {
        /// <summary>
        /// Delete course
        /// </summary>
        /// <param name="course">course</param>
        void DeleteCourse(Course course);

        /// <summary>
        /// Gets all course
        /// </summary>
        /// <param name="courseName">course name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>courses</returns>
        IPagedList<Course> GetAllCourse(string courseName = "", int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets all course filtered by parent course identifier
        /// </summary>
        /// <param name="parentCourseId">Parent course identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="includeAllLevels">A value indicating whether we should load all child levels</param>
        /// <returns>course</returns>
        IList<Course> GetAllCoursesByParentCourseId(int parentCourseId,
            bool showHidden = false, bool includeAllLevels = false);

        /// <summary>
        /// Gets all course displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>course</returns>
        IList<Course> GetAllCourseDisplayedOnHomePage(bool showHidden = false);

        /// <summary>
        /// Gets a course
        /// </summary>
        /// <param name="courseId">course identifier</param>
        /// <returns>course</returns>
        Course GetCourseById(int courseId);

        /// <summary>
        /// Inserts course
        /// </summary>
        /// <param name="course">course</param>
        void InsertCourse(Course course);

        /// <summary>
        /// Updates the course
        /// </summary>
        /// <param name="course">course</param>
        void UpdateCourse(Course course);

        /// <summary>
        /// Returns a list of names of not existing course
        /// </summary>
        /// <param name="courseNames">The nemes of the courses to check</param>
        /// <returns>List of names not existing course</returns>
        string[] GetNotExistingCourses(string[] courseNames);
    }
}
