using System;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using VitrinConsole.Model;

namespace VitrinConsole
{
    class Program
    {
        static List<Advert> adverts; // İlan listesi
        static void Main(string[] args)
        {
            // VitrinSave();

            // CreateTitlesUrlsTxt();
            
            // CreatePricesTxt();

            ParseModel();

            foreach (var advert in adverts)
            {
                System.Console.WriteLine(advert.Url);
                System.Console.WriteLine(advert.Title);
                System.Console.WriteLine(advert.Price);
                System.Console.WriteLine("\n\n");
            }

            Console.WriteLine("Ortalama Fiyat: "+TotalAverage());
        }
        /// <summary>
        ///     Title.txt ve urls.txt dosyasını oluşturur.
        /// </summary>
        static void CreateTitlesUrlsTxt()
        {
            using (StreamWriter titlesw = new StreamWriter("titles.txt"))
            {
                var html = @"https://www.sahibinden.com/";

                HtmlWeb web = new HtmlWeb();
                
                var htmlDoc = web.Load(html);

                HtmlNodeCollection node = htmlDoc.DocumentNode.SelectNodes("//*[@id=\"container\"]/div[3]/div/div[3]/div[3]/ul");

                using (StreamWriter urlsw = new StreamWriter("urls.txt"))
                {
                    foreach (var ul in node)
                    {
                        foreach (var li in ul.ChildNodes)
                        {
                            foreach (var a in li.ChildNodes)
                            {
                                var attrTitle = a.ChildAttributes("title");

                                foreach (var title in attrTitle)
                                {
                                    titlesw.WriteLine(title.Value);
                                    if (title.Value != "")
                                    {
                                        var attrHref = a.ChildAttributes("href");

                                        foreach (var item in attrHref)
                                        {
                                            urlsw.WriteLine("https://www.sahibinden.com/" + item.Value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        ///     Urls.txt dosyasını okur ve price.txt dosyasını oluşturur.
        /// </summary>
        static void CreatePricesTxt()
        {
            var s = File.ReadAllLines("urls.txt");
            
            using (StreamWriter sw = new StreamWriter("prices.txt"))
            {
                for (var i = 0; i < s.Length; i++)
                {
                    HtmlWeb tempweb = new HtmlWeb();

                    string Url = s[i];

                    var httpdoc = tempweb.Load(Url);

                    HtmlNodeCollection nodePrice = httpdoc.DocumentNode.SelectNodes("//*[@id=\"classifiedDetail\"]/div/div[2]/div[2]/h3/text()");

                    sw.WriteLine(nodePrice[0].InnerHtml.Trim().Replace(" ","").Replace("TL","").Replace(",",""));
                    System.Threading.Thread.Sleep(3000);
                }
            }
        }
        /// <summary>
        ///     Prices.txt dosyasındaki fiyatların ortalamasını alır.
        /// </summary>
        /// <returns>Double Ortalama</returns>
        static double TotalAverage()
        {
            var result = File.ReadAllLines("prices.txt");
            
            double[] prices = new double[result.Length];

            int i = 0;

            foreach (var price in result)
            {
                prices[i] = Convert.ToDouble(price);
                i++;
            }

            double total = 0;

            for (var j = 0; j < prices.Length; j++)
            {
                total += prices[j];
            }
            
            return total/prices.Length;
        }
        /// <summary>
        ///     Oluşturulan prices.txt , titles.txt, urls.txt dosyalarını List'eye aktarır.
        /// </summary>
        static void ParseModel()
        {
            adverts = new();
            var prices = File.ReadAllLines("prices.txt");
            var titles = File.ReadAllLines("titles.txt");
            var urls = File.ReadAllLines("urls.txt");

            for (var i = 0; i < prices.Length; i++)
            {
                Advert advert = new Advert();
                advert.Url = urls[i];
                advert.Price = Convert.ToDouble(prices[i]);
                advert.Title = titles[i];
                adverts.Add(advert);
            }
        }
        /// <summary>
        ///     https://www.sahibinden.com adresini locale kaydeder.
        ///     Siteye fazla istek sonucu bloklanmamak için kullanılabilir.
        /// </summary>
        static void VitrinSave()
        {

            var html = @"https://www.sahibinden.com/";

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(html);

            htmlDoc.Save("sahibinden.html");
        }
    }
}