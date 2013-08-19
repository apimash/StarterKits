using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_EchoNest_StarterKit_Phone.Data
{
    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup
    {
        public SampleDataGroup(String uniqueId, String title, String imagePath)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.ImagePath = imagePath;
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string ImagePath { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _groups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> Groups
        {
            get { return this._groups; }
        }

        public static async Task<IEnumerable<SampleDataGroup>> GetGroupsAsync()
        {
            await _sampleDataSource.GetSampleDataAsync();

            return _sampleDataSource.Groups;
        }

        public static async Task<SampleDataGroup> GetGroupAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public async Task<string> ReadFile(string fileName)
        {
            string text;
            IStorageFolder applicationFolder = ApplicationData.Current.LocalFolder;

            IStorageFile storageFile = await applicationFolder.GetFileAsync(fileName);

            IRandomAccessStream accessStream = await storageFile.OpenReadAsync();

            using (Stream stream = accessStream.AsStreamForRead((int)accessStream.Size))
            {
                byte[] content = new byte[stream.Length];
                await stream.ReadAsync(content, 0, (int)stream.Length);

                text = Encoding.UTF8.GetString(content, 0, content.Length);
            }

            return text;
        }

        private async Task GetSampleDataAsync()
        {
            if (this._groups.Count != 0)
                return;

            Uri dataUri = new Uri("ms-appx:///DataModel/SampleData.json");
            
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);

            var res = App.GetResourceStream(new Uri("DataModel/SampleData.json", UriKind.Relative));
            string jsonText = new StreamReader(res.Stream).ReadToEnd(); 

            JObject jsonObject = JObject.Parse(jsonText);
            var jsonArray = jsonObject["Groups"].AsJEnumerable();

            foreach (var groupValue in jsonArray)
            {
                SampleDataGroup group = new SampleDataGroup(groupValue["UniqueId"].ToString(),
                                                            groupValue["Title"].ToString(),                                                            
                                                            groupValue["ImagePath"].ToString());
                this.Groups.Add(group);
            }
        }
    }
}