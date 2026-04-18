using ChitalishteIskra.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChitalishteIskra.Data.Seed
{
    public static class EventSeeder
    {
        public static async Task SeedAsync(ChitalishteIskraDbContext context)
        {
            await SeedEventAsync(context,
              "Лекция - 100 години от рождението на Луна Давидова",
              new DateOnly(2026, 4, 21),
              new TimeOnly(19, 0),
              new TimeOnly(21, 0),
              "Музей на фотографията",
              " През 2026 година се навършват 100 години от рождението на актрисата Луна Давидова — родена в Казанлък и оставила ярка следа в историята на българския театър и кино. По този повод публиката ще има възможност да присъства на лекцията „За ролите, които остават“ — събитие, посветено на нейния творчески път и културно наследство.\r\n\r\nЛуна Давидова принадлежи към поколението актьори, за които сцената е призвание и духовна необходимост. Със своята сдържана емоционалност, психологическа дълбочина и човешка искреност тя създава образи, които не търсят външен ефект, а истинско съдържание — роли, оставащи живи в паметта на зрителите дълго след финалните аплодисменти.\r\n\r\nЛекцията ще предложи поглед към личността и творчеството на актрисата, както и разговор за паметта в театъра, за смисъла на сценичното присъствие и за онова, което остава отвъд времето и ролите.\r\n\r\nСъбитието е част от програмата на Национален фестивал на детско-юношеското театрално изкуство „Театрални искри“, който събира млади театрални творци и професионалисти от цялата страна.",
              "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776524041/d6kn88ykvodyldx8r5em.jpg");

            await SeedEventAsync(context,
                "IV Национален фестивал на детско - юношеското театрално изкуство “Театрални искри“ 23 - 26 април 2026",
                new DateOnly(2026, 4, 23),
                new TimeOnly(18, 30),
                new TimeOnly(20, 0),
                "Салон на читалището",
                "",
                "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776521906/sj7txzf88xf4kb3alzd2.png");

            await SeedEventAsync(context,
                "Поклон, учителю!",
                new DateOnly(2026, 12, 20),
                new TimeOnly(17, 0),
                new TimeOnly(19, 0),
                "Южно фоайе",
                " Поклон, учителю!\r\n\r\nКонцерт, посветен на първия български композитор – Емануил Манолов.\r\n\r\n\r\nШкола по изкуствата „Емануил Манолов“ има удоволствието да представи\r\nконцерт, посветен на първия български композитор – Емануил Манолов.\r\nНашите възпитаници ще изпълнят подбрана програма в знак на почит към делото и приноса на автора на първата българска опера „Сиромахкиня“.\r\n\r\n\r\nОчакваме ви с вълнение и вдъхновение!\r\n\r\n ",
                "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776522113/ybuqc3uiy33rtnyrh0cr.png");

            await SeedEventAsync(context,
                "Огън от любов ",
                new DateOnly(2026, 12, 20),
                new TimeOnly(17, 0),
                new TimeOnly(19, 0),
                "Художествена галерия",
                "",
                "https://res.cloudinary.com/dxloy3tkq/image/upload/v1776528062/mu7sgyxznawoitqfzite.jpg");

            await context.SaveChangesAsync();
        }

        private static async Task SeedEventAsync(
    ChitalishteIskraDbContext context,
    string name,
    DateOnly date,
    TimeOnly startTime,
    TimeOnly endTime,
    string location,
    string description,
    string imageUrl)
        {
            var existingEvent = await context.Events
                .FirstOrDefaultAsync(e =>
                    e.Name == name &&
                    e.Date == date &&
                    e.StartTime == startTime &&
                    e.Location == location);

            if (existingEvent == null)
            {
                await context.Events.AddAsync(new Event
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Date = date,
                    StartTime = startTime,
                    EndTime = endTime,
                    Location = location,
                    Description = description,
                    ImageUrl = imageUrl
                });
            }
            else
            {
                existingEvent.EndTime = endTime;
                existingEvent.Description = description;
                existingEvent.ImageUrl = imageUrl;
            }
        }

    }
}