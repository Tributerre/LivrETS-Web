/*
    LivrETS - Centralized system that manages selling of pre-owned ETS goods.
    Copyright (C) 2016  TribuTerre

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LivrETS.Models;

namespace LivrETS.Repositories
{
    /// <summary>
    /// Only repository of the system for now. Add a new one if needed.
    /// </summary>
    public class LivrETSRepository : IDisposable
    {
        private ApplicationDbContext _db;

        public LivrETSRepository()
        {
            _db = new ApplicationDbContext();
        }

        /// <summary>
        /// Returns an enumerable of all courses in the system.
        /// </summary>
        /// <returns>An enumerable of Course. Not Null.</returns>
        public IEnumerable<Course> GetAllCourses()
        {
            return (
                from course in _db.Courses
                orderby course.Acronym ascending
                select course
            );
        }

        /// <summary>
        /// Gets the course associated with a specific acronym.
        /// </summary>
        /// <param name="acronym">The acronym to test.</param>
        /// <returns>A Course or null if not found.</returns>
        public Course GetCourseByAcronym(string acronym)
        {
            return (
                from course in _db.Courses
                where course.Acronym == acronym
                select course
            ).FirstOrDefault();
        }

        /// <summary>
        /// Adds a new course to the system.
        /// </summary>
        /// <param name="acronym">The acronym of the new course.</param>
        public void AddNewCourse(string acronym, string title)
        {
            Course course = new Course() { Acronym = acronym, Title = title };
            _db.Courses.Add(course);
            _db.SaveChanges();
        }

        /// <summary>
        /// Add a new offer to a user.
        /// </summary>
        /// <param name="toUser">The id of the user in which to add the offer.</param>
        /// <param name="offer">The offer to add.</param>
        public void AddOffer(Offer offer, string toUser)
        {
            var user = (
                from userDb in _db.Users
                where userDb.Id.ToString() == toUser
                select userDb
            ).FirstOrDefault();

            if (user != null)
            {
                user.Offers.Add(offer);
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Get the current fair (current date between StartDate and EndDate) if there is one.
        /// </summary>
        /// <returns>A Fair or null if not found.</returns>
        public Fair GetCurrentFair()
        {
            var now = DateTime.Now;
            return (
                from fairdb in _db.Fairs
                where fairdb.StartDate <= now && now <= fairdb.EndDate
                select fairdb
            ).FirstOrDefault();
        }

        /// <summary>
        /// Gets a user by its bar code.
        /// </summary>
        /// <param name="BarCode">The bar code of the user to retrieve.</param>
        /// <returns>An ApplicationUser or null if not found.</returns>
        public ApplicationUser GetUserBy(string BarCode)
        {
            return (
                from user in _db.Users
                where user.BarCode == BarCode
                select user
            ).FirstOrDefault();
        }

        public void Update()
        {
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}