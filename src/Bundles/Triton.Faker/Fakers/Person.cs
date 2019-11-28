using System;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Resources;
using static TheXDS.Triton.Fakers.Globals;

namespace TheXDS.Triton.Fakers
{
    public class Person
    {
        private string _userName;

        public string FirstName { get; }
        public string Surname { get; }
        public Gender Gender { get; }
        public DateTime Birth { get; }
        public string UserName => _userName ??= FakeUsername(this);
        public string Name => string.Join(' ', FirstName, Surname);
        public string FullName => string.Join(", ", Surname, FirstName);
        public double Age => (DateTime.Today - Birth).TotalDays / 365.25;

        private Person(string firstName, string surname, Gender gender, DateTime birth)
        {
            FirstName = firstName;
            Surname = surname;
            Gender = gender;
            Birth = birth;
        }

        public static Person Adult()
        {
            return Someone(18, 60);
        }

        public static Person Kid()
        {
            return Someone(5, 18);
        }

        public static Person Baby()
        {
            return Someone(0, 5);
        }

        public static Person Old()
        {
            return Someone(60, 110);
        }

        public static Person Someone(int minAge, int maxAge)
        {
            var m = _rnd.CoinFlip();

            return new Person(
                (m ? StringTables.MaleNames : StringTables.FemaleNames).Pick(),
                StringTables.Surnames.Pick(),
                m ? Gender.Male : Gender.Female,
                FakeBirth(minAge, maxAge));
        }

        public static Person Someone()
        {
            return new Func<Person>[] {Baby, Kid, Adult, Old }.Pick().Invoke();
        }

        public static DateTime FakeBirth(int minAge, int maxAge)
        {
            var a = _rnd.NextDouble();
            return (DateTime.Today - TimeSpan.FromDays(((a * (maxAge - minAge)) + minAge)*365.25)).Date;
        }

        public static string FakeUsername(Person person)
        {
            var sb = new StringBuilder();
            var rounds = _rnd.Next(1, 4);
            sb.Append(new[] { person.Surname, person.FirstName, StringTables.Lorem.Pick() }.Pick());

            do
            {
                if (_rnd.CoinFlip()) sb.Append('_');
                sb.Append(new[] { StringTables.Lorem.Pick(), person.Birth.Year.ToString(), person.Birth.Year.ToString().Substring(2), _rnd.Next(0, 1000).ToString().PadLeft(_rnd.Next(1, 4), '0') }.Pick());
            } while (--rounds > 0);

            return sb.ToString().ToLower();

        }

        public static string FakeUsername()
        {
            var sb = new StringBuilder();
            var rounds = _rnd.Next(1, 4);

            sb.Append(StringTables.Lorem.Pick());
            do
            {
                if (_rnd.CoinFlip()) sb.Append('_');
                sb.Append(new[] { StringTables.Lorem.Pick(), _rnd.Next(0, 10000).ToString().PadLeft(_rnd.Next(1, 5), '0') }.Pick());
            } while (--rounds > 0);

            return sb.ToString();
        }

    }
}
