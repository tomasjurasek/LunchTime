﻿using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using LunchTime.Models;

namespace LunchTime.Restaurants
{
    public class NaKnofliku : RestaurantBase
    {
        public override string Name => "Na Knoflíku";

        public override string Url => "http://www.brnorestaurace.cz/tydenni-menu/";

        public override string Web => "";

        public override LunchMenu Get()
        {
            var lunchMenu = Create();
            var web = Fetch();
            var menu = web.DocumentNode.SelectNodes("//*[@id=\"col1\"]/table")[0];
            lunchMenu.DailyMenus = GetDailyMenus(menu);
            return lunchMenu;
        }

        private static List<DailyMenu> GetDailyMenus(HtmlNode menu)
        {
            var items = menu.SelectNodes(".//tr").ToArray();
            var dailyMenus = new List<DailyMenu>();
            int day = 0;
            for (var i = 0; i < items.Length; i = i + 7)
            {
                var dailyMenu = new DailyMenu(StartOfWeek().AddDays(day));
                var soups = GetSoups(items[i+1]);
                if (string.IsNullOrEmpty(soups.First().Name))
                {
                    break;
                }
                dailyMenu.Soups = soups;
                dailyMenu.Meals = GetMeals(new[] { items[i + 2], items[i + 3], items[i+4], items[i+5], items[i + 6] });
                dailyMenus.Add(dailyMenu);
                day++;
            }
            return dailyMenus;
        }

        private static List<Meal> GetMeals(HtmlNode[] items)
        {
            return items.Select(GetMeal).ToList();
        }

        private static List<Soup> GetSoups(HtmlNode day)
        {
            var soup = new Soup(day.SelectNodes(".//td[2]")[0].InnerText);
            return new List<Soup> { soup };
        }

        private static Meal GetMeal(HtmlNode mealNode)
        {
            var name = mealNode.SelectNodes(".//td[2]")[0].InnerText;
            var price = HttpUtility.HtmlDecode(mealNode.SelectNodes(".//td[3]")[0].InnerText);
            return new Meal(name, price);
        }
    }
}