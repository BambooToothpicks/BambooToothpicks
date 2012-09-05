using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BambooToothpicks {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page {
        public MainPage() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo( NavigationEventArgs e ) {
        }

        private void ItemListView_SelectionChanged( object sender, SelectionChangedEventArgs e ) {
            // If there's a selected item (in AddedItems)
            // show it in the WebView.
            if( e.AddedItems.Count > 0 ) {
                FeedItem feedItem = e.AddedItems[0] as FeedItem;
                ContentView.NavigateToString( feedItem.Link );
            }
        }

        private void Button_Click_1( object sender, RoutedEventArgs e ) {
            GetFeed();
        }

        public async void GetFeed() {
            //http://www.ezrss.it/search/index.php?show_name=Heroes&date=&quality=&release_group=&mode=atom20
            string url = txtRssUrl.Text;

            if( url != string.Empty ) {
                XElement rssFeed = XElement.Load( url );

                var items = from item in rssFeed.Elements( "channel" ).Elements( "item" )
                            select item;

                List<FeedData> feedDatas = ( from x in rssFeed.Elements( "channel" )
                                             select new FeedData() {
                                                 Title = x.Element( "title" ).Value.ToString(),
                                                 Description = x.Element( "description" ).Value.ToString(),
                                                 Items = ( from y in x.Elements( "item" )
                                                           select new FeedItem() {
                                                               Title = y.Element( "title" ).Value.ToString(),
                                                               Link = y.Element( "link" ).Value.ToString(),
                                                               Category = y.Element( "category" ).Value.ToString(),
                                                               Description = y.Element( "description" ).Value.ToString(),
                                                               PubDate = y.Element( "pubDate" ).Value.ToString()
                                                           } ).ToList()
                                             } ).ToList();

                if( feedDatas.Count() > 0 )
                    this.DataContext = feedDatas.First();
            }
        }
    }
}
