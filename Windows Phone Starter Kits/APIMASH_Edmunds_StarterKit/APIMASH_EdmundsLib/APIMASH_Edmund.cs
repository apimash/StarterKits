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
 * These classes provide an interface to the bindable object model for Edmunds Vehicle Data
*/

namespace APIMASH_EdmundsLib
{
    public sealed class APIMASH_EdmundsCarCollection
    {
        private static readonly APIMASH_EdmundsCarCollection _cars = new APIMASH_EdmundsCarCollection();

        private readonly ObservableCollection<EdmundsMake> _all = new ObservableCollection<EdmundsMake>();
        public ObservableCollection<EdmundsMake> All
        {
            get { return this._all; }
        }

        public static APIMASH_EdmundsCarCollection Cars
        {
            get { return _cars; }
        }

        public static IEnumerable<EdmundsMake> AllMakes()
        {
            return Cars._all;
        }

        public static void Copy(MakeCollection response)
        {
            Cars.All.Clear();
            foreach (var make in response.Makes)
            {
                var eMake = new EdmundsMake(make.Id, make.Name, make.NiceName, make.Manufacturer);

                foreach (var eModel in make.Models.Select(model => new EdmundsModel(model.Link, model.Id, model.Name)))
                {
                    eMake.Models.Add(eModel);
                }

                Cars.All.Add(eMake);
            }
        }
    }

    public sealed class APIMASH_EdmundsPhotoCollection
    {
        private static readonly APIMASH_EdmundsPhotoCollection _photos = new APIMASH_EdmundsPhotoCollection();

        private readonly ObservableCollection<EdmundsPhoto> _all = new ObservableCollection<EdmundsPhoto>();
        public ObservableCollection<EdmundsPhoto> All
        {
            get { return this._all; }
        }

        public static APIMASH_EdmundsPhotoCollection Photos
        {
            get { return _photos; }
        }

        public static IEnumerable<EdmundsPhoto> AllPhotos()
        {
            return Photos._all;
        }

        public static void Copy(PhotoCollection response)
        {
            Photos._all.Clear();

            foreach (var photo in response)
            {
                var ep = new EdmundsPhoto(photo.Id, photo.CaptionTranscript, photo.SubType, photo.ShotTypeAbbreviation);
                foreach (var source in photo.PhotoSources)
                {
                    ep.Pictures.Add(@"http://media.ed.edmunds-media.com" + source);
                }

                Photos.All.Add(ep);
            }
        }
    }
}
