using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

/*
 *
 *  A P I   M A S H 
 *
 * These classes provide an interface to the bindable object model for Rotten Tomatoes Movie Data
*/

namespace APIMASH_RottenTomatoesLib
{
   public sealed class APIMASH_RottenTomatoesCollection
    {
        private static readonly APIMASH_RottenTomatoesCollection _movieData = new APIMASH_RottenTomatoesCollection();

        private readonly ObservableCollection<MovieGroup> _allGroups = new ObservableCollection<MovieGroup>();
        public ObservableCollection<MovieGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<MovieGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _movieData.AllGroups;
        }

        public static MovieGroup GetGroupById(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _movieData.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static MovieGroup GetGroupByTitle(string title)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _movieData.AllGroups.Where((group) => group.Title.Equals(title));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static MovieItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _movieData.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static void Copy(RottenTomatoesMovies response, string groupId, string groupName)
        {
            try
            {
                MovieGroup mg = APIMASH_RottenTomatoesCollection.GetGroupByTitle(groupName);
                if (mg != null)
                    mg.Items.Clear();
                else
                    mg = new MovieGroup(groupId, groupName, response.Movies[0].Posters.Original);

                foreach (var mi in response.Movies.Select(t => new MovieItem(
                    t.Id,
                    t.Title,
                    t.MPAARating,
                    t.Ratings.AudienceRating,
                    t.Ratings.CriticsRating,
                    t.Links.Clips,
                    t.Links.Reviews,
                    t.Links.Cast,
                    t.Posters.Original,
                    t.Synopsis,
                    mg)))
                {
                    mg.Items.Add(mi);
                }
                _movieData._allGroups.Add(mg);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
