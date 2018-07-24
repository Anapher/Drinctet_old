using System.Linq;
using Drinctet.ViewModels.ViewModelBase;

namespace Drinctet.ViewModels
{
    public class PlayerViewModel : PropertyChangedBase
    {
        private bool _isFemale;
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(value, ref _name))
                    if (!IsGenderModifiedByUser)
                    {
                        if (_name?.Length > 0 && _name.Last() == 'a')
                            SetProperty(true, ref _isFemale, nameof(IsFemale));
                        else SetProperty(false, ref _isFemale, nameof(IsFemale));
                    }
            }
        }

        public bool IsFemale
        {
            get => _isFemale;
            set
            {
                if (SetProperty(value, ref _isFemale))
                    IsGenderModifiedByUser = true;
            }
        }

        public bool IsGenderModifiedByUser { get; private set; }
    }
}