using System;
using System.Collections.Generic;
using System.Text;
using Drinctet.ViewModels.ViewModelBase;
using Xamarin.Forms;

namespace Drinctet.Mobile
{
   public class DependenyServiceWrapper : IDependencyService
   {
       public T Get<T>() where T : class => DependencyService.Get<T>();
   }
}
