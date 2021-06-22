using System;
using System.Collections.Generic;
using System.Text;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Faker.Resources;
using static TheXDS.Triton.Fakers.Globals;

namespace TheXDS.Triton.Fakers
{
    /// <summary>
    /// Representa a una persona generada aleatoriamente, además de que
    /// contiene métodos estáticos que generan nuevas instancias
    /// aleatorias de esta clase.
    /// </summary>
    public class Person
    {
        private string? _userName;

        /// <summary>
        /// Obtiene el primer nombre de la persona.
        /// </summary>
        public string FirstName { get; }

        /// <summary>
        /// Obtiene el apellido de la persona.
        /// </summary>
        public string Surname { get; }

        /// <summary>
        /// Obtiene el género biológico de la persona.
        /// </summary>
        public Gender Gender { get; }

        /// <summary>
        /// Obtiene la fecha de nacimiento de la persona.
        /// </summary>
        public DateTime Birth { get; }

        /// <summary>
        /// Obtiene un nombre de usuario generado para la persona.
        /// </summary>
        public string UserName => _userName ??= FakeUsername(this);

        /// <summary>
        /// Obtiene el nombre completo de la persona.
        /// </summary>
        public string Name => string.Join(' ', FirstName, Surname);

        /// <summary>
        /// Obtiene el nombre completo, como "Apellido, Nombre" de la persona.
        /// </summary>
        public string FullName => string.Join(", ", Surname, FirstName);

        /// <summary>
        /// Calcula y obtiene la edad de la persona el día de hoy.
        /// </summary>
        public double Age => (DateTime.Today - Birth).TotalDays / 365.25;

        private Person(string firstName, string surname, Gender gender, DateTime birth)
        {
            FirstName = Capitalize(firstName);
            Surname = Capitalize(surname);
            Gender = gender;
            Birth = birth;
        }

        /// <summary>
        /// Genera un adulto completamente aleatorio.
        /// </summary>
        /// <returns>Un adulto completamente aleatorio.</returns>
        public static Person Adult()
        {
            return Someone(18, 60);
        }

        /// <summary>
        /// Genera un niño completamente aleatorio.
        /// </summary>
        /// <returns>Un niño completamente aleatorio.</returns>
        public static Person Kid()
        {
            return Someone(5, 18);
        }

        /// <summary>
        /// Genera un bebé completamente aleatorio.
        /// </summary>
        /// <returns>Un bebé completamente aleatorio.</returns>
        public static Person Baby()
        {
            return Someone(0, 5);
        }

        /// <summary>
        /// Genera un adulto mayor completamente aleatorio.
        /// </summary>
        /// <returns>Un adulto mayor completamente aleatorio.</returns>
        public static Person Old()
        {
            return Someone(60, 110);
        }

        /// <summary>
        /// Genera una persona totalmente aleatoria.
        /// </summary>
        /// <param name="minAge">Edad mínima de la persona.</param>
        /// <param name="maxAge">Edad máxima de la persona.</param>
        /// <returns>
        /// Una persona totalmente aleatoria cuya edad se encuentra dentro del
        /// rango especificado de edades.
        /// </returns>
        public static Person Someone(int minAge, int maxAge)
        {
            var m = _rnd.CoinFlip();

            return new Person(
                (m ? StringTables.MaleNames : StringTables.FemaleNames).Pick(),
                StringTables.Surnames.Pick(),
                m ? Gender.Male : Gender.Female,
                FakeBirth(minAge, maxAge));
        }

        /// <summary>
        /// Genera una persona totalmente aleatoria.
        /// </summary>
        /// <returns>
        /// Una persona totalmente aleatoria.
        /// </returns>
        public static Person Someone()
        {
            return new Func<Person>[] { Baby, Kid, Adult, Old }.Pick().Invoke();
        }

        /// <summary>
        /// Genera una fecha aleatoria cuya edad en años se encuentra dentro
        /// del rango de edad especificado.
        /// </summary>
        /// <param name="minAge">Edad mínima a generar.</param>
        /// <param name="maxAge">Edad máxima a generar.</param>
        /// <returns>
        /// Una fecha aleatoria, la cual al ser aplicada a una persona, le
        /// proporciona de una edad que se encuentra dentro del rango de edades
        /// especificado.
        /// </returns>
        public static DateTime FakeBirth(int minAge, int maxAge)
        {
            var a = _rnd.NextDouble();
            return (DateTime.Today - TimeSpan.FromDays(((a * (maxAge - minAge)) + minAge) * 365.25)).Date;
        }

        /// <summary>
        /// Genera un nombre de usuario aleatorio satisfactorio para la persona
        /// especificada.
        /// </summary>
        /// <param name="person">
        /// Persona para la cual generar un nombre de usuario aleatorio.
        /// </param>
        /// <returns>
        /// Un nombre de usuario aleatorio basado en las propiedades de la 
        /// persona especificada.
        /// </returns>
        public static string FakeUsername(Person? person)
        {
            person ??= Someone();
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

        /// <summary>
        /// Genera un nombre de usuario totalmente aleatorio.
        /// </summary>
        /// <returns>Un nombre de usuario totalmente aleatorio.</returns>
        public static string FakeUsername()
        {
            var sb = new StringBuilder();
            var rounds = _rnd.Next(1, 4);

            sb.Append(StringTables.Lorem.Pick());
            do
            {
                if (_rnd.CoinFlip()) sb.Append('_');
                sb.Append(new[] { StringTables.Lorem.Pick(), _rnd.Next(0, 10000).ToString().PadLeft(_rnd.Next(1, 5), '0')}.Pick());
            } while (--rounds > 0);

            return sb.ToString();
        }

        /// <summary>
        /// Genera una dirección de correo electrónico aleatoria para el objeto
        /// <see cref="Person"/> especificado.
        /// </summary>
        /// <param name="person">
        /// Persona para la cual generar la dirección de correo.
        /// </param>
        /// <returns>
        /// Una dirección de correo con un formato válido. Subsecuentes
        /// llamadas a este método podrán obtener direcciones de correo del
        /// mismo dominio.
        /// </returns>
        public static string FakeEmail(Person? person)
        {
            return $"{FakeUsername(person ?? Someone())}@{(fakeDomains ??=LoadDomains()).Pick()}";
        }

        /// <summary>
        /// Genera una dirección de correo totalmente aleatoria.
        /// </summary>
        /// <returns>
        /// Una dirección de correo con un formato válido. Subsecuentes
        /// llamadas a este método podrán obtener direcciones de correo del
        /// mismo dominio.
        /// </returns>
        public static string FakeEmail()
        {
            return $"{FakeUsername()}@{(fakeDomains ??= LoadDomains()).Pick()}";
        }

        private static string[] LoadDomains()
        {
            string[] top = { "com", "net", "edu", "gov" };
            List<string> domains = new();
            while (domains.Count < 5)
            {
                domains.Add($"{GetName()}{GetName()}.{top.Pick()}");
            }return domains.ToArray();
        }

        private static string GetName()
        {
            return new[] { StringTables.MaleNames, StringTables.FemaleNames, StringTables.Surnames, StringTables.Lorem }.Pick().Pick();
        }

        private static string[]? fakeDomains;
    }
}
