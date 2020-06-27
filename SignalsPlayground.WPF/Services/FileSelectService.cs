using Microsoft.Win32;

namespace SignalsPlayground.WPF.Services
{
    public interface IFileSelectService
    {
        string GetImageNameToSaveByUser();
    }

    public class FileSelectService : IFileSelectService
    {
        public string GetImageNameToSaveByUser()
        {
            var sfd = new SaveFileDialog
            {
                Title = "Save Image",
                DefaultExt = ".png",
                Filter = "Image Files|*.png",
                ValidateNames = true
            };

            if (sfd.ShowDialog().Value)
                return sfd.FileName;

            return null;
        }
    }
}
