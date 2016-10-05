using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MyAmdm.Models
{
    public class AuthorDbInitializer : DropCreateDatabaseAlways<SongContext>
    {
        protected override void Seed(SongContext db)
        {
            db.Authors.Add(new Author { Name = "Фактор 2", Biography = "Русскоязычная музыкальная поп-группа. До августа 2012 года состояла из Владимира Панченко и Ильи Подстрелова" });
            db.Authors.Add(new Author { Name = "Би 2", Biography = " Российская рок-группа. Основателями группы являются Шура Би-2 и Лёва Би-2." });
            db.Authors.Add(new Author { Name = "Звери", Biography = "Российская поп-рок-группа, созданная Романом Билыком в 2001 году. Лауреат премий «MTV Россия» и премии «Дебют»." });

            base.Seed(db);
        }
    }
}