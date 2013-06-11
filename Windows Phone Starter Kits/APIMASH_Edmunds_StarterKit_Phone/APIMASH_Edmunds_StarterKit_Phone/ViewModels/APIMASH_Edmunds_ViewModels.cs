using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using APIMASH_EdmundsLib;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_Edmunds_StarterKit_Phone.ViewModels
{
    /// <summary>
    /// APIMASH_Edmunds_Photo_ViewModel - ViewModel for Vehicle Images
    /// </summary>
    public class APIMASH_Edmunds_Photo_ViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<EdmundsPhoto> Photos = new ObservableCollection<EdmundsPhoto>();

        public ObservableCollection<EdmundsPhoto> PhotoCollection
        {
            get { return Photos; }
        }

        public void Copy(PhotoCollection response)
        {
            Photos.Clear();

            foreach (var photo in response)
            {
                var ep = new EdmundsPhoto(photo.Id, photo.CaptionTranscript, photo.SubType, photo.ShotTypeAbbreviation);
                foreach (var source in photo.PhotoSources)
                {
                    ep.Pictures.Add(@"http://media.ed.edmunds-media.com" + source);
                }

                Photos.Add(ep);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            var handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
   
    /// <summary>
    /// APIMASH_Edmunds_MakeModel_ViewModel - ViewModel for makes and models
    /// </summary>
    public class APIMASH_Edmunds_MakeModel_ViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<EdmundsMake> Makes { get; private set; }

        public IEnumerable<EdmundsMake> MakeCollection 
        {
            get { return Makes; }
        }

        public APIMASH_Edmunds_MakeModel_ViewModel()
        {
            this.Makes = new ObservableCollection<EdmundsMake>();
        }

        public void Copy(MakeCollection response)
        {
            Makes.Clear();
            foreach (var make in response.Makes)
            {
                var eMake = new EdmundsMake(make.Id, make.Name, make.NiceName, make.Manufacturer);

                foreach (var eModel in make.Models.Select(model => new EdmundsModel(model.Link, model.Id, model.Name)))
                {
                    eMake.Models.Add(eModel);
                }

                Makes.Add(eMake);
            }
        }

        public bool IsDataLoaded
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}