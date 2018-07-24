using System.Xml;
using Android.Content.Res;
using Drinctet.Core;
using Drinctet.Mobile.Droid.Cards;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidCardsProvider))]
namespace Drinctet.Mobile.Droid.Cards
{
    public class AndroidCardsProvider : CardsProvider
    {
        public AndroidCardsProvider()
        {
            var settings = new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment};

            var assets = Android.App.Application.Context.Assets;
            foreach (var file in assets.List(""))
            {
                using (var stream = assets.Open(file, Access.Streaming))
                using (var reader = XmlReader.Create(stream, settings))
                    AddCards(reader);
            }
        }
    }
}