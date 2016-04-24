using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using UWPApp.Models;
using UWPApp.Storage;

namespace UWPApp.ViewModels
{
    public class PicturesViewModel : ViewModelBase
    {
        public ObservableCollection<PictureModel> Pictures { get; set; } = new ObservableCollection<PictureModel>();
        
        public async Task LoadPicturesAsync()
        {
            // TODO: yield iterator
            var pictures = await PicturesRepository.GetPictures();

            Pictures.Clear();

            foreach (var picture in pictures)
            {
                Pictures.Add(picture);
            }
        }
    }
}
