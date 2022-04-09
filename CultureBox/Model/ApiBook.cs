using System;
using System.Collections.Generic;

namespace CultureBox.Model
{
    public class ApiBook
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public string Description { get; set; }
        public List<string> Themes { get; set; }
        public string ISBN { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj == this)
            {
                return true;
            }

            ApiBook b = (ApiBook) obj;

            bool allAuthors = true;

            foreach (var author in Authors)
            {
                if (!b.Authors.Contains(author))
                {
                    allAuthors = false;
                    break;
                }
            }

            bool allThemes = true;
            foreach (var theme in Themes)
            {
                if (!b.Themes.Contains(theme))
                {
                    allThemes = false;
                    break;
                }
            }

            return Id == b.Id && Equals(Title, b.Title) && allThemes && allAuthors &&
                   Equals(Description, b.Description) && Equals(ISBN, b.ISBN);
        }

        protected bool Equals(ApiBook other)
        {
            return Id == other.Id && Title == other.Title && Equals(Authors, other.Authors) && Description == other.Description && Equals(Themes, other.Themes) && ISBN == other.ISBN;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Authors, Description, Themes, ISBN);
        }
    }
}
