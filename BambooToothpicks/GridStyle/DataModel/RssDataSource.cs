using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Data.Xml.Dom;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace GridStyle.Data
{
    /// <summary>
    /// Base class for <see cref="RssDataItem"/> and <see cref="RssDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class RssDataCommon : GridStyle.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public RssDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(RssDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class RssDataItem : RssDataCommon
    {
        public RssDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, RssDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private RssDataGroup _group;
        public RssDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class RssDataGroup : RssDataCommon
    {
        public RssDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
        }

        private ObservableCollection<RssDataItem> _items = new ObservableCollection<RssDataItem>();
        public ObservableCollection<RssDataItem> Items
        {
            get { return this._items; }
        }
        
        public IEnumerable<RssDataItem> TopItems
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed
            get { return this._items.Take(12); }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// </summary>
    public sealed class RssDataSource
    {
        private static RssDataSource _RssDataSource = new RssDataSource();

        private ObservableCollection<RssDataGroup> _allGroups = new ObservableCollection<RssDataGroup>();
        public ObservableCollection<RssDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<RssDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _RssDataSource.AllGroups;
        }

        public static RssDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _RssDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static RssDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _RssDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public RssDataSource()
        {
            System.Uri ur = new Uri("http://rss.bt-chat.com/?group=3&cat=9");
            //System.Uri ur = new Uri("http://www.ezrss.it/search/index.php?show_name=Heroes&date=&quality=&release_group=&mode=atom20");
            var xmlDocument = XmlDocument.LoadFromUriAsync(ur);
            while (xmlDocument.Status == Windows.Foundation.AsyncStatus.Started) { ;}

            var xdoc = xmlDocument.GetResults();

            XmlNodeList videosList = xdoc.GetElementsByTagName("item");
           /*
     <item>

		<title><![CDATA[Royal.Pains.S04E12.HDTV.x264-ASAP.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=162035]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=162035</guid>
		<link>http://www.bt-chat.com/download1.php?id=162035</link>
		<pubDate>Wed, 05 Sep 2012 22:34:49 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=162035" length="388989914" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Royal.Pains.S04E12.HDTV.x264-ASAP.[eztv].torrent</fileName>
			<contentLength>388989914</contentLength>
			<infoHash>56ab3051cd0e7927ab63ba2d3a4952be98d14d7c</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:56ab3051cd0e7927ab63ba2d3a4952be98d14d7c&dn=Royal Pains S04E12 HDTV x264-ASAP [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>*/
            var group1 = new RssDataGroup(xdoc.SelectSingleNode("/rss/channel/description").InnerText,
                    xdoc.SelectSingleNode("/rss/channel/title").InnerText,
                    string.Empty,
                    xdoc.SelectSingleNode("/rss/channel/image/url").InnerText,
                    string.Empty);
            foreach (IXmlNode xn in videosList)
            {

                var title = xn.SelectSingleNode("title").InnerText;

                //Image
                

                //ID
                
                var id = xn.SelectSingleNode("link").InnerText;




                string description = xn.SelectSingleNode("description").InnerText;

                group1.Items.Add(new RssDataItem(id,
                    title,
                    string.Empty,
                    string.Empty,
                    description,
                    string.Empty,
                    group1));
            }





            this.AllGroups.Add(group1);

        }
    }
}


/*
b3a0


	<rss version="2.0" xmlns:atom="http://www.w3.org/2005/Atom">
	<!-- http://rss.bt-chat.com/?group=3&cat=9 | 20120905 214628 EST -->
	<channel>
	<title>BT-Chat.com</title>
	<description>Bt-Chat Latest Torrents</description>
	<link>http://www.bt-chat.com</link>
	<language>en-us</language>
	<ttl>15</ttl>
	<image>
		<title>BT-Chat.com</title>
		<url>http://www.bt-chat.com/images/logo.png</url>
		<link>http://www.bt-chat.com</link>
	</image>
	<atom:link href="http://rss.bt-chat.com/" rel="self" type="application/rss+xml" />
	<item>

		<title><![CDATA[Royal.Pains.S04E12.HDTV.x264-ASAP.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=162035]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=162035</guid>
		<link>http://www.bt-chat.com/download1.php?id=162035</link>
		<pubDate>Wed, 05 Sep 2012 22:34:49 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=162035" length="388989914" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Royal.Pains.S04E12.HDTV.x264-ASAP.[eztv].torrent</fileName>
			<contentLength>388989914</contentLength>
			<infoHash>56ab3051cd0e7927ab63ba2d3a4952be98d14d7c</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:56ab3051cd0e7927ab63ba2d3a4952be98d14d7c&dn=Royal Pains S04E12 HDTV x264-ASAP [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Gates.S01E04.HDTV.x264-TLA.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=162033]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=162033</guid>
		<link>http://www.bt-chat.com/download1.php?id=162033</link>
		<pubDate>Wed, 05 Sep 2012 19:59:01 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=162033" length="222342249" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Gates.S01E04.HDTV.x264-TLA.[eztv].torrent</fileName>
			<contentLength>222342249</contentLength>
			<infoHash>95622a14dd89ee9f4eb6281b51bc9ceef1b5be42</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:95622a14dd89ee9f4eb6281b51bc9ceef1b5be42&dn=Gates S01E04 HDTV x264-TLA [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Conan.2012.09.04.David.Mizejewski.HDTV.x264-BAJSKORV.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=162028]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=162028</guid>
		<link>http://www.bt-chat.com/download1.php?id=162028</link>
		<pubDate>Wed, 05 Sep 2012 19:20:09 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=162028" length="238566233" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Conan.2012.09.04.David.Mizejewski.HDTV.x264-BAJSKORV.[eztv].torrent</fileName>
			<contentLength>238566233</contentLength>
			<infoHash>4f6724ebca85c385cce686c94c6c59753ac1ef86</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:4f6724ebca85c385cce686c94c6c59753ac1ef86&dn=Conan 2012 09 04 David Mizejewski HDTV x264-BAJSKORV [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[The.Exes.S02E11.HDTV.x264-2HD.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161998]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161998</guid>
		<link>http://www.bt-chat.com/download1.php?id=161998</link>
		<pubDate>Wed, 05 Sep 2012 03:12:41 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161998" length="184415797" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>The.Exes.S02E11.HDTV.x264-2HD.[eztv].torrent</fileName>
			<contentLength>184415797</contentLength>
			<infoHash>56c45505761f402d8d5b1fc3e86b34cced4e487e</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:56c45505761f402d8d5b1fc3e86b34cced4e487e&dn=The Exes S02E11 HDTV x264-2HD [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[World.Without.End.S01E01.720p.HDTV.x264-2HD.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161997]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161997</guid>
		<link>http://www.bt-chat.com/download1.php?id=161997</link>
		<pubDate>Wed, 05 Sep 2012 02:47:20 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161997" length="1341356445" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>World.Without.End.S01E01.720p.HDTV.x264-2HD.[eztv].torrent</fileName>
			<contentLength>1341356445</contentLength>
			<infoHash>7c4721d2886424ac1b989a111fec33fb3404ec56</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:7c4721d2886424ac1b989a111fec33fb3404ec56&dn=ReelzChannel World.Without.End.S01E01.HDTV.x264-2HD]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Coma.2012.Part.Two.HDTV.x264-2HD.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161994]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161994</guid>
		<link>http://www.bt-chat.com/download1.php?id=161994</link>
		<pubDate>Wed, 05 Sep 2012 02:04:26 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161994" length="604457159" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Coma.2012.Part.Two.HDTV.x264-2HD.[eztv].torrent</fileName>
			<contentLength>604457159</contentLength>
			<infoHash>7a33c1e57f036e1b38932770226a04cb4c2e7d51</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:7a33c1e57f036e1b38932770226a04cb4c2e7d51&dn=Coma 2012 Part Two HDTV x264-2HD [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[World.Without.End.S01E01.Knight.HDTV.x264-2HD.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161993]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161993</guid>
		<link>http://www.bt-chat.com/download1.php?id=161993</link>
		<pubDate>Wed, 05 Sep 2012 00:29:22 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161993" length="406969660" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>World.Without.End.S01E01.Knight.HDTV.x264-2HD.[eztv].torrent</fileName>
			<contentLength>406969660</contentLength>
			<infoHash>e47f0a974dcfaa79e13504208c9e7de6740a03a3</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:e47f0a974dcfaa79e13504208c9e7de6740a03a3&dn=ReelzChannel World.Without.End.S01E01.HDTV.x264-2HD]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Covert.Affairs.S03E08.HDTV.x264-ASAP.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161992]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161992</guid>
		<link>http://www.bt-chat.com/download1.php?id=161992</link>
		<pubDate>Tue, 04 Sep 2012 23:22:06 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161992" length="378550745" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Covert.Affairs.S03E08.HDTV.x264-ASAP.[eztv].torrent</fileName>
			<contentLength>378550745</contentLength>
			<infoHash>eba41718bd784142554073f271119ff53d83d1f7</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:eba41718bd784142554073f271119ff53d83d1f7&dn=Covert Affairs S03E08 HDTV x264-ASAP [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[White.Collar.S04E08.HDTV.x264-ASAP.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161991]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161991</guid>
		<link>http://www.bt-chat.com/download1.php?id=161991</link>
		<pubDate>Tue, 04 Sep 2012 22:19:10 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161991" length="327953439" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>White.Collar.S04E08.HDTV.x264-ASAP.[eztv].torrent</fileName>
			<contentLength>327953439</contentLength>
			<infoHash>431f42e9d64939ec158b60717a5c2cc14ceedaf8</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:431f42e9d64939ec158b60717a5c2cc14ceedaf8&dn=White Collar S04E08 HDTV x264-ASAP [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Deadliest.Catch.S08.Special.Revelations.HDTV.x264-KILLERS.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161990]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161990</guid>
		<link>http://www.bt-chat.com/download1.php?id=161990</link>
		<pubDate>Tue, 04 Sep 2012 22:06:13 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161990" length="306682518" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Deadliest.Catch.S08.Special.Revelations.HDTV.x264-KILLERS.[eztv].torrent</fileName>
			<contentLength>306682518</contentLength>
			<infoHash>3a50ab8eb448cdf06f73edae839dd9e50b3c7f59</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:3a50ab8eb448cdf06f73edae839dd9e50b3c7f59&dn=Deadliest Catch S08 Special Revelations HDTV x264-KILLERS [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Hells.Kitchen.US.S10E19.PDTV.x264-LOL.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161989]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161989</guid>
		<link>http://www.bt-chat.com/download1.php?id=161989</link>
		<pubDate>Tue, 04 Sep 2012 21:19:54 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161989" length="325811452" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Hells.Kitchen.US.S10E19.PDTV.x264-LOL.[eztv].torrent</fileName>
			<contentLength>325811452</contentLength>
			<infoHash>6315a053ba5dd33cfaa89ce93e242224a757d191</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:6315a053ba5dd33cfaa89ce93e242224a757d191&dn=Hells Kitchen US S10E19 PDTV x264-LOL [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Dirty.Jobs.S09E02.REAL.REPACK.HDTV.x264-KILLERS.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161988]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161988</guid>
		<link>http://www.bt-chat.com/download1.php?id=161988</link>
		<pubDate>Tue, 04 Sep 2012 21:15:44 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161988" length="471195661" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Dirty.Jobs.S09E02.REAL.REPACK.HDTV.x264-KILLERS.[eztv].torrent</fileName>
			<contentLength>471195661</contentLength>
			<infoHash>a1eba95dfeefd7e3f6cf2343b2fc94c6c089f122</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:a1eba95dfeefd7e3f6cf2343b2fc94c6c089f122&dn=Dirty Jobs S09E02 REAL REPACK HDTV x264-KILLERS [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Bad.Education.S01E03.720p.HDTV.x264-BiA.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161987]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161987</guid>
		<link>http://www.bt-chat.com/download1.php?id=161987</link>
		<pubDate>Tue, 04 Sep 2012 21:12:52 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161987" length="689268314" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Bad.Education.S01E03.720p.HDTV.x264-BiA.[eztv].torrent</fileName>
			<contentLength>689268314</contentLength>
			<infoHash>6bbadd80049c1e97f103b38be25e5c011f39678b</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:6bbadd80049c1e97f103b38be25e5c011f39678b&dn=BBC Bad.Education.S01E03.HDTV.x264-BiA]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Bad.Education.1x04.School.Trip.HDTV.x264-FoV.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161981]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161981</guid>
		<link>http://www.bt-chat.com/download1.php?id=161981</link>
		<pubDate>Tue, 04 Sep 2012 20:21:29 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161981" length="280403368" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Bad.Education.1x04.School.Trip.HDTV.x264-FoV.[eztv].torrent</fileName>
			<contentLength>280403368</contentLength>
			<infoHash>e1f2e1af78025fd9a44dc907a415a109a9a1906c</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:e1f2e1af78025fd9a44dc907a415a109a9a1906c&dn=BBC Bad.Education.1x04.HDTV.x264-FoV]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Citizen.Khan.1x01.HDTV.x264-FoV.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161978]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161978</guid>
		<link>http://www.bt-chat.com/download1.php?id=161978</link>
		<pubDate>Tue, 04 Sep 2012 20:12:01 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161978" length="266008476" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Citizen.Khan.1x01.HDTV.x264-FoV.[eztv].torrent</fileName>
			<contentLength>266008476</contentLength>
			<infoHash>d458f6c6a074ec1826bd26abe499884a588f475a</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:d458f6c6a074ec1826bd26abe499884a588f475a&dn=BBC Citizen.Khan.1x01.HDTV.x264-FoV]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Accused.2x04.Tinas.Story.HDTV.x264-FoV.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161968]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161968</guid>
		<link>http://www.bt-chat.com/download1.php?id=161968</link>
		<pubDate>Tue, 04 Sep 2012 17:16:58 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161968" length="324197764" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Accused.2x04.Tinas.Story.HDTV.x264-FoV.[eztv].torrent</fileName>
			<contentLength>324197764</contentLength>
			<infoHash>4aa7751ebec9492233de812c74463fd91bdc6317</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:4aa7751ebec9492233de812c74463fd91bdc6317&dn=BBC Accused.2x04.HDTV.x264-FoV]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Citizen.Khan.1x02.HDTV.x264-FoV.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161963]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161963</guid>
		<link>http://www.bt-chat.com/download1.php?id=161963</link>
		<pubDate>Tue, 04 Sep 2012 14:46:53 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161963" length="298486024" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Citizen.Khan.1x02.HDTV.x264-FoV.[eztv].torrent</fileName>
			<contentLength>298486024</contentLength>
			<infoHash>1ecf3aae7306acc5cf3a30f8757cd23da13af22f</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:1ecf3aae7306acc5cf3a30f8757cd23da13af22f&dn=BBC Citizen.Khan.1x02.HDTV.x264-FoV]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Switched.at.Birth.S01E23.HDTV.x264-ASAP.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161927]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161927</guid>
		<link>http://www.bt-chat.com/download1.php?id=161927</link>
		<pubDate>Tue, 04 Sep 2012 03:16:09 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161927" length="348680105" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Switched.at.Birth.S01E23.HDTV.x264-ASAP.[eztv].torrent</fileName>
			<contentLength>348680105</contentLength>
			<infoHash>2ee02d98e19ed3c3eae2b66eb4a65753434f94c6</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:2ee02d98e19ed3c3eae2b66eb4a65753434f94c6&dn=Switched at Birth S01E23 HDTV x264-ASAP [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Coma.2012.Part.One.HDTV.x264-2HD.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161918]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161918</guid>
		<link>http://www.bt-chat.com/download1.php?id=161918</link>
		<pubDate>Tue, 04 Sep 2012 01:44:02 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161918" length="614794612" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Coma.2012.Part.One.HDTV.x264-2HD.[eztv].torrent</fileName>
			<contentLength>614794612</contentLength>
			<infoHash>f3197845e73e8c0bc00449711c210d4708178f08</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:f3197845e73e8c0bc00449711c210d4708178f08&dn=Coma 2012 Part One HDTV x264-2HD [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Grimm.S02E04.720p.HDTV.x264-EVOLVE.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161916]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161916</guid>
		<link>http://www.bt-chat.com/download1.php?id=161916</link>
		<pubDate>Tue, 04 Sep 2012 00:13:34 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161916" length="1078465121" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Grimm.S02E04.720p.HDTV.x264-EVOLVE.[eztv].torrent</fileName>
			<contentLength>1078465121</contentLength>
			<infoHash>860556ea5368d785055c88e12034e76f23b69222</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:860556ea5368d785055c88e12034e76f23b69222&dn=Grimm S02E04 720p HDTV x264-EVOLVE [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[The.Inbetweeners.US.S01E03.HDTV.x264-2HD.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161915]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161915</guid>
		<link>http://www.bt-chat.com/download1.php?id=161915</link>
		<pubDate>Mon, 03 Sep 2012 23:53:10 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161915" length="235946956" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>The.Inbetweeners.US.S01E03.HDTV.x264-2HD.[eztv].torrent</fileName>
			<contentLength>235946956</contentLength>
			<infoHash>85c3c9743e8743689943b6627c343e596f92230f</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:85c3c9743e8743689943b6627c343e596f92230f&dn=The Inbetweeners US S01E03 HDTV x264-2HD [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Hotel.Hell.S01E06.HDTV.x264-LOL.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161911]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161911</guid>
		<link>http://www.bt-chat.com/download1.php?id=161911</link>
		<pubDate>Mon, 03 Sep 2012 22:51:05 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161911" length="334207330" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Hotel.Hell.S01E06.HDTV.x264-LOL.[eztv].torrent</fileName>
			<contentLength>334207330</contentLength>
			<infoHash>0b37e4caf8b963d53051190640a78aa6bd71bc78</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:0b37e4caf8b963d53051190640a78aa6bd71bc78&dn=Hotel Hell S01E06 HDTV x264-LOL [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Hotel.Hell.S01E05.HDTV.x264-LOL.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161908]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161908</guid>
		<link>http://www.bt-chat.com/download1.php?id=161908</link>
		<pubDate>Mon, 03 Sep 2012 22:29:02 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161908" length="365577754" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Hotel.Hell.S01E05.HDTV.x264-LOL.[eztv].torrent</fileName>
			<contentLength>365577754</contentLength>
			<infoHash>e09a4b12e01470ae92ea5d51471a2611638a4ed9</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:e09a4b12e01470ae92ea5d51471a2611638a4ed9&dn=Hotel Hell S01E05 HDTV x264-LOL [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[The.LA.Complex.S02E08.HDTV.x264-2HD.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161907]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161907</guid>
		<link>http://www.bt-chat.com/download1.php?id=161907</link>
		<pubDate>Mon, 03 Sep 2012 21:37:43 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161907" length="390095486" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>The.LA.Complex.S02E08.HDTV.x264-2HD.[eztv].torrent</fileName>
			<contentLength>390095486</contentLength>
			<infoHash>866285663f53edb2dd7d44bcbc42cd1f68e39276</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:866285663f53edb2dd7d44bcbc42cd1f68e39276&dn=The LA Complex S02E08 HDTV x264-2HD [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Hell.on.Wheels.S02E04.720p.HDTV.x264-IMMERSE.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161906]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161906</guid>
		<link>http://www.bt-chat.com/download1.php?id=161906</link>
		<pubDate>Mon, 03 Sep 2012 21:03:06 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161906" length="1521164545" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Hell.on.Wheels.S02E04.720p.HDTV.x264-IMMERSE.[eztv].torrent</fileName>
			<contentLength>1521164545</contentLength>
			<infoHash>5bebfea67f3cf27ff29cbc4a2a177681de4e5889</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:5bebfea67f3cf27ff29cbc4a2a177681de4e5889&dn=Hell on Wheels S02E04 720p HDTV x264-IMMERSE [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Copper.S01E03.HDTV.x264-LOL.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161904]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161904</guid>
		<link>http://www.bt-chat.com/download1.php?id=161904</link>
		<pubDate>Mon, 03 Sep 2012 20:16:22 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161904" length="265985842" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Copper.S01E03.HDTV.x264-LOL.[eztv].torrent</fileName>
			<contentLength>265985842</contentLength>
			<infoHash>27db4db206d0e4f97f8e3cfb1b6cdb0315f5c349</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:27db4db206d0e4f97f8e3cfb1b6cdb0315f5c349&dn=Copper S01E03 HDTV x264-LOL [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Beaver.Falls.S02E05.HDTV.x264-TLA.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161903]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161903</guid>
		<link>http://www.bt-chat.com/download1.php?id=161903</link>
		<pubDate>Mon, 03 Sep 2012 19:11:54 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161903" length="406231160" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Beaver.Falls.S02E05.HDTV.x264-TLA.[eztv].torrent</fileName>
			<contentLength>406231160</contentLength>
			<infoHash>4c008b137ef788838a3b0aa6e831965dd140f828</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:4c008b137ef788838a3b0aa6e831965dd140f828&dn=Beaver Falls S02E05 HDTV x264-TLA [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Beaver.Falls.S02E05.720p.HDTV.x264-TLA.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161902]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161902</guid>
		<link>http://www.bt-chat.com/download1.php?id=161902</link>
		<pubDate>Mon, 03 Sep 2012 19:07:30 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161902" length="1192740441" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Beaver.Falls.S02E05.720p.HDTV.x264-TLA.[eztv].torrent</fileName>
			<contentLength>1192740441</contentLength>
			<infoHash>4202eff794ec542a9726828bb464c5966e6b3a63</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:4202eff794ec542a9726828bb464c5966e6b3a63&dn=Beaver Falls S02E05 720p HDTV x264-TLA [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[5.inch.Floppy.s03e22.Dark.Souls.PC.hdtv.h264-5if.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161882]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161882</guid>
		<link>http://www.bt-chat.com/download1.php?id=161882</link>
		<pubDate>Mon, 03 Sep 2012 07:54:05 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161882" length="173550480" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>5.inch.Floppy.s03e22.Dark.Souls.PC.hdtv.h264-5if.[eztv].torrent</fileName>
			<contentLength>173550480</contentLength>
			<infoHash>7509c897ef883af9bf6c864ef064c514b57760a2</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:7509c897ef883af9bf6c864ef064c514b57760a2&dn=5 inch Floppy s03e22 Dark Souls PC hdtv h264-5if [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[NewGamePlus.S01E20.720p.HDTV.x264-NGPcRew.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161877]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161877</guid>
		<link>http://www.bt-chat.com/download1.php?id=161877</link>
		<pubDate>Mon, 03 Sep 2012 05:32:20 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161877" length="576662104" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>NewGamePlus.S01E20.720p.HDTV.x264-NGPcRew.[eztv].torrent</fileName>
			<contentLength>576662104</contentLength>
			<infoHash>6b060602319e67f63635e12331ed506a1f83c0e8</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:6b060602319e67f63635e12331ed506a1f83c0e8&dn=NewGamePlus S01E20 720p HDTV x264-NGPcRew [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[NewGamePlus.S01E20.HDTV.x264-NGPcRew.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161876]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161876</guid>
		<link>http://www.bt-chat.com/download1.php?id=161876</link>
		<pubDate>Mon, 03 Sep 2012 05:31:51 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161876" length="232465203" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>NewGamePlus.S01E20.HDTV.x264-NGPcRew.[eztv].torrent</fileName>
			<contentLength>232465203</contentLength>
			<infoHash>1860bbd852201adff57ef95eb685db63a5aecdf9</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:1860bbd852201adff57ef95eb685db63a5aecdf9&dn=NewGamePlus S01E20 HDTV x264-NGPcRew [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Breaking.Bad.S05E08.720p.HDTV.x264-IMMERSE.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161875]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161875</guid>
		<link>http://www.bt-chat.com/download1.php?id=161875</link>
		<pubDate>Mon, 03 Sep 2012 05:15:51 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161875" length="1170208483" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Breaking.Bad.S05E08.720p.HDTV.x264-IMMERSE.[eztv].torrent</fileName>
			<contentLength>1170208483</contentLength>
			<infoHash>a78f213540d3c5f42425601c0f0822949bb75470</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:a78f213540d3c5f42425601c0f0822949bb75470&dn=Breaking Bad S05E08 720p HDTV x264-IMMERSE [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Breaking.Bad.S05E08.HDTV.x264-ASAP.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161866]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161866</guid>
		<link>http://www.bt-chat.com/download1.php?id=161866</link>
		<pubDate>Mon, 03 Sep 2012 01:16:40 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161866" length="340717567" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Breaking.Bad.S05E08.HDTV.x264-ASAP.[eztv].torrent</fileName>
			<contentLength>340717567</contentLength>
			<infoHash>3c5f2d86ce310c5a29dc90ab08a6f590d9a99416</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:3c5f2d86ce310c5a29dc90ab08a6f590d9a99416&dn=Breaking Bad S05E08 HDTV x264-ASAP [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Totally.Biased.with.W.Kamau.Bell.S01E04.HDTV.x264-2HD.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161864]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161864</guid>
		<link>http://www.bt-chat.com/download1.php?id=161864</link>
		<pubDate>Mon, 03 Sep 2012 01:01:49 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161864" length="224588464" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Totally.Biased.with.W.Kamau.Bell.S01E04.HDTV.x264-2HD.[eztv].torrent</fileName>
			<contentLength>224588464</contentLength>
			<infoHash>993b5bc10e336e329992f3dff70ea9587e6b4ec3</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:993b5bc10e336e329992f3dff70ea9587e6b4ec3&dn=Totally Biased with W Kamau Bell S01E04 HDTV x264-2HD [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Weeds.S08E10.720p.HDTV.x264-EVOLVE.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161863]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161863</guid>
		<link>http://www.bt-chat.com/download1.php?id=161863</link>
		<pubDate>Mon, 03 Sep 2012 00:46:18 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161863" length="599241079" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Weeds.S08E10.720p.HDTV.x264-EVOLVE.[eztv].torrent</fileName>
			<contentLength>599241079</contentLength>
			<infoHash>bb3ee549a9d0fbdae77d393324cbf9fde345a0ba</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:bb3ee549a9d0fbdae77d393324cbf9fde345a0ba&dn=Weeds S08E10 720p HDTV x264-EVOLVE [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Web.Therapy.S02E10.HDTV.x264-EVOLVE.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161862]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161862</guid>
		<link>http://www.bt-chat.com/download1.php?id=161862</link>
		<pubDate>Mon, 03 Sep 2012 00:40:55 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161862" length="131230819" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Web.Therapy.S02E10.HDTV.x264-EVOLVE.[eztv].torrent</fileName>
			<contentLength>131230819</contentLength>
			<infoHash>04c18d6cdb772ecbe80e8c072b87ba98efec10b8</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:04c18d6cdb772ecbe80e8c072b87ba98efec10b8&dn=Web Therapy S02E10 HDTV x264-EVOLVE [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Sinbad.S01E09.HDTV.x264-KILLERS.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161861]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161861</guid>
		<link>http://www.bt-chat.com/download1.php?id=161861</link>
		<pubDate>Mon, 03 Sep 2012 00:39:49 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161861" length="369261003" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Sinbad.S01E09.HDTV.x264-KILLERS.[eztv].torrent</fileName>
			<contentLength>369261003</contentLength>
			<infoHash>0b30fced163a450de0d9e2eb90fcc65e004f6715</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:0b30fced163a450de0d9e2eb90fcc65e004f6715&dn=Sinbad S01E09 HDTV x264-KILLERS [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Weeds.S08E10.HDTV.x264-SYS.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161860]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161860</guid>
		<link>http://www.bt-chat.com/download1.php?id=161860</link>
		<pubDate>Mon, 03 Sep 2012 00:38:59 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161860" length="206396221" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Weeds.S08E10.HDTV.x264-SYS.[eztv].torrent</fileName>
			<contentLength>206396221</contentLength>
			<infoHash>c08546de2f7d54d6b74501c18d83ae338f0e8eb5</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:c08546de2f7d54d6b74501c18d83ae338f0e8eb5&dn=Weeds S08E10 HDTV x264-SYS [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Leverage.S05E07.HDTV.x264-ASAP.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161858]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161858</guid>
		<link>http://www.bt-chat.com/download1.php?id=161858</link>
		<pubDate>Sun, 02 Sep 2012 23:29:08 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161858" length="357283239" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Leverage.S05E07.HDTV.x264-ASAP.[eztv].torrent</fileName>
			<contentLength>357283239</contentLength>
			<infoHash>2f60a1529e060f6033fc103f19582fe756d37a2e</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:2f60a1529e060f6033fc103f19582fe756d37a2e&dn=Leverage S05E07 HDTV x264-ASAP [eztv]]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
<item>

		<title><![CDATA[Sinbad.S01E08.HDTV.x264-KILLERS.[eztv].torrent]]></title>
		<category>TV - EZTV</category>
		<source url="http://www.bt-chat.com/rss.php?group=3">EZTV</source>
		<description><![CDATA[http://www.bt-chat.com/details.php?id=161800]]></description>
		<guid isPermaLink="true">http://www.bt-chat.com/details.php?id=161800</guid>
		<link>http://www.bt-chat.com/download1.php?id=161800</link>
		<pubDate>Sun, 02 Sep 2012 00:15:58 EST</pubDate>
		<enclosure url="http://www.bt-chat.com/download1.php?id=161800" length="504272023" type="application/x-bittorrent" />
		<torrent xmlns="http://xmlns.bt-chat.com/0.1/">
			<fileName>Sinbad.S01E08.HDTV.x264-KILLERS.[eztv].torrent</fileName>
			<contentLength>504272023</contentLength>
			<infoHash>4bcbd4dd8215c5a4afca23b812dc5bbf16186dbe</infoHash>
			<magnetURI><![CDATA[magnet:?xt=urn:btih:4bcbd4dd8215c5a4afca23b812dc5bbf16186dbe&dn=Sky One Sinbad.S01E08.HDTV.x264-KILLERS]]></magnetURI>
			<trackers>
				<group order="random">
					<tracker><![CDATA[udp://tracker.openbittorrent.com:80/]]></tracker>
				</group>
			</trackers>
		</torrent>
	</item>
</channel>
	
7
</rss>

1e

<!-- cache_page from apc -->

0



*/