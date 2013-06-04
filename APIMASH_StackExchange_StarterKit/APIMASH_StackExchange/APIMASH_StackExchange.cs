using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIMASH_StackExchangeLib
{
    public sealed class APIMASH_StackExchangeCollection
    {
        private static APIMASH_StackExchangeCollection _questionData = new APIMASH_StackExchangeCollection();

        private ObservableCollection<QuestionGroup> _allGroups = new ObservableCollection<QuestionGroup>();
        public ObservableCollection<QuestionGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<QuestionGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _questionData.AllGroups;
        }

        public static QuestionGroup GetGroupById(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _questionData.AllGroups.Where((group) => group.Id.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static QuestionGroup GetGroupByTitle(string title)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _questionData.AllGroups.Where((group) => group.Title.Equals(title));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static QuestionItem GetItem(int id)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _questionData.AllGroups.SelectMany(group => group.Items).Where((item) => item.Id == id);
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static void Copy(StackExchangeQuestions response, string groupId, string groupName)
        {
            try
            {
                QuestionGroup qg = APIMASH_StackExchangeCollection.GetGroupByTitle(groupName);
                if (qg != null)
                    qg.Items.Clear();
                else
                    qg = new QuestionGroup(groupId, groupName );

                response.Copy(qg);
                _questionData._allGroups.Add(qg);
            }
            catch (Exception e)
            {
                throw (e);
            }
        }
    }
}
