using System;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace GB2260
{
    public class GB2260
    {
        private static Revisions rvs;
        private SortedList<int, string> data = new SortedList<int, string>();

        public List<string> AvailableRevisions { get { return rvs.Stats; } }
        public List<Division> Provinces { get; } = new List<Division>();
        public string Revision { get; private set; }

        public GB2260()
        {
            LoadRevisions();
            Revision = rvs.FindLatestRevision();
            LoadData(Revision);
        }

        public GB2260(string revision)
        {
            LoadRevisions();
            Revision = revision;
            LoadData(Revision);
        }

        private void LoadRevisions()
        {
            if (rvs != null) return;
            try
            {
                var path = Path.Combine(Assembly.GetExecutingAssembly().Location, @"..\..\..\");
                var revisionContent = File.ReadAllText(string.Format(@"{0}\resources\GB2260\revisions.json", path));
                rvs = new JavaScriptSerializer().Deserialize<Revisions>(revisionContent);
            }
            catch (IOException)
            {
                throw new IOException("Error in loading revision data");
            }
        }
        private void LoadData(string revision)
        {
            try
            {
                var path = Path.Combine(Assembly.GetExecutingAssembly().Location, @"..\..\..\");

                if (!AvailableRevisions.Contains(revision)) throw new IOException("Error in loading revision data");

                var lines = File.ReadAllLines(string.Format(@"{0}\resources\GB2260\stats\{1}.tsv", path, revision)).ToList();
                lines.RemoveAt(0);
                lines.ForEach(l =>
                {
                    string[] record = Regex.Split(l, @"\s+");
                    string code = record[2];
                    string name = record[3];
                    data.Add(int.Parse(code), name);
                    if (Regex.Match(code, @"^\d{2}0{4}$").Success)
                        Provinces.Add(new Division(code, name, revision));
                });
            }
            catch (IOException)
            {
                throw new IOException("Error in loading division data");
            }
        }

        public Division Division(string code)
        {
            if (code.Length > 6 || !data.ContainsKey(int.Parse(code)))
                throw new InvalidCodeException("Code for division not found");

            Division division = new Division(code, data[int.Parse(code)], Revision);

            if (Regex.Match(code, @"^\d{2}0{4}$").Success)
                return division;

            division.Province = data[int.Parse(code.Substring(0, 2) + "0000")];
            if (Regex.Match(code, @"^\d{4}0{2}$").Success)
                return division;

            division.Prefecture = data[int.Parse(code.Substring(0, 4) + "00")];
            return division;
        }

        public List<Division> Prefectures(string code)
        {
            if (!Regex.Match(code, @"^\d{2}0{4}$").Success && !Regex.Match(code, @"^\d{2}0{2}$").Success && !Regex.Match(code, @"^\d{2}$").Success)
                throw new InvalidCodeException("Invalid code pattern");
            var keyStartInt = int.Parse(code.Substring(0, 2));
            if (!data.ContainsKey(keyStartInt * 10000))
                throw new InvalidCodeException("Code for division not found");
            var province = data[keyStartInt * 10000];
            var prefectures = data.Where(d => d.Key / 10000 == keyStartInt && d.Key % 100 == 0).Select(d => new Division(d.Key.ToString(), d.Value, Revision, null, province)).ToList();
            return prefectures;
        }

        public List<Division> Countries(string code)
        {
            if (!Regex.Match(code, @"^\d{4}0{2}$").Success && !Regex.Match(code, @"^\d{4}$").Success)
                throw new InvalidCodeException("Invalid code pattern");
            var province = data[int.Parse(code.Substring(0, 2)) * 10000];
            var keyStartInt = int.Parse(code.Substring(0, 4));
            if (!data.ContainsKey(keyStartInt * 100))
                throw new InvalidCodeException("Code for division not found");
            var prefecture = data[keyStartInt * 100];
            var countries = data.Where(d => d.Key / 100 == keyStartInt).Select(d => new Division(d.Key.ToString(), d.Value, Revision, prefecture, province)).ToList();
            return countries;
        }

        private class Revisions
        {
            public List<string> Gb { get; set; }
            public List<string> Mca { get; set; }
            public List<string> Stats { get; set; }

            public string FindLatestRevision()
            {
                return Stats.Select(s => int.Parse(s)).Max().ToString();
            }
        }
    }
}
