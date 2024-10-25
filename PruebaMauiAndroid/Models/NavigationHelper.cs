using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaMauiAndroid.Models
{
     public interface INavigationHelper
    {

        Task PushModalAsync(Page page);
        Task PopModalAsync();
    }

    public class NavigationHelper : INavigationHelper
    {
        public async Task PushModalAsync(Page page)
        {
           if(Application.Current?.MainPage is not null)
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(page);

            }
        }

        public async Task PopModalAsync()
        {
            if (Application.Current?.MainPage is not null)
            {

                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            }
    }

}
